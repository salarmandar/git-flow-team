
using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.Denomination;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Job;
using Bgt.Ocean.Repository.EntityFramework.Repositories.StandardTable;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Bgt.Ocean.Service.Mapping.Mappers;
using Bgt.Ocean.Service.Messagings;
using Bgt.Ocean.Service.Messagings.Denomination;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Bgt.Ocean.Service.Implementations.Denomination
{

    public interface IDenominationService
    {
        DenominationResponse GetMasterDenomination(GetDenominationRequest req);
        DenominationResponse GetLiabilityDenominationByLiabilityGuid(GetDenominationRequest req);
        Task<DenominationResponse> InsertOrUpdateDenominationAsync(SetDenominationAsyncRequest req);
    }

    public class DenominationService : RequestBase, IDenominationService
    {
        private readonly IMasterDenominationRepository _masterDenominationRepository;
        private readonly IMasterActualJobItemsLiabilityRepository _masterActualJobItemsLiabilityRepository;
        private readonly IMasterActualJobItemsLiabilityDenominationRepository _masterActualJobItemsLiabilityDenominationRepository;
        private readonly ISystemService _systemService;
        private readonly ISystemMessageRepository _systemMessageRepository;
        private readonly IBaseRequest _req;
        private readonly IUnitOfWork<OceanDbEntities> _uow;

        public DenominationService(IMasterDenominationRepository masterDenominationRepository
            , IMasterActualJobItemsLiabilityDenominationRepository masterActualJobItemsLiabilityDenominationRepository
            , IMasterActualJobItemsLiabilityRepository masterActualJobItemsLiabilityRepository
            , ISystemService systemService
            , ISystemMessageRepository systemMessageRepository
            , IBaseRequest req
            , IUnitOfWork<OceanDbEntities> uow
            )
        {
            _masterDenominationRepository = masterDenominationRepository;
            _masterActualJobItemsLiabilityDenominationRepository = masterActualJobItemsLiabilityDenominationRepository;
            _masterActualJobItemsLiabilityRepository = masterActualJobItemsLiabilityRepository;
            _systemService = systemService;
            _systemMessageRepository = systemMessageRepository;
            _req = req;
            _uow = uow;
        }
        /// <summary>
        /// => TFS#71743: Denomination => TblMasterDenomination
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public DenominationResponse GetMasterDenomination(GetDenominationRequest req)
        {
            var result = new DenominationResponse();
            try
            {
                var denoGuids = new Guid[]
                      {
                        EnumHelper.GetDescription(DenoUnit.Coin).ToGuid(),
                        EnumHelper.GetDescription(DenoUnit.BankNote).ToGuid()
                      };

                result.DenominationList = _masterDenominationRepository.GetDenominationByDenoUnit(req.CurrencyGuid, _req.Data.UserLanguageGuid, denoGuids);
            }
            catch (Exception ex)
            {
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
                result.SetMessageView(_systemMessageRepository.FindByMsgId(-184, _req.Data.UserLanguageGuid.GetValueOrDefault()).ConvertToMessageView());
            }
            return result;
        }

        public DenominationResponse GetLiabilityDenominationByLiabilityGuid(GetDenominationRequest req)
        {
            var result = new DenominationResponse();
            try
            {
                var denoGuids = new Guid[]
                   {
                        EnumHelper.GetDescription(DenoUnit.Coin).ToGuid(),
                        EnumHelper.GetDescription(DenoUnit.BankNote).ToGuid()
                   };

                var baseLiabilityDeno = _masterActualJobItemsLiabilityDenominationRepository.GetDenominationByDenoUnit(req.LiabilityGuid, _req.Data.UserLanguageGuid.GetValueOrDefault(), denoGuids);

                result.DenominationList = baseLiabilityDeno;
            }
            catch (Exception ex)
            {
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
                result.SetMessageView(_systemMessageRepository.FindByMsgId(-184, _req.Data.UserLanguageGuid.GetValueOrDefault()).ConvertToMessageView());
            }
            return result;
        }

        public async Task<DenominationResponse> InsertOrUpdateDenominationAsync(SetDenominationAsyncRequest req)
        {
            var taskSave = await Task.Factory.StartNew(() => InsertOrUpdateDenomination(req));
            return taskSave;
        }
        /// <summary>
        /// TFS#71743: Denomination => TblMasterActualJobItemsLiability_Denomination
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        private DenominationResponse InsertOrUpdateDenomination(SetDenominationAsyncRequest req)
        {
            var result = new DenominationResponse();
            using (var transaction = _uow.BeginTransaction())
            {
                try
                {
                    foreach (var model in req.DenominationHeaderList)
                    {
                        var liability = _masterActualJobItemsLiabilityRepository.FindById(model.LiabilityGuid);

                        RemoveDenomination(model, liability);

                        InsertDenomination(req, model, liability);

                        ModifyDenomination(req, model);

                        ModifyLiability(model, liability);
                    }
                    //output
                    var msgID = 0;
                    var message = _systemMessageRepository.FindByMsgId(msgID, req.LanguageGuid);
                    result.SetMessageView(message.ConvertToMessageView(true));

                    _uow.Commit();
                    transaction.Complete();
                }
                catch (Exception ex)
                {
                    _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
                    result.SetMessageView(_systemMessageRepository.FindByMsgId(-184, req.LanguageGuid).ConvertToMessageView());
                }
            }

            return result;
        }

        private void ModifyLiability(DenominationHeaderView model, TblMasterActualJobItemsLiability liability)
        {
            if (liability != null && model.CurrencyGuid != null)
            {
                var deno = model.DenominationList.Where(o => o.ItemState != EnumState.Deleted);
                var totalValue = deno.Sum(s => s.Value);
                var flagOverrideLiability = totalValue > 0;
                if (flagOverrideLiability)
                {
                    liability.Liability = totalValue;
                }
                liability.MasterCurrency_Guid = model.CurrencyGuid;
                _masterActualJobItemsLiabilityRepository.Modify(liability);
            }
        }

        private void ModifyDenomination(SetDenominationAsyncRequest req, DenominationHeaderView model)
        {
            var modifyDenoList = model.DenominationList.Where(o => o.ItemState == EnumState.Modified);

            foreach (var item in modifyDenoList)
            {
                var baseDenomination = _masterActualJobItemsLiabilityDenominationRepository.FindById(item.LiabilityDenoGuid);

                if (baseDenomination != null)
                {
                    baseDenomination.Qty = item.Qty;
                    baseDenomination.DenominationValue = item.DenoValue;
                    baseDenomination.Value = item.Value;
                    baseDenomination.UserModified = req.UserName;
                    baseDenomination.DatetimeModified = req.ClientDateTime;
                    baseDenomination.UniversalDatetimeModified = req.UniversalDatetime;

                    _masterActualJobItemsLiabilityDenominationRepository.Modify(baseDenomination);
                }
            }
        }

        private void InsertDenomination(SetDenominationAsyncRequest req, DenominationHeaderView model, TblMasterActualJobItemsLiability liability)
        {
            if (liability != null)
            {
                var insertListLiabilityDeno = model.DenominationList.Where(o => o.ItemState == EnumState.Added);
                var baseInsertLiabilityDeno = insertListLiabilityDeno.Select(o => new TblMasterActualJobItemsLiability_Denomination
                {
                    Guid = o.LiabilityDenoGuid ?? Guid.NewGuid(),
                    MasterActualJobItemsLiability_Guid = model.LiabilityGuid,
                    MasterDenomination_Guid = o.DenoGuid,
                    SystemDenominationUnit_Guid = o.DenoUnitGuid,
                    Qty = o.Qty,
                    DenominationValue = o.DenoValue,
                    Value = o.Value,
                    DenominationText = o.DenoName,
                    UserCreated = req.UserName,
                    DatetimeCreated = req.ClientDateTime,
                    UniversalDatetimeCreated = req.UniversalDatetime
                });
                _masterActualJobItemsLiabilityDenominationRepository.CreateRange(baseInsertLiabilityDeno);
            }
        }

        private void RemoveDenomination(DenominationHeaderView model, TblMasterActualJobItemsLiability liability)
        {
            if (liability != null && model.FlagDeletePrevDeno)
            {
                var deleteListLiabilityDeno = _masterActualJobItemsLiabilityDenominationRepository.FindByLiabilityGuid(model.LiabilityGuid);
                _masterActualJobItemsLiabilityDenominationRepository.RemoveRange(deleteListLiabilityDeno);
            }
        }
    }
}
