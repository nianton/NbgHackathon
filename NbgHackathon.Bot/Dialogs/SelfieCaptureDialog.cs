using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using NbgHackathon.Domain;

namespace NbgHackathon.Bot.Dialogs
{
    [Serializable]
    public class SelfieCaptureDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as IMessageActivity;

            var image = await Helpers.GetImage(activity);

            if (image == null)
            {
                await context.PostAsync("Ωπα, κάτι πήγε στραβά. Ανέβασε ή τράβα ξανα τη Selfie!");
                context.Wait(MessageReceivedAsync);
            }
            else
            {
                string contentType = activity.Attachments.First().ContentType;

                if (Helpers.IsValidImage(image, contentType))
                {
                    var persistedState = context.UserData.GetValue<OnboardingState>(Helpers.StateKey);

                    var selfiePath = await Helpers.StoreImage(persistedState.Id, image, contentType, Helpers.Selfie);

                    await context.PostAsync($"Πολύ ωραία. Δώσε μου λίγο χρόνο να τα επεξεργαστώ. \n Πληκτρολόγισε νιώσε για να δεις τη τρέχουσα κατάσταση.");
                    context.Call(new ProgressCheckDialog(), MessageReceivedAsync);
                }
                else
                {
                    await context.PostAsync("Ωπα, κάτι πήγε στραβά. Ανέβασε ή τράβα ξανα τη Selfie!");
                }
            }
        }
    }
}