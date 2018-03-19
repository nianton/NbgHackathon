using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using NbgHackathon.Domain;
using System;
using System.Threading.Tasks;

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
                var similarFaces = await faceClient.FindSimilarAsync(
                    Guid.Parse(onboarding.PassportFaceId), 
                    new[] { Guid.Parse(onboarding.SelfieFaceId) });

                var validationState = GetValidationState(similarFaces);
                onboarding.SetImageComparisonState(validationState);

                // Persist state change
                await onboardingRepository.Update(onboarding);
            }
            catch (Exception ex)
            {
                log.Error($"Failed processing image comparison", ex);
                throw;
            }
        }

        private static FaceComparisonValidationState GetValidationState(SimilarFace[] similarFaces)
        {
            return similarFaces.Length == 0
                ? FaceComparisonValidationState.NotMatched
                : similarFaces[0].Confidence > 0.8
                    ? FaceComparisonValidationState.Valid
                    : similarFaces[0].Confidence > 0.5
                        ? FaceComparisonValidationState.PotentialMatch
                        : FaceComparisonValidationState.NotMatched;
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
