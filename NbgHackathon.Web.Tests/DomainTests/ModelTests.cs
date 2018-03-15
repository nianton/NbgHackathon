using Microsoft.VisualStudio.TestTools.UnitTesting;
using NbgHackathon.Domain;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbgHackathon.Web.Tests.Domains
{
    [TestClass]
    public class ModelTests
    {
        private readonly IOnboardingRepository repository;

        public ModelTests()
        {
            var connectionString = ConfigurationManager.AppSettings["MainApplicationStorage"];
            repository = ServiceLocator.ResolveRepository(connectionString);
        }

        [TestMethod]
        public void TestGetOrCreateWithNewEmail()
        {
            var model = repository.GetOrCreate($"test-{Guid.NewGuid():N}@nbg.gr").Result;
            Assert.IsNotNull(model);
        }

        [TestMethod]
        public void TestCreateAndUpdate()
        {
            var model = repository.GetOrCreate($"nianton@gmail.com").Result;
            model.SetPassportState(PassportValidationState.Valid, new PassportInformation
            {
                DateOfBirth = new DateTime(1900, 10, 12),
                ExpirationDate = new DateTime(2020, 10, 8),
                Firstname = "Nikolaos",
                Surname = "Antoniou",
                Nationality = "GRC",
                PassportNumber = "P1230312093"
            });

            var updatedModel = repository.Update(model).Result;
            Assert.IsNotNull(model);
        }
    }
}