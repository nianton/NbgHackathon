using Microsoft.VisualStudio.TestTools.UnitTesting;
using NbgHackathon.Domain;
using System;
using System.Configuration;
using System.IO;
using System.Linq;

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
        public void TestPassportUpload()
        {
            var uri = repository.UploadSelfie(Guid.NewGuid(), "image/jpg", File.OpenRead("images/passport.jpg")).Result;
            Assert.IsNotNull(uri);
        }

        [TestMethod]
        public void TestSelfieUpload()
        {
            var uri = repository.UploadSelfie(Guid.NewGuid(), "image/jpg", File.OpenRead("images/selfie.jpg")).Result;
            Assert.IsNotNull(uri);
        }

        [TestMethod]
        public void TestGetAll()
        {
            var itemsPerPage = 2;
            var pagedResult = repository.GetAll(itemsPerPage).Result;

            Assert.IsNotNull(pagedResult.Items.Count <= 2);

            if (pagedResult.HasMoreResults)
            {
                var pagedResult2 = repository.GetAll(itemsPerPage, pagedResult.ContinuationToken).Result;

                // Page items are as requested
                Assert.IsNotNull(pagedResult2.Items.Count <= 2);

                // 2nd page items do not exist on the 1st page's items
                Assert.IsFalse(pagedResult2.Items.Any(x => pagedResult.Items.Any(y => y.Id == x.Id)));
            }
        }
    }
}