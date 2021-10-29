using Bgt.Ocean.Models;
using Bgt.Ocean.Repository.EntityFramework.Repositories;
using Bgt.Ocean.Repository.EntityFramework.Repositories.CustomerLocation;
using Bgt.Ocean.Service.Implementations.StandardTable;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Bgt.Ocean.Service.Test.CustomerLocation
{
    public class ChangeCustomerTest : BaseTest
    {
        private readonly IMasterCustomerLocationService _masterCustomerLocationService;


        public ChangeCustomerTest()
        {
            _masterCustomerLocationService = Util.CreateInstance<MasterCustomerLocationService>();
        }

        [Fact]
        public void ValidGetChangeCustomer()
        {
            Guid currentCustomerGuid = Guid.NewGuid();
            Guid countryGuid = Guid.NewGuid();
            var currentLocation = new TblMasterCustomerLocation
            {
                Guid = Guid.NewGuid(),
                MasterCustomer_Guid = currentCustomerGuid,
                BranchName = "Location1"
            };

            var currentCustomer = new TblMasterCustomer {
                Guid = currentCustomerGuid,
                MasterCountry_Guid = countryGuid,
                CustomerFullName = "CurrentCustomer",
                FlagDisable = false,
                FlagChkCustomer = true
            };
            var listNewCustomer = new List<TblMasterCustomer>();
            listNewCustomer.Add(new TblMasterCustomer
            {
                Guid = Guid.NewGuid(),
                MasterCountry_Guid = countryGuid,
                CustomerFullName = "NewCustomer1",
                FlagDisable = false,
                FlagChkCustomer = true
            });
            listNewCustomer.Add(new TblMasterCustomer
            {
                Guid = Guid.NewGuid(),
                MasterCountry_Guid = countryGuid,
                CustomerFullName = "NewCustomer2",
                FlagDisable = false,
                FlagChkCustomer = true
            });
            listNewCustomer.Add(new TblMasterCustomer
            {
                Guid = Guid.NewGuid(),
                MasterCountry_Guid = countryGuid,
                CustomerFullName = "NewCustomer2",
                 FlagDisable = false,
                FlagChkCustomer = true
            });
            listNewCustomer.Add(currentCustomer);

            _masterCustomerLocationService.GetMock<IMasterCustomerLocationRepository>()
                           .Setup(fn => fn.FindById(It.IsAny<object>())).Returns(currentLocation);
            _masterCustomerLocationService.GetMock<IMasterCustomerRepository>()
                         .Setup(fn => fn.FindById(It.IsAny<object>())).Returns(currentCustomer);
            _masterCustomerLocationService.GetMock<IMasterCustomerRepository>()
                       .Setup(fn => fn.FindAllAsQueryable())
                       .Returns(listNewCustomer.AsQueryable());
            var result = _masterCustomerLocationService.GetChangeCustomer(currentLocation.Guid);

            Assert.Equal(currentLocation.Guid, result.LocationGuid);
            Assert.True(result.NewCustomer.Any());
            Assert.False(result.NewCustomer.Any(a => a.Guid == currentCustomerGuid));           
        }
    }
}
