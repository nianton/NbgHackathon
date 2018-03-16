using Microsoft.VisualStudio.TestTools.UnitTesting;
using NbgHackathon.Domain;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbgHackathon.Web.Tests.Domains
{
    [TestClass]
    public class ModelTests
    {
        private readonly string connectionString;
        private readonly IOnboardingRepository repository;

        public ModelTests()
        {
            connectionString = ConfigurationManager.AppSettings["MainApplicationStorage"];
            repository = ServiceLocator.ResolveRepository(connectionString);
        }

        [TestMethod]
        public void TestGetOrCreateWithNewEmail()
        {
            var model = repository.GetOrCreate($"test-{Guid.NewGuid():N}@nbg.gr", Guid.NewGuid().ToString()).Result;
            Assert.IsNotNull(model);
        }

        [TestMethod]
        public void TestCreateAndUpdate()
        {
            var model = repository.GetOrCreate($"nianton@gmail.com", Guid.NewGuid().ToString()).Result;
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

        [TestMethod]
        public void TestImageUpload()
        {
            var uri = repository.UploadSelfie(Guid.NewGuid(), "image/jpg", File.OpenRead("images/passport.jpg")).Result;
            Assert.IsNotNull(uri);
        }

        //public void TestStorageConnection()
        //{
        //    var storageAccount = CloudStorageAccount
        //}
    }
}