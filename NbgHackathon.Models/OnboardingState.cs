using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbgHackathon.Models
{
    public partial class OnboardingState
    {
        private OnboardingState()
        { }

        public OnboardingState(string email)
        {
            Id = Guid.NewGuid();
            UserEmail = email;
            CreatedAt = UpdatedAt = DateTimeOffset.Now;
        }

        public Guid Id { get; private set; }
        public string ETag { get; private set; }
        public string UserEmail { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public DateTimeOffset UpdatedAt { get; private set; }
        public PassportInformation PassportInfo { get; private set; }
        public PassportValidationState PassportValidation { get; private set; }
        public EmotionValidationState EmotionValidation { get; private set; }
        public EmotionScores EmotionScores { get; private set; }        

        internal static OnboardingState Create(DynamicTableEntity entity)
        {
            var model = new OnboardingState()
            {
                ETag = entity.ETag,
                Id = entity.Properties[nameof(Id)].GuidValue.Value,
                UserEmail = entity.Properties[nameof(UserEmail)].StringValue,
                CreatedAt = entity.Properties[nameof(CreatedAt)].DateTimeOffsetValue.Value,
                UpdatedAt = entity.Properties[nameof(UpdatedAt)].DateTimeOffsetValue.Value,
                PassportValidation = entity.Properties.ReadEnum<PassportValidationState>(nameof(PassportValidation)).GetValueOrDefault(),
                PassportInfo = entity.Properties.ReadObject<PassportInformation>(nameof(PassportInformation)),
                EmotionValidation = entity.Properties.ReadEnum<EmotionValidationState>(nameof(EmotionValidation)).GetValueOrDefault(),
                EmotionScores = entity.Properties.ReadObject<EmotionScores>(nameof(PassportInformation))
            };

            return model;
        }
    }
}