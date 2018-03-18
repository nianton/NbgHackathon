using Abbyy.CloudOcrSdk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreadTask = System.Threading.Tasks.Task;

namespace NbgHackathon.Passport
{
    public class PassportParser : IPassportParser
    {
        private readonly RestServiceClient ocrClient;

        public PassportParser(string applicationId, string password)
        {
            ocrClient = new RestServiceClient()
            {
                ApplicationId = applicationId,
                Password = password,
                ServerUrl = "http://cloud.ocrsdk.com"
            };
        }

        public async Task<PassportParseResult> ParsePassport(Stream passportImage)
        {
            var result = new PassportParseResult();
            try
            {
                // TODO: Reusing last task's result (if exists) to spare executions during dev -remove in deployed
                var task = ocrClient.ListTasks().LastOrDefault() ?? ocrClient.ProcessMrz(passportImage);
                result.TaskId = task.Id.ToString();
                while (task.IsTaskActive())
                {
                    await ThreadTask.Delay(1000);
                    task = ocrClient.GetTaskStatus(task.Id);
                }

                /// Sample response file content: /SampleResponse/OcrMrzTaskDocument.xml
                var documentUrl = task.DownloadUrls.Single();
                result.PassportDocument = PassportDocumentInfo.CreateFromDocumentUrl(documentUrl);
            }
            catch (Exception ex)
            {
                result.SetError(ex);
            }

            return result;
        }
    }
}