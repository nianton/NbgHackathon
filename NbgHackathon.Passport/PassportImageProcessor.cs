using Microsoft.Azure.WebJobs.Host;
using Microsoft.ProjectOxford.Face;
using NbgHackathon.Domain;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NbgHackathon.Passport
{
    public class PassportImageProcessor
    {
        private readonly IPassportParser passportParser;
        private readonly FaceServiceClient faceServiceClient;
        private readonly TraceWriter log;

        public PassportImageProcessor(IPassportParser parser, FaceServiceClient faceClient, TraceWriter log)
        {
            this.passportParser = parser;
            this.faceServiceClient = faceClient;
            this.log = log;
        }

        /// <summary>
        /// Processes the passport image for the given onboarding.
        /// </summary>
        /// <param name="passportImage"></param>
        /// <param name="onboarding"></param>
        /// <returns>The onboarding with updated its passport validation state.</returns>
        public async Task<OnboardingState> Process(Stream passportImage, OnboardingState onboarding)
        {
            using (var ms = new MemoryStream())
            {
                // We will have to process the stream twice, so we need to be able to seek back to start
                // we copy the data in a MemoryStream
                await passportImage.CopyToAsync(ms);
                ms.Seek(0, SeekOrigin.Begin);

                // 1. Detecting faces in the passport photo (one and only should be located)

                log.Info($"Requesting face detection for onboarding ({onboarding.Id}) passport...");

                // Input stream gets disposed by the client -creating a new instance
                var faces = await faceServiceClient.DetectAsync(new MemoryStream(ms.ToArray()));
                log.Info($"Face detection results for onboarding ({onboarding.Id}) passport:\r\n {JsonConvert.SerializeObject(faces)}");

                if (faces.Length == 1)
                {
                    var face = faces.Single();
                    var passportFaceId = face.FaceId.ToString();

                    // 2. Parsing passport information

                    ms.Seek(0, SeekOrigin.Begin);
                    var parseResult = await passportParser.ParsePassport(ms);
                    if (parseResult.Success())
                    {
                        var passportInfo = ExtractPassportInformation(parseResult);

                        // TODO: More checks should be performed here for the validity of the parsed information
                        var isExpirationDateValid = passportInfo.ExpirationDate.HasValue && passportInfo.ExpirationDate > DateTime.Today;
                        onboarding.SetPassportState(!isExpirationDateValid ? PassportValidationState.PassportExpired : PassportValidationState.Valid, passportInfo, passportFaceId);
                    }
                    else
                    {
                        log.Error($"Passport parsing failed with error: {parseResult.ErrorMessage} {Environment.NewLine} ExceptionInfo: {parseResult.ExceptionInfo}");
                        onboarding.SetPassportState(PassportValidationState.MrzNotRecognized, passportFaceId: passportFaceId);
                    }
                }
                else
                {
                    log.Error($"Photo could not be located on passport (Onboarding Id: {onboarding.Id})");
                    onboarding.SetPassportState(PassportValidationState.PhotoNotLocated);
                }

                return onboarding;
            }
            
           
        }

        /// <summary>
        /// Extracts the passport information from the parsed result.
        /// </summary>
        private PassportInformation ExtractPassportInformation(PassportParseResult passportParseResult)
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

            return passportInfo;           
        }
    }
}