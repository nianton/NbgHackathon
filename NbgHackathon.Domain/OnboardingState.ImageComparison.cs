using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbgHackathon.Domain
{
    public partial class OnboardingState
    {
        public void SetImageComparisonState(FaceComparisonValidationState validationState)
        {
            FaceValidation = validationState;
        }
    }
}