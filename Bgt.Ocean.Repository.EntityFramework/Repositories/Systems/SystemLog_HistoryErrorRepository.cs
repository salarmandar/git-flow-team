using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Models;
using System;
using System.Threading.Tasks;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Systems
{
    public interface ISystemLog_HistoryErrorRepository : IRepository<TblSystemLog_HistoryError>
    {
        /// <summary>
        /// Create Log History error (WARNING!, THIS METHOD WILL RESET DBFACTORY)
        /// </summary>
        /// <param name="ex"></param>
        void CreateHistoryError(Exception ex);

        /// <summary>
        /// Create Log History error (WARNING!, THIS METHOD WILL RESET DBFACTORY)
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="pageName"></param>
        /// <param name="clientIp"></param>
        /// <param name="flagSendMail"></param>
        void CreateHistoryError(Exception ex, string pageName, string clientIp, bool flagSendMail = false);

        Task CreateAsync(Exception ex, string clientName, string pageName = "", string ipAddress = "");
    }

    public class SystemLog_HistoryErrorRepository : Repository<OceanDbEntities, TblSystemLog_HistoryError>, ISystemLog_HistoryErrorRepository
    {
        public SystemLog_HistoryErrorRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public void CreateHistoryError(Exception ex, string pageName, string clientIp, bool flagSendMail = false)
        {
            DbFactory.ForceDispose();
            // client computer name
            string clientName = SystemHelper.ClientHostName;
            Task.Run(() =>  CreateAsync(ex, clientName));
        }

        public void CreateHistoryError(Exception ex) => CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress);

        public async Task CreateAsync(Exception ex, string clientName, string pageName = "", string ipAddress = "")
        {
            try
            {
                using (var db = NewDbContext())
                {

                    TblSystemLog_HistoryError newError = new TblSystemLog_HistoryError();
                    newError.Guid = Guid.NewGuid();
                    newError.ErrorDescription = ex.Message;
                    newError.FunctionName = ex.TargetSite == null ? "" : ex.TargetSite.Name;
                    newError.PageName = pageName;
                    newError.InnerError = ex.InnerException == null ? ex.ToString() : ex.InnerException.ToString();
                    newError.ClientIP = ipAddress;
                    newError.ClientName = clientName;
                    newError.DatetimeCreated = DateTime.UtcNow;
                    newError.FlagSendEmail = false;

                    db.TblSystemLog_HistoryError.Add(newError);
                    await db.SaveChangesAsync();

                }
            }
            catch { /* no need to handle */ }

        }
    }
}
