using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace NbgHackathon.Bot.Dialogs
{
    [Serializable]
    public class NameDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as IMessageActivity;

            if ((activity.Text != null) && (activity.Text.Trim().Length > 0) && IsValidName(activity.Text))
            {
                await context.PostAsync($"Γειά σου {activity.Text} είμαι η Stacey.");

                context.UserData.SetValue(Helpers.UserNameKey, activity.Text);

                await context.PostAsync($"Καλώς ήρθες στο Dev Portal!");

                await context.PostAsync($"Θα μου δώσεις το email σου?");

                context.Call(new EmailDialog(), MessageReceivedAsync);
            }
            else
            {
                context.Wait(MessageReceivedAsync);
            }
        }

        private bool IsValidName(string text)
        {
            return true;
            //TODO implement name validation logic
        }
    }
}