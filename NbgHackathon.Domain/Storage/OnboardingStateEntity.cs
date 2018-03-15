using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbgHackathon.Domain.Storage
{
    internal class OnboardingStateEntity : TableEntity
    {
        public Guid Id { get; set; }
        public string UserEmail { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public PassportInformation PassportInfo { get; set; }
        public PassportValidationState PassportValidation { get; set; }
        public EmotionValidationState EmotionValidation { get; set; }
        public EmotionScores EmotionScores { get; set; }

        public override void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            base.ReadEntity(properties, operationContext);
        }

        public override IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            return base.WriteEntity(operationContext);
        }
    }
}
