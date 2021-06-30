using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Prem.N3.PayerActivation.Helper
{
    public interface ICosmosDbRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetItemsAsync(string queryString);

        Task<ItemResponse<T>> CreateItemAsync(T item, string partitionKey);

        Task<ItemResponse<T>> DeleteItemAsync(string id, string partitionKey);

        Task<ItemResponse<T>> UpdateItemAsync(string id, T item);
    }
}
