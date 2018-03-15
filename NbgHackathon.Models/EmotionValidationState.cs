using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbgHackathon.Models
{
    public enum EmotionValidationState
    {
        None,        
        FaceNotFound,
        EmotionNotMatched,
        Valid
    }
}
