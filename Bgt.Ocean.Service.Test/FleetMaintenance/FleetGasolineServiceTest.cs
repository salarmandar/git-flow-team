using Bgt.Ocean.Models;
using Bgt.Ocean.Repository.EntityFramework.Repositories.FleetMaintenance;
using Bgt.Ocean.Service.Implementations.FleetMaintenance;
using Bgt.Ocean.Service.Messagings;
using System;
using System.Globalization;
using Xunit;

namespace Bgt.Ocean.Service.Test.FleetMaintenance
{
    public class FleetGasolineServiceTest : BaseTest
    {
        private readonly IFleetGasolineService _fleetGasolineService;
        public FleetGasolineServiceTest()
        {
            _fleetGasolineService = Util.CreateInstance<FleetGasolineService>();
        }

        
        [Theory]
        [InlineData("dd-MM-yyyy")]
        [InlineData("MM-dd-yyyy")]
        [InlineData("yyyy-MM-dd")]
        [InlineData("dd/MM/yyyy")]
        [InlineData("MM/dd/yyyy")]
        [InlineData("yyyy/MM/dd")]
        public void GetGasolineDataByGuid_ReturnGasolineData(string formatDate)
        {
            Guid runGasolineGuid = new Guid("B9EEFD20-CAD8-4293-8232-548423DB786B");
            Guid gasolineGuid = new Guid("F387A47F-2BFC-4B55-BD36-D880AC3B524E");
            Guid gasolineVendorGuid = new Guid("9E329EC0-62F5-4C85-B246-C9615F5FAD33");
            
            var dummyRunGasoline = Util.CreateDummy<TblMasterRunResource_GasolineExpense>();
            dummyRunGasoline.Guid = runGasolineGuid;
            dummyRunGasoline.MasterGasloine_Guid = gasolineGuid;

            string strTopUpDate = dummyRunGasoline.TopUpDate.ToString(formatDate, CultureInfo.InvariantCulture);
            string strTopUpTime = dummyRunGasoline.TopUpDate.ToString("HH:mm");


            var dummyGasoline = Util.CreateDummy<TblMasterGasloine>();
            dummyGasoline.Guid = gasolineGuid;
            dummyGasoline.MasterGasloineVendor_Guid = gasolineVendorGuid;

            var dummyGasolineVendor = Util.CreateDummy<TblMasterGasloine_Vendor>();
            dummyGasolineVendor.Guid = gasolineVendorGuid;

            _fleetGasolineService.GetMock<IBaseRequest>().Setup(fn => fn.Data)
                                  .Returns(new RequestBase()
                                  {
                                      UserFormatDate = formatDate,
                                      UserName = "unittest",
                                      UserLanguageGuid = Guid.Empty
                                  });

            _fleetGasolineService.GetMock<IMasterGasolineRepository>()
                                 .Setup(fn => fn.FindById(gasolineGuid))
                                 .Returns(dummyGasoline);

            _fleetGasolineService.GetMock<IMasterGasolineVendorRepository>()
                                 .Setup(fn => fn.FindById(gasolineVendorGuid))
                                 .Returns(dummyGasolineVendor);

            _fleetGasolineService.GetMock<IMasterRunResourceGasolineExpenseRepository>()
                                 .Setup(fn => fn.FindById(runGasolineGuid))
                                 .Returns(dummyRunGasoline);

            var result = _fleetGasolineService.GetGasolineDetail(runGasolineGuid);

            Assert.NotNull(result);
            Assert.Equal(strTopUpDate, result.StrTopUpDate);
            Assert.Equal(strTopUpTime, result.StrTopUpTime);
        }
    }
}
