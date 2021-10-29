using System.Transactions;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Bgt.Ocean.Infrastructure.Configuration
{
    public interface IUnitOfWork<TDbContext> 
        where TDbContext : DbContext
    {
        TransactionScope BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified);
        void Commit();
        Task CommitAsync();
        string GetConnectionString();

        /// <summary>
        /// For more speed when Insert many data set to Enable = false before.
        /// </summary>
        /// <param name="isEnable"></param>
        void ConfigAutoDetectChanges(bool isEnable);
    }
}
