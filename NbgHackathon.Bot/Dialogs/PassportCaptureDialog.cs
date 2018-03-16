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

        internal async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var message = await result as IMessageActivity;

            var image = await Helpers.GetImage(message);

            if (image == null)
            {
                await context.PostAsync("Ωπα, κάτι πήγε στραβά. Ανέβασε ή τράβα ξανα φωτογραφία το διαβατήριο σου!");
                context.Wait(MessageReceivedAsync);
            }
            else
            {
                //await context.PostAsync($"Attachment of {attachment.ContentType} type and size of {contentLenghtBytes} bytes received.");

                if (Helpers.IsValidImage(image.Item1, image.Item2.MediaType))
                {
                    await Helpers.StoreImage(image.Item1, image.Item2.MediaType, Helpers.Passport);

                    context.Call(new SelfieCaptureDialog(), this.MessageReceivedAsync);
                }

                await context.PostAsync($"Τέλεια, το πήρα! \n Τώρα πάρε ένα {GetRandomEmotion(context)} και πάμε να βγάλουμε Seeelfieee!");
                context.Call(new SelfieCaptureDialog(), MessageReceivedAsync);
            }
        }

        private string GetRandomEmotion(IDialogContext context)
        {
            int randomIndex = new Random((int)DateTime.Now.Ticks).Next(0, Helpers.Emotions.Count - 1);

            context.UserData.SetValue(Helpers.EmotionKey, Helpers.Emotions.ElementAt(randomIndex).Key);

            return Helpers.Emotions.ElementAt(randomIndex).Value;
        }
    }
}