using System;
using System.Configuration;

namespace NbgHackathon.Models
{
    public static class ServiceLocator
    {
        private static readonly string DefaultConnectionString = ConfigurationManager.AppSettings[Constants.DefaultConnectionStringName];

        public static IOnboardingRepository ResolveRepository()
        {
            if (string.IsNullOrWhiteSpace(DefaultConnectionString))
                throw new InvalidOperationException($"The setting '{Constants.DefaultConnectionStringName}' was not initialized.");

            return new OnboardingRepository(DefaultConnectionString);
        }

        public static IOnboardingRepository ResolveRepository(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            return new OnboardingRepository(connectionString);
        }
    }
}