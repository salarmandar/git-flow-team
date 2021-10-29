using Bgt.Ocean.Infrastructure.Storages;
using Bgt.Ocean.Service.Implementations.Denomination;
using Bgt.Ocean.Service.Messagings.Denomination;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace Bgt.Ocean.WebAPI.Controllers.Internals.v1
{
    public class v1_DenominationController : ApiControllerBase
    {

        private readonly IDenominationService _denominationService;

        public v1_DenominationController(
            IDenominationService denominationService
            )
        {
            _denominationService = denominationService;
        }

        [HttpGet]
        public DenominationResponse GetMasterDenomination(Guid? currencyGuid)
        {
            return _denominationService.GetMasterDenomination(new GetDenominationRequest { CurrencyGuid = currencyGuid });
        }

        [HttpGet]
        public DenominationResponse GetLiabilityDenominationByLiabilityGuid(Guid? liabilityGuid)
        {
            return _denominationService.GetLiabilityDenominationByLiabilityGuid(new GetDenominationRequest { LiabilityGuid = liabilityGuid });
        }

        [HttpPost]
        public async Task<DenominationResponse> InsertUpdateLiabilityDenomination(SetDenominationRequest req)
        {
            SetDenominationAsyncRequest request = new SetDenominationAsyncRequest
            {
                DenominationHeaderList = req.DenominationHeaderList,
                LanguageGuid = ApiSession.UserLanguage_Guid.GetValueOrDefault(),
                UserName = ApiSession.UserName,
                ClientDateTime = ApiSession.ClientDatetime.DateTime,
                FormatDate = ApiSession.UserFormatDate,
                UniversalDatetime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc)
            };
            return await _denominationService.InsertOrUpdateDenominationAsync(request);
        }

    }
}