using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace NbgHackathon.Bot.Dialogs
{
    [Serializable]
    public class ExitDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Post("Γειά σου για τώρα.", MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var message = await result;

            if (message is string)
            {
                await context.PostAsync((string)message);
                context.Wait(MessageReceivedAsync);
            }
            else
            {
                var retryMessage = message as IMessageActivity;
                if ((retryMessage.Text != null) && (retryMessage.Text.Trim().Length > 0) && retryMessage.Text.ToLower() == "πάμε πάλι")
                {
                    context.Reset();
                }
                else
                {
                    context.Wait(MessageReceivedAsync);
                }
            }
        }
    }
}