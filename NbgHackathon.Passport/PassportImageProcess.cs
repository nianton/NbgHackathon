using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace NbgHackathon.Passport
{
    public static class PassportImageProcess
    {
        const string FunctioName = "PassportImageProcess";

        [FunctionName(FunctioName)]
        public static void Run([BlobTrigger("passports/{fileName}.{fileExtension}", Connection = "MainApplicationStorage")]Stream myBlob, string fileName, string fileExtension, TraceWriter log)
        {
            log.Info($"C# Blob trigger function Processed blob\n Name:{fileName}.{fileExtension} \n Size: {myBlob.Length} Bytes");

        }
    }
}
