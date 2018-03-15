using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbgHackathon.Domain
{
    public partial class OnboardingState
    {
        public void SetPassportState(PassportValidationState validation, PassportInformation passportInfo = null)
        {
            PassportValidation = validation;

            var isInfoRequired = PassportValidation == PassportValidationState.PassportExpired 
                || PassportValidation == PassportValidationState.Valid;

            if (isInfoRequired && passportInfo == null)
                throw new InvalidOperationException("PassportInfo is required");

            PassportInfo = passportInfo;
            UpdatedAt = DateTimeOffset.Now;
        }
    }
}
