using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace NbgHackathon.Bot.Dialogs
{
    [Serializable]
    public class LegalEnityDialog : IDialog<object>
    {
        private const string IndividualOption = "Φυσικό Πρόσωπο";
        private const string CompanyOption = "Νομικό Πρόσωπο";

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
                    IndividualOption,
                    CompanyOption
                }, "Ωραία, ας ξεκινήσουμε.Δουλεύεις μόνος σου ή είσαι από εταιρία;", "Λάθος επιλογή, προσπάθησε ξανά.", 3);
        }

        private async Task OnOptionSelected(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                var option = await result;

                switch (option as string)
                {
                    case IndividualOption:
                        context.Call(new PassportCaptureDialog(), null);
                        break;

                    case CompanyOption:
                        await context.PostAsync(
                            $"{context.UserData.GetValue<string>(Helpers.UserNameKey)}, θα σε ενημερώσουμε με e-mail για το τι χρειαζόμαστε για να έρθεις στη παρέα μας.");
                        //Send email to admin with name and email
                        context.Call(new ExitDialog(), null);
                        break;
                }
            }
            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync($"Ooops! Πάρα πολλές προσπάθειες :(. Μή φοβάσαι όμως, μπορώ να το διαχειριστώ. Προσπάθησε ξανά!");

                this.ShowOptions(context);
            }
        }
    }
}