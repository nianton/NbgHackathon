using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Autofac;
using NbgHackathon.Domain;

namespace NbgHackathon.Bot.Dialogs
{
    [Serializable]
    public class ProgressCheckDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as IMessageActivity;

            if (!string.IsNullOrEmpty(activity.Text) && activity.Text.Trim().ToLower() == "νιώσε")
            {
                var done = await IsProcessingComplete(
                    context.UserData.GetValue<OnboardingState>(Helpers.StateKey).Id);
                if (done)
                {
                    //Present Current status
                    //...
                    await context.PostAsync("Καταπληκτικά! Ρίξε μια τελευταία ματιά στα στοιχεία σου, και διάβασε τους όρους να δεις αν συμφωνείς.");
                    context.Call(new ConfirmDialog(), MessageReceivedAsync);
                }
                else
                {
                    await context.PostAsync("Τίποτα ακόμη.");
                }
            }
        }

        private async Task<bool> IsProcessingComplete(Guid uid)
        {
            IOnboardingRepository repos = Conversation.Container.Resolve<IOnboardingRepository>();

            //var persistedState = await repos.Get(uid);

            return true;
        }
    }
}