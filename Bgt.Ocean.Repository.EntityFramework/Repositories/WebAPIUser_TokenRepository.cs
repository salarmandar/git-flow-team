using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories
{
    public interface IWebAPIUser_TokenRepository : IRepository<TblWebAPIUser_Token>
    {
        TblWebAPIUser_Token FindByTokenId(Guid tokenId);
        TblSystemApplication FindApplicationByAppKey(Guid appKey);
    }

    public class WebAPIUser_TokenRepository : Repository<OceanDbEntities, TblWebAPIUser_Token>, IWebAPIUser_TokenRepository
    {
        public WebAPIUser_TokenRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public TblSystemApplication FindApplicationByAppKey(Guid appKey)
        {
            return DbContext.TblSystemApplication.FirstOrDefault(e => e.TokenID == appKey);
        }

        public TblWebAPIUser_Token FindByTokenId(Guid tokenId)
        {
            return DbContext.TblWebAPIUser_Token.FirstOrDefault(e => e.TokenID == tokenId);
        }
    }
}
