using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using static Bgt.Ocean.Infrastructure.Util.EnumOTC;
using static Bgt.Ocean.Infrastructure.Util.EnumRoute;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Job
{
    #region Interface

    public interface IMasterActualJobHeaderOTCRepository : IRepository<TblMasterActualJobHeader_OTC>
    {
        IEnumerable<TblMasterActualJobHeader_OTC> FindByJob(Guid jobGuid);
        IEnumerable<TblMasterActualJobHeader_OTC> GetOTCLegsByJobGuids(IEnumerable<Guid> jobGuids);
    }

    #endregion

    public class MasterActualJobHeaderOTCRepository : Repository<OceanDbEntities, TblMasterActualJobHeader_OTC>, IMasterActualJobHeaderOTCRepository
    {
        public MasterActualJobHeaderOTCRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public IEnumerable<TblMasterActualJobHeader_OTC> FindByJob(Guid jobGuid)
        {
            return DbContext.TblMasterActualJobHeader_OTC.Where(e => e.MasterActualJobHeader_Guid == jobGuid);
        }



        public IEnumerable<TblMasterActualJobHeader_OTC> GetOTCLegsByJobGuids(IEnumerable<Guid> jobGuids)
        {
            Func<string, string> GetLockType = (strLock) =>
            {
                string result = "";
                switch (strLock)
                {
                    case LockTypeID.SG: { result = LockType.SG; break; }
                    case LockTypeID.Cencon: { result = LockType.Cencon; break; }
                    case LockTypeID.SpinDial: { result = LockType.SpinDial; break; }
                    case LockTypeID.BrinksBox: { result = LockType.BrinksBox; break; }
                }
                return result;
            };
            Func<string, string, string, string, string> GetLockSerailNumber = (strLock, serailNumber, referenceCode, machineLockID) =>
             {
                 string result = "";
                 switch (strLock)
                 {
                     case LockTypeID.SG: { result = machineLockID; break; }
                     case LockTypeID.Cencon: { result = serailNumber; break; }
                     case LockTypeID.SpinDial: { result = referenceCode; break; }
                     case LockTypeID.BrinksBox: { result = serailNumber; break; }
                 }
                 return result;
             };

            var allMachine = (from leg in DbContext.TblMasterActualJobServiceStopLegs.Where(o => jobGuids.Any(j => j == o.MasterActualJobHeader_Guid))
                              join mac in DbContext.SFOTblMasterMachine on leg.MasterCustomerLocation_Guid equals mac.Guid
                              join mac_lock in DbContext.SFOTblMasterMachine_LockType on mac.Guid equals mac_lock.SFOMasterMachine_Guid
                              join system_locktype in DbContext.SFOTblSystemLockType on mac_lock.SFOSystemLockType_Guid equals system_locktype.Guid
                              join loc in DbContext.TblMasterCustomerLocation on leg.MasterCustomerLocation_Guid equals loc.Guid
                              join cus in DbContext.TblMasterCustomer on loc.MasterCustomer_Guid equals cus.Guid
                              join site in DbContext.TblMasterSite on leg.MasterSite_Guid equals site.Guid
                              join country in DbContext.TblMasterCountry on site.MasterCountry_Guid equals country.Guid
                              where !(bool)mac_lock.FlagDisable && (bool)cus.FlagChkCustomer
                              select new { mac_lock, system_locktype, leg, site, country }).ToList();

            return allMachine.Select(mac =>
            {
                string otc_branch_name = mac.site.SiteName;
                string otc_lock_user = string.Empty;
                string otc_lock_mode = "R";

                var otc = new TblMasterActualJobHeader_OTC();
                otc.Guid = Guid.NewGuid();
                otc.MasterActualJobHeader_Guid = mac.leg.MasterActualJobHeader_Guid.GetValueOrDefault();
                otc.MasterActualJobServiceStopLegs_Guid = mac.leg.Guid;
                otc.Lock = GetLockSerailNumber(mac.system_locktype.LockTypeID, mac.mac_lock.SerailNumber, mac.mac_lock.ReferenceCode, mac.mac_lock.MachineLockID); //mc.SerailNumber,//get from 
                otc.Country = mac.country.MasterCountryAbbreviation;// get from customer
                otc.LockIndex = mac.mac_lock.LockSeq.GetValueOrDefault() - 1;
                otc.LockMode = otc_lock_mode;
                otc.LockUser = otc_lock_user;
                otc.Branch = otc_branch_name;
                otc.LockType = GetLockType(mac.system_locktype.LockTypeID);
                //otc.MachineType = o.OtcTypeId,
                otc.CombinationCode = mac.system_locktype.LockTypeID.Equals(LockTypeID.SpinDial) ? mac.mac_lock.CombinationCode : "";
                return otc;
            });
        }
    }
}
