using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace NbgHackathon.Bot.Dialogs
{
    [Serializable]
    public class PassportCaptureDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var message = await result as IMessageActivity;

            if (message.Attachments != null && message.Attachments.Any())
            {

                var image = await Helpers.GetImage(message);

                //await context.PostAsync($"Attachment of {attachment.ContentType} type and size of {contentLenghtBytes} bytes received.");

                if (Helpers.IsValidImage(image))
                {
                    await Helpers.StoreImage(image, Helpers.Passport);

                    await context.Forward<object>(new SelfieCaptureDialog(), this.MessageReceivedAsync, message);
                }

                await context.PostAsync("Η φωτογραφία δεν ήταν έγγυρη");
            }
            else
            {
                await context.PostAsync("Παρακαλώ στείλτε μου μια φωτογραφία του διαβατηρίου σας.");
            }
        }
    }
}