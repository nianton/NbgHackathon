using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using NbgHackathon.Domain;

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
                string contentType = message.Attachments.First().ContentType;

                if (Helpers.IsValidImage(image, contentType))
                {
                    var persistedState = context.UserData.GetValue<OnboardingState>(Helpers.StateKey);

                    var passportpath = await Helpers.StoreImage(persistedState.Id, image, contentType, Helpers.Passport);

                    await context.PostAsync($"Τέλεια, το πήρα! \n Τώρα πάρε ένα {GetRandomEmotion(context)} και πάμε να βγάλουμε Seeelfieee!");
                    context.Call(new SelfieCaptureDialog(), MessageReceivedAsync);
                }
                else
                {
                    await context.PostAsync("Ωπα, κάτι πήγε στραβά. Ανέβασε ή τράβα ξανα φωτογραφία το διαβατήριο σου!");
                }
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