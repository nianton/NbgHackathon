using System.IO;
using System.Threading.Tasks;

namespace NbgHackathon.Passport
{
    public interface IPassportParser
    {
        Task<PassportParseResult> ParsePassport(Stream passportImage);
    }
}