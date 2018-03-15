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
        public OnboardingState()
        {

        }

        public Guid Id { get; private set; }
        protected string ETag { get; private set; }
        public string UserId { get; private set; }
        public string UserEmail { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public DateTimeOffset UpdatedAt { get; private set; }
        public PassportInformation PassportInfo { get; private set; }
        public PassportInformation PassportValidation { get; private set; }
        
        public void SetImageComparisonState()
        {

        }

        public void SetImageEmotionState()
        {

        }

        internal static OnboardingState Create(DynamicTableEntity entity)
        {
            return new OnboardingState();
        }
    }
}