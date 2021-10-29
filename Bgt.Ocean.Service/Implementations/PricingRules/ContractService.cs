using Bgt.Ocean.Models;
using Bgt.Ocean.Repository.EntityFramework.Repositories;
using Bgt.Ocean.Service.ModelViews.PricingRules;
using System;
using System.Collections.Generic;
using System.Linq;
using static Bgt.Ocean.Infrastructure.Helpers.L2CHelper;
using Bgt.Ocean.Service.Mapping.Mappers;

namespace Bgt.Ocean.Service.Implementations.PricingRules
{
    public interface IContractService
    {
        bool VerifyContract(Guid contract_Guid);
        TblMasterCustomerContract GetContractDetail(Guid contract_Guid);
        IEnumerable<PricingRuleView> GetPricingRuleInContract(Guid contract_Guid);
    }

    public class ContractService : IContractService
    {
        private readonly IMasterCustomerContractRepository _contractRepository;
        public ContractService(
            IMasterCustomerContractRepository contractRepository)
        {
            _contractRepository = contractRepository;
        }

        public TblMasterCustomerContract GetContractDetail(Guid contract_Guid)
        {
            return _contractRepository.FindById(contract_Guid);
        }

        public bool VerifyContract(Guid contract_Guid)
        {
            var contract = _contractRepository.FindById(contract_Guid);
            return contract != null && contract.SignedDate.HasValue && contract.TblSystemLeedToCashContractStatus.StatusID == ContractStatus.Signed;
        }

        public IEnumerable<PricingRuleView> GetPricingRuleInContract(Guid contract_Guid)
        {
            var contract = _contractRepository.FindById(contract_Guid);
            var productPricingRuleView = contract.TblLeedToCashContract_PricingRules.Select(e => e.TblPricingRule).ConvertToPricingRuleView();
            return productPricingRuleView.OrderBy(e => e.ProductID).ThenBy(e => e.ProductName);
        }
    }
}
