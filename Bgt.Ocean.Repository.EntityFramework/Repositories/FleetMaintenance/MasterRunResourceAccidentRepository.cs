using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Infrastructure.Storages;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.FleetMaintenance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.FleetMaintenance
{
    public interface IMasterRunResourceAccidentRepository : IRepository<TblMasterRunResource_Accident>
    {
        IEnumerable<AccidentInfoView> GetRunResourceAccidentInfo(AccidentInfoViewRequest request);
    }
    public class MasterRunResourceAccidentRepository : Repository<OceanDbEntities, TblMasterRunResource_Accident>, IMasterRunResourceAccidentRepository
    {
        public MasterRunResourceAccidentRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }     

        public IEnumerable<AccidentInfoView> GetRunResourceAccidentInfo(AccidentInfoViewRequest request)
        {
            var dateFormat = ApiSession.UserFormatDate;
            var languageGuid = ApiSession.UserLanguage_Guid;

            DateTime DateFrom = request.FromDate.ChangeFromStringToDate(dateFormat).GetValueOrDefault();
            DateTime DateTo = request.ToDate.ChangeFromStringToDate(dateFormat).GetValueOrDefault();

            var CounterPartynoSpace = request.CounterParty.Replace(" ", "");

            Expression<Func<TblMasterRunResource_Accident, bool>> where_condition = o
               => o.MasterSite_Guid == request.SiteGuid
                && o.MasterRunResource_Guid == request.RunResourceGuid
                // selected date
                && (o.DateOfAccident >= DateFrom && o.DateOfAccident <= DateTo)
                && (o.FlagBrinksIsFault == request.FlagBrinksIsFault)
                && (request.FlagShowAll || o.FlagDisable == false)
                && (string.IsNullOrEmpty(request.CounterParty) || (o.Parties_FirstName + o.Parties_MiddleName + o.Parties_LastName).Contains(CounterPartynoSpace));

            var result = (from A in DbContext.TblMasterRunResource_Accident.Where(where_condition).Take(request.MaxRow)
                          join R in DbContext.TblMasterRunResource on A.MasterRunResource_Guid equals R.Guid
                          join RB in DbContext.TblMasterRunResourceBrand.Where(e => !request.RunResourceBrandGuid.HasValue || e.Guid == request.RunResourceBrandGuid) on A.Parties_RunResourceBrand_Guid equals RB.Guid into RBL from subRB in RBL.DefaultIfEmpty()
                          join RT in DbContext.TblMasterRunResourceType.Where(e => !request.RunResourceTypeGuid.HasValue || e.Guid == request.RunResourceTypeGuid) on A.Parties_RunResourceType_Guid equals RT.Guid into RTL from subRT in RTL.DefaultIfEmpty()
                          join E in DbContext.TblMasterEmployee on A.BrinksEmployee_Guid equals E.Guid
                          where (!request.RunResourceBrandGuid.HasValue || A.Parties_RunResourceBrand_Guid == request.RunResourceBrandGuid) &&
                                (!request.RunResourceTypeGuid.HasValue || A.Parties_RunResourceType_Guid == request.RunResourceTypeGuid)
                          select new { A, R, subRB, subRT, E }).OrderByDescending(e => e.A.DateOfAccident).ThenBy(e => e.A.FlagDisable).ThenBy(e => e.R.VehicleNumber).AsEnumerable().Select(e =>
                           new AccidentInfoView()
                           {
                               Guid = e.A.Guid,
                               DateOfAccident = e.A.DateOfAccident.AddHours(double.Parse(e.A.TimeOfAccident.Split(':')[0])).AddMinutes(double.Parse(e.A.TimeOfAccident.Split(':')[1])),
                               TimeOfAccident = DateTime.Parse(e.A.TimeOfAccident),
                               BrinksDriver = string.IsNullOrEmpty(e.E.MiddleName)? e.E.FirstName + " " + e.E.LastName : e.E.FirstName + " " + e.E.MiddleName + " " + e.E.LastName,
                               CounterParty = (e.A.Parties_FirstName + " " + e.A.Parties_MiddleName + " " + e.A.Parties_LastName),
                               Parties_DriverLicenseID = e.A.Parties_DriverLicenseID,
                               MasterRunResourceBrandName = e.subRB?.MasterRunResourceBrandName?? "" ,
                               MasterRunResourceTypeName = e.subRT?.MasterRunResourceTypeName?? "" ,
                               Parties_RunResourceModel = e.R.VehicleNumber,
                               FlagDisable = e.A.FlagDisable ?? false,
                               DatetimeCreated = e.A.DatetimeCreated,
                               DatetimeModified = e.A.DatetimeModified,
                               UserCreated = e.A.UserCreated,
                               UserModifed = e.A.UserModifed
                           });

            return result;


        }
    }
}
