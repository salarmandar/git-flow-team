using Bgt.Ocean.Models.ServiceRequest;
using Bgt.Ocean.Repository.EntityFramework.Repositories.SFO;
using Bgt.Ocean.Service.Messagings.ServiceRequest;
using System.Linq;

namespace Bgt.Ocean.Service.Implementations.AuditLog.ServiceRequest
{
    public interface IServiceRequestReaderService
    {
        ResponseQueryServiceRequest GetServiceRequestList(RequestQueryServiceRequest request);
    }


    public class ServiceRequestReaderService : IServiceRequestReaderService
    {
        private readonly ISFOTransactionServiceRequestRepository _transactionServiceRequestRepository;

        public ServiceRequestReaderService(
                ISFOTransactionServiceRequestRepository transactionServiceRequestRepository
            )
        {
            _transactionServiceRequestRepository = transactionServiceRequestRepository;
        }

        public ResponseQueryServiceRequest GetServiceRequestList(RequestQueryServiceRequest request)
        {
            var result = new ResponseQueryServiceRequest();
            var requestRepo = new QueryServiceRequestView_Request();

            requestRepo.countryAbb = request.countryAbb;
            requestRepo.atmId = request.atmId;
            requestRepo.ticketNumber = request.ticketNumber;
            requestRepo.statusId = request.statusId;
            requestRepo.customerReferenceNumber = request.customerReferenceNumber;
            requestRepo.createdUser = request.createdUser;
            requestRepo.createdDatetimeFrom = request.createdDatetimeFrom;
            requestRepo.createdDatetimeTo = request.createdDatetimeTo;

            var resultRepo = _transactionServiceRequestRepository.Func_SFO_GetServiceRequestList(requestRepo);

            foreach (var item in resultRepo)
            {
                var resultItem = new ResponseQueryServiceRequest_Main();

                resultItem.serviceRequestDetail.closeSource = item.closeSource;
                resultItem.serviceRequestDetail.countryAbb = item.countryAbb;
                resultItem.serviceRequestDetail.createdUser = item.createdUser;
                resultItem.serviceRequestDetail.guid = item.guid;
                resultItem.serviceRequestDetail.machineServiceType = item.machineServiceType;
                resultItem.serviceRequestDetail.modifiedUser = item.modifiedUser;
                resultItem.serviceRequestDetail.onHold = item.onHold;
                resultItem.serviceRequestDetail.openSource = item.openSource;

                if (item.problem != null)
                {
                    var arrProblem = item.problem.Split('|');
                    if (arrProblem.Count() == 3)
                    {
                        resultItem.serviceRequestDetail.problemId = arrProblem[0];
                        resultItem.serviceRequestDetail.problemName = arrProblem[1];
                        resultItem.serviceRequestDetail.priority = arrProblem[2];
                    }
                }

                resultItem.serviceRequestDetail.reportedIncidentDescription = item.reportedIncidentDescription;
                resultItem.serviceRequestDetail.responderStatusName = item.responderStatusName;
                resultItem.serviceRequestDetail.serviceJobType = item.serviceJobType;

                if (item.solution != null)
                {
                    var arrSolution = item.solution.Split('|');
                    if (arrSolution.Count() == 2)
                    {
                        resultItem.serviceRequestDetail.solutionId = arrSolution[0];
                        resultItem.serviceRequestDetail.solutionName = arrSolution[1];
                    }
                }

                resultItem.serviceRequestDetail.statusId = item.statusId;
                resultItem.serviceRequestDetail.statusName = item.statusName;
                resultItem.serviceRequestDetail.ticketNumber = item.ticketNumber;

                resultItem.routeDetail.brinksSiteCode = item.brinksSiteCode;
                resultItem.routeDetail.brinksSiteName = item.brinksSiteName;
                resultItem.routeDetail.routeGroupDetailName = item.routeGroupDetailName;
                resultItem.routeDetail.routeGroupName = item.routeGroupName;
                resultItem.routeDetail.runNumber = item.runNumber;
                resultItem.routeDetail.technicianId = item.technicianId;
                resultItem.routeDetail.technicianName = item.technicianName;
                resultItem.routeDetail.technicianPhone = item.technicianPhone;

                resultItem.datetimeDetail.acknowlegedDatetime = item.acknowlegedDatetime;
                resultItem.datetimeDetail.closedDatetime = item.closedDatetime;
                resultItem.datetimeDetail.createdDatetime = item.createdDatetime;
                resultItem.datetimeDetail.departedToOnsiteDatetime = item.departedToOnsiteDatetime;
                resultItem.datetimeDetail.departedToOriginDatetime = item.departedToOriginDatetime;
                resultItem.datetimeDetail.dispatchedDatetime = item.dispatchedDatetime;
                resultItem.datetimeDetail.downedDatetime = item.downedDatetime;
                resultItem.datetimeDetail.dueDatetime = item.dueDatetime;
                resultItem.datetimeDetail.etaDatetime = item.etaDatetime;
                resultItem.datetimeDetail.modifiedDatetime = item.modifiedDatetime;
                resultItem.datetimeDetail.notifiedDatetime = item.notifiedDatetime;
                resultItem.datetimeDetail.openedDatetime = item.openedDatetime;
                resultItem.datetimeDetail.reportedOnsiteDatetime = item.reportedOnsiteDatetime;
                resultItem.datetimeDetail.reportedToOriginDatetime = item.reportedToOriginDatetime;
                resultItem.datetimeDetail.resolvedDatetime = item.resolvedDatetime;
                resultItem.datetimeDetail.servicedDatetime = item.servicedDatetime;

                result.result.Add(resultItem);
            }

            result.rows = result.result.Count;

            return result;
        }
    }
}
