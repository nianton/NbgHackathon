using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ProjectOxford.Face;
using NbgHackathon.Domain;

namespace NbgHackathon.ImageCompare
{
    public static class ImageComparisonFunction
    {
        const string FunctionName = "ImageComparisonFunction";

        [FunctionName(FunctionName)]
        public static async Task Run([QueueTrigger("imagecomparison-items", Connection = "MainApplicationStorage")]string myQueueItem, TraceWriter log)
        {
            log.Info($"{FunctionName} Queue trigger function processed: {myQueueItem}");

            try
            {
                var onboardingRepository = ServiceLocator.ResolveRepository();
                if (!Guid.TryParse(myQueueItem, out var onboardingId))
                {
                    throw new ArgumentException($"The queue item ({myQueueItem}) could not be parsed as a valid onboarding session identifier");
                }

                var onboarding = await onboardingRepository.Get(onboardingId);
                if (onboarding == null)
                {
                    throw new InvalidOperationException($"No onboarding session could be located for the identifier: {onboardingId}");
                }

                var faceClient = CreateFaceClient();

                // TODO: Process queue item to compare similarity of selfie and passport image.
                throw new NotImplementedException();
                //await onboardingRepository.Update(onboarding);
            }
            catch (Exception ex)
            {
                log.Error($"Failed processing image comparison", ex);
                throw;
            }
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
