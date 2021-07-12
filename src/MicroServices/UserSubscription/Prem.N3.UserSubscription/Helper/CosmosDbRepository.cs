using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Prem.N3.UserSubscription.Helper
{
    public class CosmosDbRepository<T> : ICosmosDbRepository<T> where T : class
    {
        #region State
            private static readonly string CollectionId = GetAttributeCosmoDbCollection<T>();
            private readonly CosmosClient _client;
            private Container _container;
            private const string CosmosDbId = "CosmosDbId";
        #endregion


        #region Constructors

        public CosmosDbRepository(CosmosClient client)
        {
            _client = client;
            _container = _client.GetContainer(Environment.GetEnvironmentVariable(CosmosDbId), CollectionId);
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> GetItemsAsync(string queryString)
        {
            List<T> results = new List<T>();
            try
            {
                var query = _container.GetItemQueryIterator<T>(new QueryDefinition(queryString));
                while (query.HasMoreResults)
                {
                    var response = await query.ReadNextAsync();

                    results.AddRange(response.ToList());
                }
            }
            catch (Exception ex)
            {

            }
            return results;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="partitionKey"></param>
        /// <returns></returns>
        public Task<ItemResponse<T>> CreateItemAsync(T item, string partitionKey)
        {
            return _container.CreateItemAsync<T>(item, new PartitionKey(partitionKey));
        }

        /// <summary>
        /// Delete item with partition key value
        /// </summary>
        /// <param name="id"></param>
        /// <param name="partitionKey"></param>
        /// <returns></returns>
        public Task<ItemResponse<T>> DeleteItemAsync(string id, string partitionKey)
        {
            return _container.DeleteItemAsync<T>(id, new PartitionKey(partitionKey));
        }

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public Task<ItemResponse<T>> UpdateItemAsync(string id, T item)
        {
            return _container.ReplaceItemAsync<T>(item, id);
        }
        
        #endregion


        #region Private Methods
        private static string GetAttributeCosmoDbCollection<U>()
        {
            var attribute = (CosmoDBContainerAttribute)Attribute.GetCustomAttribute(typeof(U), typeof(CosmoDBContainerAttribute));
            return attribute.Name;
        }

        #endregion
    }
}