using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using NbgHackathon.Domain;

namespace NbgHackathon.Passport
{
    public static class PassportImageProcess
    {
        const string FunctionName = "PassportImageProcess";

        [FunctionName(FunctionName)]
        public static async Task Run([BlobTrigger("passports/{fileName}.{fileExtension}", Connection = "MainApplicationStorage")]Stream myBlob, string fileName, string fileExtension, TraceWriter log)
        {
            var onboardingRepository = ServiceLocator.ResolveRepository();
            if (!Guid.TryParse(fileName, out var onboardingId))
            {
                throw new ArgumentException($"The blob's filename ({fileName}) could not be parsed as an onboarding session identifier");
            }

            var onboarding = await onboardingRepository.Get(onboardingId);
            if (onboarding == null)
            {
                throw new InvalidOperationException($"No onboarding session could be located for the identifier: {onboardingId}");
            }

            var passportParser = CreatePassportParser();
            var passportParseResult = await passportParser.ParsePassport(myBlob);
            if (passportParseResult.Success())
            {
                var passportDocument = passportParseResult.PassportDocument;
                var passportInfo = new PassportInformation()
                {
                    Gender = passportDocument.Sex,
                    DateOfBirth = passportDocument.BirthDateVerified
                        ? DateTime.ParseExact(passportDocument.BirthDate, "yyMMdd", CultureInfo.InvariantCulture)
                        : default(DateTime?),
                    ExpirationDate = passportDocument.ExpiryDateVerified
                        ? DateTime.ParseExact(passportDocument.ExpiryDate, "yyMMdd", CultureInfo.InvariantCulture)
                        : default(DateTime?),
                    Firstname = passportDocument.GivenName,
                    Surname = passportDocument.LastName,
                    PassportNumber = passportDocument.DocumentNumber,
                    IssuingState = passportDocument.IssuingCountry,
                    Nationality = passportDocument.Nationality
                };

                var isExpirationDateValid = passportInfo.ExpirationDate.HasValue && passportInfo.ExpirationDate < DateTime.Now;
                onboarding.SetPassportState(!isExpirationDateValid ? PassportValidationState.PassportExpired : PassportValidationState.Valid, passportInfo);
            }
            else
            {
                onboarding.SetPassportState(PassportValidationState.MrzNotRecognized);
            }

            await onboardingRepository.Update(onboarding);
        }

        private static IPassportParser CreatePassportParser()
        {
            // Passport parser uses ABBYY Cloud OCR to obtain credentials
            // register here for a trial account: https://cloud.ocrsdk.com/Account/Register
            var applicationId = Environment.GetEnvironmentVariable("CloudOcrApplicationId");
            var password = Environment.GetEnvironmentVariable("CloudOcrPassword");

            var passportParser = new PassportParser(applicationId, password);
            return passportParser;
        }
    }
}
