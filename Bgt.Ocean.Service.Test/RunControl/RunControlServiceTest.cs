using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.Masters;
using Bgt.Ocean.Models.RunControl;
using Bgt.Ocean.Repository.EntityFramework.Repositories;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Job;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Mobile;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Bgt.Ocean.Service.Implementations.RunControl;
using Bgt.Ocean.Service.Messagings.RunControlService;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Bgt.Ocean.Service.Test.RunControl
{
    public class RunControlServiceTest
    {
        public class RunControl_JobProperties : BaseTest
        {
            private readonly IRunControlService _runControlService;            


            public RunControl_JobProperties()
            {
                _runControlService = Util.CreateInstance<RunControlService>();
            }

            #region GET

            private JobPropertiesRequest ValidateSeviceDetail_TearUp(bool? flagOrderMCSAscending)
            {
                var request = CreateDummy<JobPropertiesRequest>();
                _runControlService.GetMock<IMasterCommodityRepository>().Setup(fn => fn.GetCCBySite(request.SiteGuid, false, true, false, false)).Returns(CreateDummy<IEnumerable<CommodityView>>());
                _runControlService.GetMock<ISystemEnvironmentMasterCountryRepository>().Setup(fn => fn.FindAppkeyValueByEnumAppkeyName(request.SiteGuid, EnumAppKey.FlagOrderMCSAscending)).Returns(flagOrderMCSAscending.GetValueOrDefault());

                var inMR = CreateDummy<SvdNoteWithdrawMachineReport>();
                var inAC = CreateDummy<SvdNoteWithdrawActualCount>();
                var inCA = CreateDummy<SvdNoteWithdrawCashAdd>();
                var inCR = CreateDummy<SvdNoteWithdrawCashReturn>();

                inMR.CassetteList = CreateDummy<IEnumerable<MRNWTransectionView>>();
                inAC.CassetteList = CreateDummy<IEnumerable<ACNWTransectionView>>();
                inCA.CassetteList = CreateDummy<IEnumerable<CANWTransectionView>>();
                inCR.CassetteList = CreateDummy<IEnumerable<CRNWTransectionView>>();
                _runControlService.GetMock<IMasterActualJobSumMachineReportRepository>().Setup(fn => fn.GetMachineReportDetail(request.JobGuid)).Returns(inMR);
                _runControlService.GetMock<IMasterActualJobSumActualCountRepository>().Setup(fn => fn.GetActualCountDetail(request.JobGuid)).Returns(inAC);
                _runControlService.GetMock<IMasterActualJobSumCashAddRepository>().Setup(fn => fn.GetCashAddDetail(request.JobGuid)).Returns(inCA);
                _runControlService.GetMock<IMasterActualJobSumCashReturnRepository>().Setup(fn => fn.GetCashReuturnDetail(request.JobGuid)).Returns(inCR);

                return request;
            }

            public static IEnumerable<object[]> SeviceDetailData()
            {
                return new List<object[]>
                        {

                             new object[] {default(bool?)},
                             //CASE FlagOrderMCSAscending = false => always ASC
                             new object[] { false},
                             //CASE FlagOrderMCSAscending = true => always ASC
                             new object[] { true},
                        };
            }
            [Theory]
            [MemberData(nameof(SeviceDetailData))]
            public void ValidateSeviceDetail_ReturnMachineActualCount_CassetteAlwaysASC(bool? FlagOrderMCSAscending)
            {
                var request = ValidateSeviceDetail_TearUp(FlagOrderMCSAscending);

                //GET MC,AC
                request.TabID = CashAddPropertiesTab.tabSVD_NoteWithdraw_MachineReport_ActualCount;
                var result = (TabServiceDetailView)_runControlService.InvokeMethod("GetTabSeviceDetail", request);
                //Asert MC
                var MC = result.SVD_MachineReport.CassetteList;
                var min1 = MC.Min(o => o.CassetteSequence);
                Assert.True(min1 == MC.FirstOrDefault().CassetteSequence);
                //Asert AC
                var AC = result.SVD_ActualCount.CassetteList;
                var min2 = AC.Min(o => o.CassetteSequence);
                Assert.True(min2 == AC.FirstOrDefault().CassetteSequence);

                //GET CA
                request.TabID = CashAddPropertiesTab.tabSVD_NoteWithdraw_CashAdd_CashReturn;
                result = (TabServiceDetailView)_runControlService.InvokeMethod("GetTabSeviceDetail", request);
                //Asert CA
                var CA = result.SVD_CashAdd.CassetteList;
                var min3 = CA.Min(o => o.CassetteSequence);
                Assert.True(min3 == CA.FirstOrDefault().CassetteSequence);
            }


            [Fact]
            public void ValidateSeviceDetail_ReturnCashReturn_DenoASC()
            {
                var FlagOrderMCSAscending = true;
                var request = ValidateSeviceDetail_TearUp(FlagOrderMCSAscending);
                request.TabID = CashAddPropertiesTab.tabSVD_NoteWithdraw_CashAdd_CashReturn;

                var result = (TabServiceDetailView)_runControlService.InvokeMethod("GetTabSeviceDetail", request);
                var CR = result.SVD_CashReturn.CassetteList;
                var min2 = CR.Min(o => o.DenominationValue);
                Assert.True(min2 == CR.FirstOrDefault().DenominationValue);
            }

            [Fact]
            public void ValidateSeviceDetail_ReturnCashReturn_DenoDESC()
            {
                var FlagOrderMCSAscending = false;
                var request = ValidateSeviceDetail_TearUp(FlagOrderMCSAscending);
                request.TabID = CashAddPropertiesTab.tabSVD_NoteWithdraw_CashAdd_CashReturn;
                var result = (TabServiceDetailView)_runControlService.InvokeMethod("GetTabSeviceDetail", request);

                var CR = result.SVD_CashReturn.CassetteList;

                var min2 = CR.Min(o => o.DenominationValue);
                Assert.True(min2 == CR.LastOrDefault().DenominationValue);
            }

            public static IEnumerable<object[]> GetMCSAvailableTab()
            {

                Util.CreateFakeContext();
                var request = Util.CreateDummy<JobPropertiesRequest>();
                request.TabID = CashAddPropertiesTab.tabDetail;
                var Machine_Guid = Guid.NewGuid();
                var machine = new SFOTblMasterMachine();
                var defaultTab = new List<CashAddPropertiesTab>
                {
                    CashAddPropertiesTab.tabDetail,
                    CashAddPropertiesTab.tabLeg,
                     CashAddPropertiesTab.tabHistory,
                    CashAddPropertiesTab.tabServiceDetail,
                    CashAddPropertiesTab.tabSVD_CITDelivery,
                    CashAddPropertiesTab.tabSVD_CapturedCard,
                    CashAddPropertiesTab.tabSVD_Checklist,
                };
                return new List<object[]>
                {
                    //Should Show --> tab CIT Delivery + MR,AC + (MCS + CashAdd)
                    new object[] { request, Machine_Guid,new List<Capability> { Capability.NoteWithdraw }, SubServiceTypeHelper.CashAdd, machine,
                        //actual
                        new List<CashAddPropertiesTab> { CashAddPropertiesTab.tabSVD_NoteWithdraw_MachineReport_ActualCount
                        //expected
                        ,CashAddPropertiesTab.tabSVD_NoteWithdraw_CashAdd_CashReturn }.Union(defaultTab)
                    },
                    //Should Not Show --> tab CIT Delivery + MR,AC + (MCS)
                    new object[] { request, Machine_Guid,new List<Capability> { Capability.NoteWithdraw }, null, machine,
                        new List<CashAddPropertiesTab> { }.Union(defaultTab)
                    },


                    //Should Show --> MR,AC,CA,CR + (MCS + CashAdd)
                    new object[] { request, Machine_Guid,new List<Capability> { Capability.NoteWithdraw }, SubServiceTypeHelper.CashAdd, null,
                        new List<CashAddPropertiesTab> { CashAddPropertiesTab.tabSVD_NoteWithdraw_MachineReport_ActualCount
                        //expected
                        , CashAddPropertiesTab.tabSVD_NoteWithdraw_CashAdd_CashReturn }.Union(defaultTab)
                    },
                    //Should Not Show --> MR,AC,CA,CR + (MCS)
                    new object[] { request, Machine_Guid,new List<Capability> { Capability.NoteWithdraw }, null, machine,
                        //expected
                        new List<CashAddPropertiesTab> { }.Union(defaultTab)
                    },


                    //Should Show --> Recycling + (MCS + CashAdd)
                    new object[] { request, Machine_Guid,new List<Capability> { Capability.Recycling }, SubServiceTypeHelper.CashAdd, machine,
                        new List<CashAddPropertiesTab> { CashAddPropertiesTab.tabSVD_Recycling_MachineReportWODispense_ActualCount
                        //expected
                        , CashAddPropertiesTab.tabSVD_Recycling_CashRecycling }.Union(defaultTab)
                    },
                      //Should Not Show --> Recycling + (MCS)
                    new object[] { request, Machine_Guid,new List<Capability> { Capability.Recycling }, null, machine,
                        //expected
                        new List<CashAddPropertiesTab> { }.Union(defaultTab)
                    },

                    //Should Show --> Bulk Note Deposit + (MCS + CashAdd)
                    new object[] { request, Machine_Guid,new List<Capability> { Capability.BulkNoteDeposit }, SubServiceTypeHelper.CashAdd, machine,
                          //expected
                        new List<CashAddPropertiesTab> { CashAddPropertiesTab.tabSVD_BulkNoteDeposit_DepositReport_Retract, CashAddPropertiesTab.tabSVD_BulkNoteDeposit_SuspectFake
                        , CashAddPropertiesTab.tabSVD_BulkNoteDeposit_Jammed }.Union(defaultTab)
                    },
                      //Should Not Show --> Bulk Note Deposit + (MCS)
                    new object[] { request, Machine_Guid,new List<Capability> { Capability.BulkNoteDeposit }, null, machine,
                          //expected
                        new List<CashAddPropertiesTab> { }.Union(defaultTab)
                    },

                    //Should Show --> Small Bag Deposit + (MCS + CashAdd)
                    new object[] { request, Machine_Guid,new List<Capability> { Capability.SmallBagDeposit }, SubServiceTypeHelper.CashAdd, machine,
                        //expected
                        new List<CashAddPropertiesTab> { CashAddPropertiesTab.tabSVD_SmallBagDeposit_SmallBag }.Union(defaultTab)
                    },
                    //Should Show --> Small Bag Deposit + (MCS)
                    new object[] { request, Machine_Guid,new List<Capability> { Capability.SmallBagDeposit }, null, machine,
                        //expected
                        new List<CashAddPropertiesTab> { CashAddPropertiesTab.tabSVD_SmallBagDeposit_SmallBag }.Union(defaultTab)
                    },

                    //Other Case
                    //Should Show --> default
                    new object[] { request, null, new List<Capability> { }, SubServiceTypeHelper.CashAdd, null,
                        new List<CashAddPropertiesTab> { }.Union(defaultTab)
                    },
                      //Should Show --> default
                    new object[] { request, null, new List<Capability> { }, null, null,
                        new List<CashAddPropertiesTab> { }.Union(defaultTab)
                    },
                };
            }

            [Theory]
            [MemberData(nameof(GetMCSAvailableTab))]
            public void ValidateSeviceDetail_GetSvdAvailableTab_ShouldShowTabCorrectly(JobPropertiesRequest request, Guid? Machine_Guid, IEnumerable<Capability> capabilities, int subServiceTypeID, SFOTblMasterMachine machine, IEnumerable<CashAddPropertiesTab> expected)
            {
                List<HidePanel> listHidePanel = new List<HidePanel>()
                {
                    new HidePanel() { JobScreen = JobScreen.ActualCount , JobField = new List<JobField>() { JobField.Dispense_Beginning_TotalATM} }
                };                

                _runControlService.GetMock<IMasterActualJobHeaderCapabilityRepository>()
                                  .Setup(fn => fn.FindCapabilityIDByJobGuid(request.JobGuid)).Returns(capabilities);
                _runControlService.GetMock<IMasterActualJobHeaderRepository>()
                                  .Setup(fn => fn.FindSubServiceTypeIDByJobGuid(request.JobGuid)).Returns(subServiceTypeID);
                _runControlService.GetMock<IMasterActualJobHeaderRepository>()
                                  .Setup(fn => fn.GetMachineTransferSafeModel(Machine_Guid)).Returns(machine);
                _runControlService.GetMock<IMasterActualJobHeaderRepository>()
                                  .Setup(fn => fn.GetJobScreenMapping(request.JobGuid)).Returns(listHidePanel); //Enumerable.Empty<HidePanel>);

                var result = (IEnumerable<CashAddAvailableTab>)_runControlService.InvokeMethod("GetSvdAvailableTab", request, Machine_Guid);
                Assert.NotNull(result);
                Assert.True(result.All(o => expected.Contains(o.TabID)));

                if (request.TabID == CashAddPropertiesTab.tabSVD_NoteWithdraw_MachineReport_ActualCount)
                {
                    Assert.True(result.FirstOrDefault(e => e.TabID == CashAddPropertiesTab.tabSVD_NoteWithdraw_MachineReport_ActualCount).HidePanels.FirstOrDefault().JobScreen == JobScreen.ActualCount);
                    Assert.True(result.FirstOrDefault(e => e.TabID == CashAddPropertiesTab.tabSVD_NoteWithdraw_MachineReport_ActualCount).HidePanels.FirstOrDefault().JobField.FirstOrDefault() == JobField.Dispense_Beginning_TotalATM);
                }                
            }

            public static IEnumerable<object[]> GetMCSRequestByTabID()
            {

                Util.CreateFakeContext();

                Func<CashAddPropertiesTab, JobPropertiesRequest> getRequestByTabID = t =>
                {
                    var req = Util.CreateDummy<JobPropertiesRequest>();
                    req.TabID = t;
                    return req;
                };

                return new List<object[]>
                {
                    new object[] { getRequestByTabID(CashAddPropertiesTab.tabSVD_NoteWithdraw_MachineReport_ActualCount) },
                    new object[] { getRequestByTabID(CashAddPropertiesTab.tabSVD_NoteWithdraw_CashAdd_CashReturn) },
                    new object[] { getRequestByTabID(CashAddPropertiesTab.tabSVD_Recycling_MachineReportWODispense_ActualCount) },
                    new object[] { getRequestByTabID(CashAddPropertiesTab.tabSVD_Recycling_CashRecycling) },
                    //new object[] { getRequestByTabID(CashAddPropertiesTab.tabSVD_BulkNoteDeposit_DepositReport_Retract) },
                    //new object[] { getRequestByTabID(CashAddPropertiesTab.tabSVD_BulkNoteDeposit_SuspectFake) },
                    //new object[] { getRequestByTabID(CashAddPropertiesTab.tabSVD_BulkNoteDeposit_Jammed) },
                    //new object[] { getRequestByTabID(CashAddPropertiesTab.tabSVD_CoinExchange_MachineBalance_CashAdd) },
                    //new object[] { getRequestByTabID(CashAddPropertiesTab.tabSVD_Recycling_CashRecycling) },
                    //new object[] { getRequestByTabID(CashAddPropertiesTab.tabSVD_CoinExchange_CashReturn_BulkNote) },
                    //new object[] { getRequestByTabID(CashAddPropertiesTab.tabSVD_CoinExchange_SuspectFake) },
                    //new object[] { getRequestByTabID(CashAddPropertiesTab.tabSVD_SmallBagDeposit_SmallBag) },
                    new object[] { getRequestByTabID(CashAddPropertiesTab.tabSVD_Checklist) },
                    new object[] { getRequestByTabID(CashAddPropertiesTab.tabSVD_CITDelivery) }
                };
            }
            [Theory]
            [MemberData(nameof(GetMCSRequestByTabID))]
            public void ValidateSeviceDetail_GetSvdDetailCapability_ShouldWorkingNormally(JobPropertiesRequest request)
            {
                _runControlService.GetMock<IMasterActualJobHeaderRepository>().Setup(fn => fn.GetLiability(request.SiteGuid, request.JobGuid, default(IEnumerable<CommodityView>))).Returns(new List<LiabilityView>().AsEnumerable());

                _runControlService.GetMock<IMasterActualJobSumMachineReportRepository>().Setup(fn => fn.GetMachineReportDetail(request.JobGuid)).Returns(new SvdNoteWithdrawMachineReport());
                _runControlService.GetMock<IMasterActualJobSumActualCountRepository>().Setup(fn => fn.GetActualCountDetail(request.JobGuid)).Returns(new SvdNoteWithdrawActualCount());

                _runControlService.GetMock<IMasterActualJobSumCashAddRepository>().Setup(fn => fn.GetCashAddDetail(request.JobGuid)).Returns(new SvdNoteWithdrawCashAdd());
                _runControlService.GetMock<IMasterActualJobSumCashReturnRepository>().Setup(fn => fn.GetCashReuturnDetail(request.JobGuid)).Returns(new SvdNoteWithdrawCashReturn());

                _runControlService.GetMock<IMasterActualJobMCSRecyclingMachineReportRepository>().Setup(fn => fn.GetRecyclingMachineReportWODispense(request.JobGuid)).Returns(new SvdRecyclingMachineReportWODispense());
                _runControlService.GetMock<IMasterActualJobMCSRecyclingActualCountRepository>().Setup(fn => fn.GetRecyclingActualCount(request.JobGuid)).Returns(new SvdRecyclingActualCount());

                _runControlService.GetMock<IMasterActualJobMCSRecyclingCashRecyclingRepository>().Setup(fn => fn.GetRecyclingCashRecycling(request.JobGuid)).Returns(new SvdRecyclingCashRecycling());

                _runControlService.GetMock<IMobileATMCheckListEERepository>().Setup(fn => fn.GetCheckListEE(request.JobGuid)).Returns(new List<CheckListItem>().AsEnumerable());
                _runControlService.GetMock<IMasterActualJobHeaderRepository>().Setup(fn => fn.FindById(request.JobGuid)).Returns(new TblMasterActualJobHeader());

                _runControlService.GetMock<IMasterActualJobHeaderRepository>().Setup(fn => fn.GetCitDeliveryView(request.JobGuid)).Returns(new List<CitDeliveryView>().AsEnumerable());

                var result = (TabServiceDetailView)_runControlService.InvokeMethod("GetTabSeviceDetail", request);
                Assert.NotNull(result);

                if (request.TabID == CashAddPropertiesTab.tabSVD_NoteWithdraw_CashAdd_CashReturn)
                {
                    Assert.NotNull(result.SVD_CashAdd);
                    Assert.NotNull(result.SVD_CashReturn);                    
                }

                if (request.TabID == CashAddPropertiesTab.tabSVD_NoteWithdraw_MachineReport_ActualCount)
                {
                    Assert.NotNull(result.SVD_MachineReport);
                    Assert.NotNull(result.SVD_ActualCount);
                }

                if (request.TabID == CashAddPropertiesTab.tabSVD_CITDelivery)
                {
                    Assert.NotNull(result.SVD_CITDelivery);
                }
            }
         
            #endregion

            #region SET

            #endregion
        }
    }
}
