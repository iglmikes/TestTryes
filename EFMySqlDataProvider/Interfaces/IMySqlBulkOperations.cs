

namespace EFMySqlDataProvider.Interfaces
{
    public interface IMySqlBulkOperations
    {
        Task BulkInsertAsync<T>(IEnumerable<T> entities) where T : class;
    }
}
