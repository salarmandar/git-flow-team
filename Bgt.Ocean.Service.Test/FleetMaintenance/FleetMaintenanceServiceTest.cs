using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.FleetMaintenance;
using Bgt.Ocean.Repository.EntityFramework.Repositories;
using Bgt.Ocean.Repository.EntityFramework.Repositories.FleetMaintenance;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Bgt.Ocean.Service.Implementations.FleetMaintenance;
using Bgt.Ocean.Service.Messagings;
using Bgt.Ocean.Service.Messagings.FleetMaintenance;
using System;
using System.Collections.Generic;
using Xunit;

namespace Bgt.Ocean.Service.Test.FleetMaintenance
{
    public class FleetMaintenanceServiceTest : BaseTest
    {
        private readonly IFleetMaintenanceService _fleetMaintenanceService;


        public FleetMaintenanceServiceTest()
        {
            _fleetMaintenanceService = Util.CreateInstance<FleetMaintenanceService>();
        }

        public static IEnumerable<object[]> DiscountData()
        {
            return new List<object[]>
            {
              new object[] { EnumDiscountType.Currency,1,100,10,90 },
              new object[] { EnumDiscountType.Currency,2,100,20,180 },

              new object[] { EnumDiscountType.Percentage,1,100,10,90 },
              new object[] { EnumDiscountType.Percentage,2,100,20,160 },
            };
        }

        [Theory]
        [MemberData(nameof(DiscountData))]
        public void ValidateGetFleetMaintenanceDetail_ModelValidDiscount(EnumDiscountType distype, int qty, int price, double discount, double total)
        {
            var item = new MaintenanceCategoryDetailView
            {
                DiscountType = distype,
                PartQty = qty,
                UnitPrice = price,
                Discount = discount,
                Total = total
            };
            Assert.True(item.Validate());
        }

        [Fact]
        public void ValidateGetFleetMaintenanceDetail_ReturnCurrectly()
        {

            var mGuid = Guid.NewGuid();
            var cGuid = Guid.NewGuid();
            var dateFrom = new DateTime(2020, 12, 30);
            var dateTo = new DateTime(2021, 01, 01);

            _fleetMaintenanceService.GetMock<IBaseRequest>().Setup(fn => fn.Data)
                                    .Returns(new RequestBase()
                                    {
                                        UserFormatDate = "dd/MM/yyyy",
                                        UserName = "unittest",
                                        UserLanguageGuid = Guid.Empty
                                    });
            _fleetMaintenanceService.GetMock<IMasterCurrencyRepository>().Setup(fn => fn.FindById(cGuid)).Returns(new TblMasterCurrency() { Guid = cGuid, MasterCurrencyAbbreviation = "THB" });
            _fleetMaintenanceService.GetMock<IMasterRunResourceMaintenanceRepository>().Setup(fn => fn.FindById(mGuid)).Returns(new TblMasterRunResource_Maintenance
            {
                Guid = mGuid,
                MaintenanceID = 111,
                Open_DateServiceFrom = dateFrom,
                Open_DateServiceTo = dateTo,
                Discount_Type = (int)EnumDiscountType.Percentage,
                CurrencyActual_Guid = cGuid,
                CurrencyEstimate_Guid = cGuid
            });
            _fleetMaintenanceService.GetMock<ISystemMessageRepository>().Setup(fn => fn.FindByMsgId(0, Guid.Empty)).Returns(new TblSystemMessage { MsgID = 0, MessageTextTitle = "A", MessageTextContent = "B" });
            var req = new FleetMaintenanceDetailRequest() { MaintenanceGuid = mGuid };
            var result = _fleetMaintenanceService.GetFleetMaintenanceDetail(req);

            Assert.True(result.MaintenanceModel.strOpen_DateServiceFrom == "30/12/2020");
            Assert.True(result.MaintenanceModel.strOpen_DateServiceTo == "01/01/2021");
            Assert.True(result.MsgID == 0);
        }
    }
}
