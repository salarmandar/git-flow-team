using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.ActualJob.CashAddModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Job
{
    public interface IMasterActualJobHeaderCapabilityRepository : IRepository<TblMasterActualJobHeader_Capability>
    {
        IEnumerable<Capability> FindCapabilityIDByJobGuid(Guid jobGuid);
        IEnumerable<CapabilityView> GetMachineCapability(Guid machineGuid);
    }
    public class MasterActualJobHeaderCapabilityRepository : Repository<OceanDbEntities, TblMasterActualJobHeader_Capability>, IMasterActualJobHeaderCapabilityRepository
    {
        public MasterActualJobHeaderCapabilityRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public IEnumerable<Capability> FindCapabilityIDByJobGuid(Guid jobGuid)
        {
            var result = from headcap in DbContext.TblMasterActualJobHeader_Capability
                         join cap in DbContext.SFOTblSystemMachineCapability on headcap.SystemMachineCapability_Guid equals cap.Guid
                         where headcap.MasterActualJobHeader_Guid == jobGuid
                         select (Capability)cap.CapabilityID;
            return result;
        }
        public IEnumerable<CapabilityView> GetMachineCapability(Guid machineGuid)
        {
            Capability[] cappability = new Capability[] {
                Capability.NoteWithdraw ,
                Capability.SmallBagDeposit ,
                Capability.BulkNoteDeposit,
                Capability.CoinExchange,
                Capability.Recycling
            };
            var cap = DbContext.SFOTblMasterMachine_Capabilties.Where(w => w.MasterMachine_Guid == machineGuid &&
                                                                            cappability.Contains((Capability)w.SFOTblMasterMachineModelType_Capability.SFOTblSystemMachineCapability.CapabilityID))
                    .Select(s => s.SFOTblMasterMachineModelType_Capability.SFOTblSystemMachineCapability)?.Select(s => new CapabilityView
                    {
                        Guid = s.Guid,
                        CapabilityDesc = s.CapabilityDescription,
                        CapabilityId = (Capability)s.CapabilityID
                    });

            return cap;
        }
    }




}
