using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbgHackathon.Domain
{
    public enum EmotionValidationState
    {
        None,        
        FaceNotFound,
        MultipleFacesDetected,
        EmotionNotMatched,
        Valid
    }
}
