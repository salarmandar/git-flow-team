using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Models.PreVault;
using Bgt.Ocean.Service.Mapping.Mappers;
using Bgt.Ocean.Service.Messagings;
using Bgt.Ocean.Service.Messagings.PreVault;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bgt.Ocean.Service.Implementations.PreVault
{
    public partial interface IVaultBalanceService
    {
        VaultBalanceStateResponse GetVaultBalanceState(VaultBalanceRequest req);
    }

    public partial class VaultBalanceService : RequestBase, IVaultBalanceService
    {

        /// <summary>
        /// =>  TFS#71736:Vault Balance Report
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public VaultBalanceStateResponse GetVaultBalanceState(VaultBalanceRequest req)
        {

            var result = new VaultBalanceStateResponse();
            var msgID = 0;
            var message = _systemMessageRepository.FindByMsgId(msgID, _req.Data.UserLanguageGuid.GetValueOrDefault());
            IEnumerable<VaultStateModel> state = null;
            IEnumerable<Guid> vault = null;

            Action validateState = () =>
            {
                state = _vaultBalanceHeaderRepository.GetVaultBalanceStateByInternalDepartment(req.InternalGuidList.Where(o => o != null));
                msgID = state.Any(s => s.VaultState == EnumVaultState.Process) ? msgID : 0;
            };

            //Handle Page
            switch (req.PageId)
            {
                case EnumPageName.CashDeliveryImport:
                    vault = _vaultBalanceHeaderRepository.GetInternalDepartmentBySiteGuid(req.SiteGuid);
                    req.InternalGuidList = new Guid?[] { vault.Take(1).FirstOrDefault() };
                    msgID = -17376;
                    validateState();
                    break;
                case EnumPageName.CashDeliveryPreparation:
                    vault = _vaultBalanceHeaderRepository.GetInternalDepartmentBySiteGuid(req.SiteGuid);
                    req.InternalGuidList = vault.Select(v => (Guid?)v);
                    msgID = -17376;
                    validateState();
                    break;
                case EnumPageName.BankCleanOut:
                    vault = _vaultBalanceHeaderRepository.GetInternalDepartmentBySiteGuid(req.SiteGuid);
                    req.InternalGuidList = req.InternalGuidList.Where(o => vault.Any(v => v == o));
                    msgID = -180011;
                    validateState();
                    break;
                case EnumPageName.Deconsolidation:
                case EnumPageName.Consolidation:
                    msgID = -17376;
                    vault = _vaultBalanceHeaderRepository.GetInternalDepartmentByItemsGuid(req.SealGuidList, req.CommodityGuidList, req.ConsolidateGuidList);
                    req.InternalGuidList = vault.Select(v => (Guid?)v);
                    validateState();
                    break;
                default:
                    msgID = -17376;
                    validateState();
                    break;
            }

            //Handle Massage
            switch (msgID)
            {
                case -180011:
                    var text = state.Where(s => s.VaultState == EnumVaultState.Process).Select(o => o.InternalFullName).Distinct();
                    text = text.To3DotAfterTake(3);
                    var _message = _systemMessageRepository.FindByMsgId(-180011, _req.Data.UserLanguageGuid.GetValueOrDefault()).ConvertToMessageView(false);
                    _message.MessageTextContent = string.Format(_message.MessageTextContent, string.Join(",", text.Select(o => o).Distinct()));
                    result.SetMessageView(_message);

                    break;
                case -17376:
                    message = _systemMessageRepository.FindByMsgId(-17376, _req.Data.UserLanguageGuid.GetValueOrDefault());
                    result.SetMessageView(message.ConvertToMessageView(false));
                    break;
                case 0:
                    result.SetMessageView(message.ConvertToMessageView(true));
                    break;
                default:
                    break;
            }

            result.VaultState = state;

            return result;
        }

    }
}
