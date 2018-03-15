﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbgHackathon.Domain
{
    public partial class OnboardingState
    {
        public void SetRequestedEmotion(FaceEmotion emotion)
        {
            RequestedEmotion = emotion;
            EmotionValidation = EmotionValidationState.None;
            EmotionScores = null;
            UpdatedAt = DateTimeOffset.Now;
        }

        public void SetEmotionResult(EmotionValidationState validation, EmotionScores scores)
        {
            if (RequestedEmotion == null)
                throw new InvalidOperationException("Requested score has not been set.");

            EmotionScores = scores;
            EmotionValidation = validation;
        }
    }
}