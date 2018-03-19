using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ProjectOxford.Face;
using NbgHackathon.Domain;

namespace NbgHackathon.Emotion
{
    public static class SelfieImageProcess
    {
        const string FunctionName = "SelfieImageProcess";
        const float MinimumThreshold = 0.7f;

        [FunctionName(FunctionName)]
        public static async Task Run([BlobTrigger("selfies/{fileName}.{extension}", Connection = "MainApplicationStorage")]Stream myBlob, string fileName, string fileExtension, TraceWriter log)
        {
            log.Info($"Executing {FunctionName} for {fileName}.{fileExtension} (size: {myBlob.Length}).");
            try
            {
                var onboardingRepository = ServiceLocator.ResolveRepository();
                if (!Guid.TryParse(fileName, out var onboardingId))
                {
                    throw new ArgumentException($"The selfie's filename ({fileName}) could not be parsed as a valid onboarding session identifier");
                }

                var onboarding = await onboardingRepository.Get(onboardingId);
                if (onboarding == null)
                {
                    throw new InvalidOperationException($"No onboarding session could be located for the identifier: {onboardingId}");
                }

                if (onboarding.RequestedEmotion == null)
                {
                    throw new InvalidOperationException($"No request emotion has been set on the onboarding state");
                }

                var faceClient = CreateFaceClient();
                var faces = await faceClient.DetectAsync(myBlob, returnFaceAttributes: new[] { FaceAttributeType.Emotion });

                if (faces.Length == 0)
                {
                    onboarding.SetEmotionResult(EmotionValidationState.FaceNotFound);
                }
                else if (faces.Length > 1)
                {
                    onboarding.SetEmotionResult(EmotionValidationState.MultipleFacesDetected);
                }
                else
                {
                    var face = faces[0];
                    var emotionResults = face.FaceAttributes.Emotion;
                    var emotions = emotionResults.ToRankedList().ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                    var emotionScores = new EmotionScores
                    {
                        Anger = emotionResults.Anger,
                        Contempt = emotionResults.Contempt,
                        Disgust = emotionResults.Disgust,
                        Fear = emotionResults.Fear,
                        Happiness = emotionResults.Happiness,
                        Neutral = emotionResults.Neutral,
                        Sadness = emotionResults.Sadness,
                        Surprise = emotionResults.Surprise
                    };

                    var requestedEmotionScore = emotions[$"{onboarding.RequestedEmotion}"];
                    var isValid = requestedEmotionScore > MinimumThreshold;
                    var validationState = isValid ? EmotionValidationState.Valid : EmotionValidationState.EmotionNotMatched;

                    onboarding.SetEmotionResult(validationState, emotionScores, face.FaceId.ToString());
                }

                await onboardingRepository.Update(onboarding);
            }
            catch (Exception ex)
            {
                log.Error($"Failed processing passport image", ex);
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
