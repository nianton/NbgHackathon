using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace NbgHackathon.Bot
{
    public static class Helpers
    {
        public const string Passport = "Passport";
        public const string Selfie = "Selfie";

        public static async Task<Stream> GetImage(IMessageActivity message)
        {
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

                    return await responseMessage.Content.ReadAsStreamAsync();
                }
            }

            return null;
        }

        public static bool IsValidImage(Stream data)
        {
            return true;
        }

        public static Task StoreImage(Stream data, string container)
        {
            //Call Data Model
            throw new NotImplementedException();
        }
    }
}