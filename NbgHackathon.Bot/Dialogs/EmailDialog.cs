using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Autofac;
using NbgHackathon.Domain;

namespace NbgHackathon.Bot.Dialogs
{
    [Serializable]
    public class EmailDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as IMessageActivity;

            if ((activity.Text != null) && (activity.Text.Trim().Length > 0) && IsValidEmail(activity.Text))
            {
                //Persist session using Domain Model
                var repo = Conversation.Container.Resolve<IOnboardingRepository>();
                var persistantState = await repo.GetOrCreate(activity.Text, "");
                context.UserData.SetValue(Helpers.StateKey, persistantState);
                context.Call(new LegalEnityDialog(), MessageReceivedAsync);
            }
            else
            {
                await context.PostAsync($"Χωρίς το Email σου δεν μπορούμε να συνεχίσουμε :-(");
                context.Wait(MessageReceivedAsync);
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}