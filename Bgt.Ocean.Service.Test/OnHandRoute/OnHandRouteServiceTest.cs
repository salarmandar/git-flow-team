using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.ActualJob;
using Bgt.Ocean.Models.OnHandRoute;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Job;
using Bgt.Ocean.Repository.EntityFramework.Repositories.MasterRoute;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Bgt.Ocean.Service.Implementations.MasterRoute;
using Bgt.Ocean.Service.Mapping.Mappers;
using Bgt.Ocean.Service.Messagings.MasterRouteService;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static Bgt.Ocean.Infrastructure.Util.EnumRoute;

namespace Bgt.Ocean.Service.Test.OnHandRoute
{
    public class OnHandRouteServiceTest
    {
        private readonly IOnHandRouteService _onHandRouteService;
        public OnHandRouteServiceTest()
        {
            _onHandRouteService = Util.CreateInstance<OnHandRouteService>();
        }

        [Fact]
        public void GetJobOnRun_ShouldReturnSumQty()
        {
            Guid dailyRunGuid = new Guid("5D21197B-5AE2-4174-AECC-0FBE9C100CB9");

            Util.CreateFakeContext();
            List<Guid> dailyRunList = new List<Guid> { dailyRunGuid };
            List<TblMasterActualJobItemsSeal> mockSeal = new List<TblMasterActualJobItemsSeal>();
            List<JobDetailOnRunView> mockJobDetail = new List<JobDetailOnRunView>();

            #region Job normal

            //(1) P: Done = 3, Unable = 1
            int[] statusP = new int[] { IntStatusJob.Department, IntStatusJob.PickedUp, IntStatusJob.InPreVault, IntStatusJob.Unrealized };
            mockJobDetail.AddRange(Util.CreateDummy<JobDetailOnRunView>(statusP.Count())
                .Select((j, i) =>
                {
                    j.JobTypeID = IntTypeJob.P;
                    j.GroupJobTypeID = IntTypeJob.P;
                    j.JobStatusID = statusP[i];
                    return j;
                }).ToList());

            //(2) D: Done = 2, Unable = 1
            int[] statusD = new int[] { IntStatusJob.Delivered, IntStatusJob.VisitWithStamp, IntStatusJob.ReturnToPreVault };
            mockJobDetail.AddRange(Util.CreateDummy<JobDetailOnRunView>(statusD.Count())
                .Select((j, i) =>
                {
                    j.JobTypeID = IntTypeJob.P;
                    j.GroupJobTypeID = IntTypeJob.P;
                    j.JobStatusID = statusD[i];
                    return j;
                }).ToList());

            //(3) T: Done = 2, Unable = 1
            int[] statusT = new int[] { IntStatusJob.Delivered, IntStatusJob.ReturnToPreVault };
            mockJobDetail.AddRange(Util.CreateDummy<JobDetailOnRunView>(statusT.Count())
                .Select((j, i) =>
                {
                    j.JobTypeID = IntTypeJob.P;
                    j.GroupJobTypeID = IntTypeJob.P;
                    j.JobStatusID = statusT[i];
                    return j;
                }).ToList());

            //(4) TV: Done = 6, Unable = 1
            int[] statusTV = new int[] { IntStatusJob.Delivered, IntStatusJob.PickedUp, IntStatusJob.InPreVault, IntStatusJob.OnTruckDelivery, IntStatusJob.OnTheWayDelivery,
                                          IntStatusJob.ReturnToPreVault};
            mockJobDetail.AddRange(Util.CreateDummy<JobDetailOnRunView>(statusTV.Count())
                .Select((j, i) =>
                {
                    j.JobTypeID = IntTypeJob.P;
                    j.GroupJobTypeID = IntTypeJob.P;
                    j.JobStatusID = statusTV[i];
                    return j;
                }).ToList());

            #endregion

            #region Job Multi br.

            //(5) P_MultiBr: Done = 6, Unable = 1
            int[] statusPM = new int[] { IntStatusJob.Department, IntStatusJob.PickedUp, IntStatusJob.InPreVault, IntStatusJob.IntransitInterBranch, IntStatusJob.ReturnToPreVault };
            mockJobDetail.AddRange(Util.CreateDummy<JobDetailOnRunView>(statusPM.Count())
           .Select(j =>
           {
               j.JobTypeID = IntTypeJob.P_MultiBr;
               j.GroupJobTypeID = IntTypeJob.P;
               j.JobStatusID = JobStatusHelper.InDepartment;
               return j;
           }).ToList());

            //(6) TV_MultiBr: In Prevault Pickup
            int[] statusTVM = new int[] { IntStatusJob.Department, IntStatusJob.PickedUp, IntStatusJob.InPreVaultPickUp, IntStatusJob.InPreVaultDelivery, IntStatusJob.IntransitInterBranch, IntStatusJob.ReturnToPreVault };
            mockJobDetail.AddRange(Util.CreateDummy<JobDetailOnRunView>(statusTVM.Count())
           .Select(j =>
           {
               j.JobTypeID = IntTypeJob.TV_MultiBr;
               j.GroupJobTypeID = IntTypeJob.TV;
               j.JobStatusID = JobStatusHelper.InPreVaultPickUp;
               return j;
           }).ToList());

            //(7) BCD_MultiBr: Return to prevault
            int[] statusBCDM = new int[] { IntStatusJob.Department, IntStatusJob.InPreVaultPickUp, IntStatusJob.InPreVaultDelivery, IntStatusJob.IntransitInterBranch, IntStatusJob.ReturnToPreVault };
            mockJobDetail.AddRange(Util.CreateDummy<JobDetailOnRunView>(statusBCDM.Count())
           .Select(j =>
           {
               j.JobTypeID = IntTypeJob.BCD_MultiBr;
               j.GroupJobTypeID = IntTypeJob.BCD;
               j.JobStatusID = JobStatusHelper.ReturnToPreVault;
               return j;
           }).ToList());

            #endregion

            foreach (var item in mockJobDetail)
            {
                mockSeal.AddRange(Util.CreateDummy<TblMasterActualJobItemsSeal>(2)
                    .Select(m =>
                    {
                        m.MasterActualJobHeader_Guid = item.JobGuid;
                        return m;
                    }));
            }

            //Mock Job Detail
            _onHandRouteService.GetMock<IMasterRouteRepository>()
                .Setup(fn => fn.GetJobDetailOnRun(dailyRunList, It.IsAny<Guid>(), false))
                .Returns(mockJobDetail);

            List<OnHandJobOnRunView> mapJobDetail = mockJobDetail.ConvertToOnHandMasterRouteResponse().ToList();
            List<OnHandJobOnRunView> mockJobDetailList = (List<OnHandJobOnRunView>)_onHandRouteService.InvokeMethod("SeparateSectionJobs", mapJobDetail);

            var jobList = mapJobDetail.Select(x => x.JobGuid);

            //Mock STC
            List<TblMasterCurrency_ExchangeRate> exchangeRate = new List<TblMasterCurrency_ExchangeRate>();
            for (int i = 0; i < 4; i++)
            {
                exchangeRate.Add(Util.CreateDummy<TblMasterCurrency_ExchangeRate>());
            }
         
            var mockSTC = Util.CreateDummy<IEnumerable<JobWithStcView>>();
            _onHandRouteService.GetMock<IMasterActualJobHeaderRepository>()
                .Setup(fn => fn.GetSTCOnHandByJobList(jobList, It.IsAny<Guid>(), It.IsAny<Guid?>(), exchangeRate, It.IsAny<Guid>(), true, true))
                .Returns(mockSTC);

            //Mock Seal
            _onHandRouteService.GetMock<IMasterActualJobItemsSealRepository>()
                .Setup(fn => fn.FindSealOnHandByJob(jobList))
                .Returns(mockSeal);

            //Mock Nonbarcode
            _onHandRouteService.GetMock<IMasterActualJobItemsCommodityRepository>()
                .Setup(fn => fn.FindCommodityOnHandByJob(jobList))
                .Returns(new List<TblMasterActualJobItemsCommodity>());

            //Mock Message
            var mockMsg = Util.CreateDummy<TblSystemMessage>(1).FirstOrDefault();
            mockMsg.MsgID = 0;
            mockMsg.MessageTextContent = "Success";
            mockMsg.MessageTextTitle = "Success (Msg ID: 0)";

            _onHandRouteService.GetMock<ISystemMessageRepository>()
                .Setup(fn => fn.FindByMsgId(0, It.IsAny<Guid>()))
                .Returns(mockMsg);

            var result = (OnHandRouteResponse)_onHandRouteService.InvokeMethod("GetDetailJobByDailyRun", It.IsAny<Guid>(), dailyRunList);

            Assert.Equal(mockJobDetailList.Count, result.GrpJobOnRunList.Sum(e => e.Qty));
            Assert.Equal(mockJobDetailList.Count(e => e.FlagSecServiceDone), result.GrpJobServiceDone.Sum(e => e.Qty));
            Assert.Equal(mockJobDetailList.Count(e => e.FlagSecUnableToService), result.GrpJobUnable.Sum(e => e.SumQty));
        }

    }
}
