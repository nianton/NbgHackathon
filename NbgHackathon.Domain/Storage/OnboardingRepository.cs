using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbgHackathon.Domain
{
    internal class OnboardingRepository : IOnboardingRepository
    {
        private static bool isInitialized;
        private readonly CloudTable table;

        public OnboardingRepository(string storageConnectionString)
        {
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            table = tableClient.GetTableReference(Constants.StorageTableName);
            EnsureInitialization();
        }

        public async Task Delete(OnboardingState state)
        {
            var entity = ToEntity(state);
            var deleteOperation = TableOperation.Delete(entity);
            await table.ExecuteAsync(deleteOperation);
        }

        public async Task<OnboardingState> Get(Guid id)
        {
            var retrieveOperation = TableOperation.Retrieve(GetPartitionKey(id), GetRowKey(id));
            var result = await table.ExecuteAsync(retrieveOperation);
            var entity = (DynamicTableEntity)result.Result;
            return ToModel(entity);
        }

        public async Task<OnboardingState> GetOrCreate(string email, string botSessionId)
        {
            var query = new TableQuery()
                .Where(TableQuery.GenerateFilterCondition(nameof(OnboardingState.UserEmail), QueryComparisons.Equal, email));
            var result = await table.ExecuteQuerySegmentedAsync(query, null);
            var existingEntity = result.SingleOrDefault();
            if (existingEntity != null)
            {
                return ToModel(existingEntity);
            }

            var model = new OnboardingState(email, botSessionId);
            var entity = ToEntity(model);
            var insertOperation = TableOperation.Insert(entity);
            var insertResult = await table.ExecuteAsync(insertOperation);
            entity = (DynamicTableEntity)insertResult.Result;
            return ToModel(entity);
        }

        public async Task<IList<OnboardingState>> GetAll(int itemsPerPage = 50)
        {
            var query = new TableQuery() { TakeCount = itemsPerPage };
            var entities = await table.ExecuteQuerySegmentedAsync(query, null);

            var models = entities.ToList().ConvertAll(ToModel);
            return models;
        }

        public async Task<OnboardingState> Update(OnboardingState state)
        {
            var entity = ToEntity(state);
            var updateOperation = TableOperation.Replace(entity);
            var result = await table.ExecuteAsync(updateOperation);
            var updatedEntity = (DynamicTableEntity)result.Result;
            return ToModel(updatedEntity);
        }     

        private void EnsureInitialization()
        {
            if (!isInitialized)
            {
                table.CreateIfNotExists();
                isInitialized = true;
            }
        }

        private static string GetPartitionKey(Guid id)
        {
            return id.ToString("N").Substring(0, 3);
        }

        private static string GetRowKey(Guid id)
        {
            return id.ToString("N");
        }

        private static DynamicTableEntity ToEntity(OnboardingState model)
        {
            var properties = TableEntity.Flatten(model, null);
            var entity = new DynamicTableEntity(
                GetPartitionKey(model.Id), GetRowKey(model.Id), model.ETag, properties);
            return entity;
        }

        private static OnboardingState ToModel(DynamicTableEntity entity)
        {
            return OnboardingState.Create(entity);
        }
    }
}
