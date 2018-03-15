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

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            if (message.Attachments != null && message.Attachments.Any())
            {
                var attachment = message.Attachments.First();
                using (HttpClient httpClient = new HttpClient())
                {
                    // Skype & MS Teams attachment URLs are secured by a JwtToken, so we need to pass the token from our bot.
                    if ((message.ChannelId.Equals("skype", StringComparison.InvariantCultureIgnoreCase) || message.ChannelId.Equals("msteams", StringComparison.InvariantCultureIgnoreCase))
                        && new Uri(attachment.ContentUrl).Host.EndsWith("skype.com"))
                    {
                        var token = await new MicrosoftAppCredentials().GetTokenAsync();
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    }

                    var responseMessage = await httpClient.GetAsync(attachment.ContentUrl);

                    var contentLenghtBytes = responseMessage.Content.Headers.ContentLength;

                    //await context.PostAsync($"Attachment of {attachment.ContentType} type and size of {contentLenghtBytes} bytes received.");

                    if (IsValidContent(responseMessage))
                    {
                        await StorePassportImage(responseMessage);
                        await context.Forward<object>(new SelfieCaptureDialog(), null, context.MakeMessage());
                    }

                    await context.PostAsync("Η φωτογραφία δεν ήταν έγγυρη");
                }
            } 
            else
            {
                await context.PostAsync("Παρακαλώ στείλτε μου μια φωτογραφία του διαβατηρίου σας.");
            }

            context.Wait(this.MessageReceivedAsync);
        }

        private Task StorePassportImage(HttpResponseMessage responseMessage)
        {
            throw new NotImplementedException();
        }

        private bool IsValidContent(HttpResponseMessage responseMessage)
        {
            throw new NotImplementedException();
        }
    }
}