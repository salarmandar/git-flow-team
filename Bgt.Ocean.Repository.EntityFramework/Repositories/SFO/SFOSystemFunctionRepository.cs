using Bgt.Ocean.Models;
using System;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.SFO
{
    public interface ISFOSystemFunctionRepository
    {
        DateTime? Func_CalculateTime(DateTime? sourceDateTime, int sourceTimezoneId, int targetTimezoneId);
    }

    public class SFOSystemFunctionRepository : ISFOSystemFunctionRepository
    {
        private readonly IDbFactory<OceanDbEntities> _dbContext;

        public SFOSystemFunctionRepository(
                IDbFactory<OceanDbEntities> dbContext
            )
        {
            _dbContext = dbContext;
        }

        public DateTime? Func_CalculateTime(DateTime? sourceDateTime, int sourceTimezoneId, int targetTimezoneId)
        {
            if (!sourceDateTime.HasValue || sourceDateTime.Value == DateTime.MinValue) return null;

            try
            {
                var result = _dbContext.GetCurrentDbContext.Up_OceanOnlineMVC_SFO_CalculateTime_Get(sourceDateTime, sourceTimezoneId, targetTimezoneId);
                var convertedDate = result.FirstOrDefault();

                return convertedDate;
            }
            catch (Exception err)
            {
                return null;
            }

        }
    }
}
