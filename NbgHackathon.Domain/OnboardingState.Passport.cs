using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbgHackathon.Domain
{
    public partial class OnboardingState
    {
        public void SetPassportState(PassportValidationState validation, PassportInformation passportInfo = null, string passportFaceId = null)
        {
            PassportValidation = validation;

            var isInfoRequired = PassportValidation == PassportValidationState.PassportExpired 
                || PassportValidation == PassportValidationState.Valid;

            if (isInfoRequired && passportInfo == null)
                throw new InvalidOperationException("PassportInfo is required");

            var isFaceIdRequired = PassportValidation == PassportValidationState.PassportExpired
                || PassportValidation == PassportValidationState.Valid;

            if (isFaceIdRequired && string.IsNullOrWhiteSpace(passportFaceId))
                throw new InvalidOperationException("PassportFaceId is required");

            PassportFaceId = passportFaceId;
            PassportInfo = passportInfo;
            UpdatedAt = DateTimeOffset.Now;
        }
    }
}
