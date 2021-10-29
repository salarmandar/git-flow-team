using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.ServiceRequest;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Configuration;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.SFO
{
    #region interface

    public interface ISFOTransactionServiceRequestRepository : IRepository<SFOTblTransactionServiceRequest>
    {
        List<QueryServiceRequestView> Func_SFO_GetServiceRequestList(QueryServiceRequestView_Request request);
        Up_OceanOnlineMVC_SFO_GetServiceRequestByGuidEscalation_Result Func_SFO_GetServiceRequestByGuidEscalation(Guid serviceRequestGuid);
    }

    #endregion

    public class SFOTransactionServiceRequestRepository : Repository<OceanDbEntities, SFOTblTransactionServiceRequest>, ISFOTransactionServiceRequestRepository
    {
        public SFOTransactionServiceRequestRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
        

        public GetCreateServiceRequestDataView Func_SFO_GetCreateServiceRequestData(GetCreateServiceRequestDataView request)
        {
            var result = DbContext.Database.Connection
                .Query<GetCreateServiceRequestDataView>
                (
                    "Up_OceanOnlineMVC_API_GetPrepareCreateServiceRequest",
                    new
                    {
                        @CountryAbb = request.masterCountryAbb,
                        @MachineId = request.machineId,
                        @ServiceJobTypeAbb = request.serviceJobTypeAbb,
                        @OpenSourceId = request.openSourceId,
                        @MachineServiceTypeName = request.machineServiceTypeName,
                        @ProblemId = request.problemId
                    },
                    commandType: CommandType.StoredProcedure
                ).ToList();

            return result[0];
        }

        public List<QueryServiceRequestView> Func_SFO_GetServiceRequestList(QueryServiceRequestView_Request request)
        {
            var result = DbContext.Database.Connection
                .Query<QueryServiceRequestView>
                (
                    "Up_OceanOnlineMVC_API_GetServiceRequestList",
                    new
                    {
                        @MaxRow = WebConfigurationManager.AppSettings["MaxRow"],
                        @CountryAbb = request.countryAbb,
                        @AtmId = request.atmId,
                        @TicketNumber = request.ticketNumber,
                        @StatusId = request.statusId,
                        @CustomerReferenceNumber = request.customerReferenceNumber,
                        @CreatedUser = request.createdUser,
                        @CreatedFrom = request.createdDatetimeFrom,
                        @CreatedTo = request.createdDatetimeTo
                    },
                    commandType: CommandType.StoredProcedure
                ).ToList();

            return result;
        }

        public override void Modify(SFOTblTransactionServiceRequest entity)
        {
            if (IsAllowDuplicateStatus(entity.TicketStatus_Guid, entity.TblMasterSite.MasterCountry_Guid))
            {
                entity.DateStatus = GetDateStatus(
                        entity.SFOMasterCategory_Guid,
                        entity.TicketStatus_Guid,
                        entity.MasterCustomerLocation_Guid,
                        entity.SFOMasterMachineServiceType_Guid,
                        entity.Guid,
                        entity.DateStatus
                    );
            }

            base.Modify(entity);

        }

        private int GetDateStatus(
                Guid? masterCategoryGuid,
                Guid ticketStatusGuid,
                Guid masterCustomerLocationGuid,
                Guid? machineServiceTypeGuid,
                Guid ticketGuid,
                int dateStatus
            )
        {
            var result = DbContext.Database.Connection
                .Query<int?>("dbo.Up_OceanOnlineMVC_SFO_GetTicketDateStatus", new
                {
                    @newTicketStatusGuid = ticketStatusGuid,
                    @newMachineGuid = masterCustomerLocationGuid,
                    @newMachineServiceTypeGuid = machineServiceTypeGuid,
                    @currentTicketGuid = ticketGuid,
                    @currentDateStatus = dateStatus
                }, commandType: CommandType.StoredProcedure);
            
            var newDateStatus = result.FirstOrDefault().GetValueOrDefault();

            return newDateStatus == 0 ? dateStatus : newDateStatus;
        }

        private bool IsAllowDuplicateStatus(Guid ticketStatusGuid, Guid masterCoutryGuid)
        {

            var isConfigurationExist = DbContext.SFOTblSystemAllowServiceRequestStatusDuplicate
                .Any(e => e.MasterCountry_Guid == masterCoutryGuid && e.SystemJobStatus_Guid == ticketStatusGuid);

            return isConfigurationExist;
        }

        public Up_OceanOnlineMVC_SFO_GetServiceRequestByGuidEscalation_Result Func_SFO_GetServiceRequestByGuidEscalation(Guid serviceRequestGuid)
        {
            return DbContext.Up_OceanOnlineMVC_SFO_GetServiceRequestByGuidEscalation(serviceRequestGuid).FirstOrDefault();
        }
    }
}
