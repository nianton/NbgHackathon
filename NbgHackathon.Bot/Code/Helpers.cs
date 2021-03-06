﻿using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Autofac;
using Microsoft.Bot.Builder.Dialogs;
using NbgHackathon.Domain;

namespace NbgHackathon.Bot
{
    public static class Helpers
    {
        public const string Passport = "Passport";
        public const string Selfie = "Selfie";
        public const string UserNameKey = "UserNameKey";
        public const string StateKey = "StateKey";
        public const string EmotionKey = "EmotionKey";

        public static Dictionary<string, string> Emotions = new Dictionary<string, string>()
        {
            {"Angry", "Θυμωμένο ύφος" },
            {"Happy", "Χαμόγελο" },
            {"Sad", "Λυπημένο ύφος" }
        };

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

                    var data = await responseMessage.Content.ReadAsStreamAsync();
                    var contentType = responseMessage.Content.Headers.ContentType;
                    return data;
                }
            }

            return null;
        }

        public static bool IsValidImage(Stream data, string contentType)
        {
            return Conversation.Container.Resolve<IOnboardingRepository>().IsAcceptedContentType(contentType);
        }

        public static async Task<string> StoreImage(Guid userState, Stream data, string contentType, string container)
        {
            var repository = Conversation.Container.Resolve<IOnboardingRepository>();

            string path = null;
            switch (container)
            {
                case Passport:
                    path = await repository.UploadPassport(userState, contentType, data);
                    break;
                case Selfie:
                    path = await repository.UploadSelfie(userState, contentType, data);
                    break;
                default:
                    throw new Exception("Κάποιο λάθος συνέβη!");
            }

            return path;
        }
    }
}