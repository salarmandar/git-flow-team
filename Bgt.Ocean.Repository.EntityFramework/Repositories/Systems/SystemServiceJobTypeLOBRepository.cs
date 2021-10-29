using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Bgt.Ocean.Infrastructure.CompareHelper;
using static Bgt.Ocean.Models.MasterRoute.MassUpdateView;
using static Bgt.Ocean.Infrastructure.Util.EnumRoute;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Systems
{
    public interface ISystemServiceJobTypeLOBRepository : IRepository<TblSystemServiceJobTypeLOB>
    {
        IEnumerable<LineOfBusinessJobTypeByFlagAdhocJobResult> Func_LineOfBusinessJobTypeByFlagAdhocJob(Guid? LanguageGuid);
        IEnumerable<DropDownServiceTypeView> GetServiceTypeByLOBs_MassUpdate(List<Guid> LobGuids, bool flagPcustomer, bool flagDcustomer, bool flagSameSite);
    }

    public class SystemServiceJobTypeLOBRepository : Repository<OceanDbEntities, TblSystemServiceJobTypeLOB>, ISystemServiceJobTypeLOBRepository
    {
        public SystemServiceJobTypeLOBRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        #region ## LOB.

        public IEnumerable<LineOfBusinessJobTypeByFlagAdhocJobResult> Func_LineOfBusinessJobTypeByFlagAdhocJob(Guid? LanguageGuid)
        {
            return DbContext.Up_OceanOnlineMVC_LineOfBusinessJobTypeByFlagAdhocJob_Get(LanguageGuid);
        }
        #endregion ## LOB.

        public IEnumerable<DropDownServiceTypeView> GetServiceTypeByLOBs_MassUpdate(List<Guid> LobGuids, bool flagPcustomer, bool flagDcustomer, bool flagSameSite)
        {
            IEnumerable<DropDownServiceTypeView> dataInterSect;
            int[] jobID = null;

            if (flagPcustomer && flagDcustomer)
            {
                jobID = flagSameSite ? new int[] { IntTypeJob.TV, IntTypeJob.T } : new int[] { IntTypeJob.TV };
            }
            else if (flagPcustomer)
            {
                jobID = flagSameSite ? new int[] { IntTypeJob.P, IntTypeJob.BCP } : new int[] { IntTypeJob.P };
            }
            else if (flagDcustomer)
            {
                jobID = flagSameSite ? new int[] { IntTypeJob.D, IntTypeJob.AC, IntTypeJob.AE, IntTypeJob.BCD } : new int[] { };
            }
            else
            {
                jobID = new int[] { };
            }

            //If LobGuids have more than 1 value, it will return intersection of service job type in LOB.
            if (LobGuids.Count > 0)
            {
                var data = (from jtLob in DbContext.TblSystemServiceJobTypeLOB
                            join lob in DbContext.TblSystemLineOfBusiness on jtLob.SystemLineOfBusinessGuid equals lob.Guid
                            join jt in DbContext.TblSystemServiceJobType on jtLob.SystemServiceJobTypeGuid equals jt.Guid
                            where LobGuids.Contains(lob.Guid) && jobID.Any(a => jt.ServiceJobTypeID == a) && jt.FlagDisable == false && lob.FlagDisable == false
                            select new DropDownServiceTypeView { ServiceTypeGuid = jt.Guid, ServiceTypeName = jt.ServiceJobTypeName }).ToList();

                //Way to find intersect records when LobGuid have more than 1 value.
                dataInterSect = data.Distinct((x, y) => x.ServiceTypeGuid == y.ServiceTypeGuid);
            }
            //If LobGuids have 1 value, it will return all of service job type in LOB.
            else
            {
                var data = (from jtLob in DbContext.TblSystemServiceJobTypeLOB
                            join lob in DbContext.TblSystemLineOfBusiness on jtLob.SystemLineOfBusinessGuid equals lob.Guid
                            join jt in DbContext.TblSystemServiceJobType on jtLob.SystemServiceJobTypeGuid equals jt.Guid
                            where LobGuids.Contains(lob.Guid) && jobID.Any(a => jt.ServiceJobTypeID == a) && jt.FlagDisable == false && lob.FlagDisable == false
                            select new DropDownServiceTypeView { ServiceTypeGuid = jt.Guid, ServiceTypeName = jt.ServiceJobTypeName }).ToList();

                dataInterSect = data;
            }

            return dataInterSect;
        }

    }
}
