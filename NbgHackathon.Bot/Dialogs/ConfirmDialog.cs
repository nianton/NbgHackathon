using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace NbgHackathon.Bot.Dialogs
{
    [Serializable]
    public class ConfirmDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            this.ShowOptions(context);

            return Task.CompletedTask;
        }

        private void ShowOptions(IDialogContext context)
        {
            PromptDialog.Choice(context,
                this.OnOptionSelected,
                new List<string>()
                {
                    "Ναι",
                    "Όχι"
                }, "Συμφωνείς?", "Κρίμα! L Σε περιμένουμε την επόμενη φορά!", 3);
        }

        private async Task OnOptionSelected(IDialogContext context, IAwaitable<object> result)
        {
            var response = await result as string;

            switch (response)
            {
                case "Ναι":
                    context.Call(new WelcomeAboardDialog(), MessageReceivedAsync);
                    break;
                case "Όχι":
                    break;
            }
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as IMessageActivity;

            // TODO: Put logic for handling user message here
            

            context.Wait(MessageReceivedAsync);
        }
    }
}