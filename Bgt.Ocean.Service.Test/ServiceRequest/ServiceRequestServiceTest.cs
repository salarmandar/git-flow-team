using Bgt.Ocean.Models;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Bgt.Ocean.Service.Implementations.AuditLog.ServiceRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Bgt.Ocean.Service.Test.ServiceRequest
{
    public class ServiceRequestServiceTest : BaseTest
    {
        private readonly IServiceRequestCreatorService _serviceRequestCreatorService;
        public ServiceRequestServiceTest()
        {
            _serviceRequestCreatorService = Util.CreateInstance<ServiceRequestCreatorService>();
        }

        #region [SFO 21.2.0] Suffix ticket number
        [Theory]
        [InlineData(390414)]
        [InlineData(1234)]
        [InlineData(12345)]
        [InlineData(999999)]
        public void GenerateTicket_Suffix3Digit_ShouldReturnTrue(int runningVal)
        {
            var mockRunningValue = Util.CreateDummy<TblSystemRunningVaule_Global>();
            mockRunningValue.RunningKey = "ServiceRequestRunning";
            mockRunningValue.RunningVaule1 = runningVal;

            _serviceRequestCreatorService.GetMock<ISystemRunningValueGlobalRepository>()
                .Setup(fn => fn.GetServiceRequestRunning())
                .Returns(mockRunningValue);

            string runningValue = (mockRunningValue.RunningVaule1 + 1).ToString("D5");
            string ticketNumber1 = (string)_serviceRequestCreatorService.InvokeMethod("GetNewTicketNumber");
            int countSuffix = ticketNumber1.Length - runningValue.Length;

            Assert.Equal(3, countSuffix);
            Assert.NotEqual(runningVal.ToString("D5"), runningValue);
        }

        [Theory]
        [InlineData(10)]
        public async Task GenerateTicketNotDuplicate_ShouldReturnTrue(int countGenerate)
        {
            int runningVal = 390414;
            var mockRunningValue = Util.CreateDummy<TblSystemRunningVaule_Global>();
            mockRunningValue.RunningKey = "ServiceRequestRunning";
            mockRunningValue.RunningVaule1 = runningVal;

            _serviceRequestCreatorService.GetMock<ISystemRunningValueGlobalRepository>()
                .Setup(fn => fn.GetServiceRequestRunning())
                .Returns(mockRunningValue);

            List<Task<string>> ticketList = new List<Task<string>>();
            

            for (int i = 0; i < countGenerate; i++)
            {
                var ticket = GetNewTicketNumberAsync();
                ticketList.Add(ticket);
            }

            string[] result = await Task.WhenAll(ticketList);
            bool isNotDuplicate = result.Distinct().Count() == countGenerate;
            Assert.True(isNotDuplicate);
        }

        private async Task<string> GetNewTicketNumberAsync()
        {
            return (string)_serviceRequestCreatorService.InvokeMethod("GetNewTicketNumber");
        }
        #endregion
    }

}
