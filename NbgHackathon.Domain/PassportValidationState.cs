using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbgHackathon.Domain
{
    public enum PassportValidationState
    {
        None,        
        MrzNotRecognized,
        PassportExpired,
        PhotoNotLocated,
        Valid
    }
}
