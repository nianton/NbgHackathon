using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ProjectOxford.Face;
using NbgHackathon.Domain;

namespace NbgHackathon.Passport
{
    public static class PassportImageProcess
    {
        const string FunctionName = "PassportImageProcess";

        [FunctionName(FunctionName)]
        public static async Task Run([BlobTrigger("passports/{fileName}.{fileExtension}", Connection = "MainApplicationStorage")]Stream myBlob, string fileName, string fileExtension, TraceWriter log)
        {
            log.Info($"Executing {FunctionName} for {fileName}.{fileExtension}.");
            try
            {
                var onboardingRepository = ServiceLocator.ResolveRepository();
                if (!Guid.TryParse(fileName, out var onboardingId))
                {
                    throw new ArgumentException($"The blob's filename ({fileName}) could not be parsed as a valid onboarding session identifier");
                }

                var onboarding = await onboardingRepository.Get(onboardingId);
                if (onboarding == null)
                {
                    throw new InvalidOperationException($"No onboarding session could be located for the identifier: {onboardingId}");
                }

                var passportParser = CreatePassportParser();
                var faceClient = CreateFaceClient();

                var passportProcessor = new PassportImageProcessor(passportParser, faceClient, log);
                onboarding = await passportProcessor.Process(myBlob, onboarding);

                await onboardingRepository.Update(onboarding);
            }
            catch (Exception ex)
            {
                log.Error($"Failed processing passport image", ex);
                throw;
            }
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

        private static FaceServiceClient CreateFaceClient()
        {
            var subscriptionKey = Environment.GetEnvironmentVariable("FaceApiSubscriptionKey");
            var apiRoot = Environment.GetEnvironmentVariable("FaceApiEndpoint");

            var faceClient = new FaceServiceClient(subscriptionKey, apiRoot);
            return faceClient;
        }
    }
}
