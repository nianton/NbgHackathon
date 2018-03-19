using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbgHackathon.Domain
{
    public enum FaceComparisonValidationState
    {
        None,
        NotMatched,
        PotentialMatch, // Needs human approval
        Valid
    }
}
