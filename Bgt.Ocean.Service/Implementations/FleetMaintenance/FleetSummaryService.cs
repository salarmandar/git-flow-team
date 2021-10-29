using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Models.BaseModel;
using Bgt.Ocean.Models.FleetMaintenance;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Run;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Bgt.Ocean.Service.Helpers;
using Bgt.Ocean.Service.Mapping.Mappers;
using Bgt.Ocean.Service.Messagings;
using Bgt.Ocean.Service.Messagings.FleetMaintenance;
using System;

namespace Bgt.Ocean.Service.Implementations.FleetMaintenance
{
    public interface IFleetSummaryService
    {
        FleetSummaryResponse GetSummaryByOption(FleetSummaryRequest req);
    }
    public class FleetSummaryService : RequestBase, IFleetSummaryService
    {
        public readonly IMasterRunResourceRepository _masterRunResourceRepository;
        public readonly ISystemMessageRepository _systemMessageRepository;
        public readonly ISystemService _systemService;

        public FleetSummaryService(IMasterRunResourceRepository masterRunResourceRepository
            , ISystemMessageRepository systemMessageRepository
            , ISystemService systemService)
        {
            _masterRunResourceRepository = masterRunResourceRepository;
            _systemMessageRepository = systemMessageRepository;
            _systemService = systemService;
        }

        #region GET
        public FleetSummaryResponse GetSummaryByOption(FleetSummaryRequest req)
        {
            var result = new FleetSummaryResponse();
            try
            {
                result.Summary = GetFleetSummary(req);
                result.Total = result.Summary.Total;
            }
            catch (Exception ex)
            {
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
                result.SetMessageView(_systemMessageRepository.FindByMsgId(-184, this.UserLanguageGuid.GetValueOrDefault()).ConvertToMessageView());
            }
            return result;
        }

        private FleetSummaryView GetFleetSummary(FleetSummaryRequest req)
        {
            FleetSummaryView summary = new FleetSummaryView();
            var pagingBase = (PagingBase)req.Filters;
            switch (req.Filters.FleetOption)
            {
                case EnumFleetOption.Summary:
                case EnumFleetOption.Maintenance:
                    summary.MaintenanceList = _masterRunResourceRepository.GetMaintenance(req.Filters);
                    if (pagingBase != null)
                    {
                        var response = PaginationHelper.ToPagination(summary.MaintenanceList, pagingBase);
                        summary.MaintenanceList = response.Data;
                        summary.Total = response.Total;
                    }
                    break;
                case EnumFleetOption.Gasoline:
                    summary.GasolineList = _masterRunResourceRepository.GetGasolineExpense(req.Filters);
                    if (pagingBase != null)
                    {
                        var response = PaginationHelper.ToPagination(summary.GasolineList, pagingBase);
                        summary.GasolineList = response.Data;
                        summary.Total = response.Total;
                    }
                    break;
                case EnumFleetOption.Accident:
                    summary.AccidentList = _masterRunResourceRepository.GetAccident(req.Filters);
                    if (pagingBase != null)
                    {
                        var response = PaginationHelper.ToPagination(summary.AccidentList, pagingBase);
                        summary.AccidentList = response.Data;
                        summary.Total = response.Total;
                    }
                    break;
            }
            return summary;
        }
        #endregion 
    }
}