using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Models;
using Bgt.Ocean.Service.Mapping.Mappers;
using Bgt.Ocean.Service.Messagings.AdhocService;
using Bgt.Ocean.Service.Messagings.RunControlService;
using Bgt.Ocean.Service.ModelViews.ActualJobHeader;
using Bgt.Ocean.Service.ModelViews.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using static Bgt.Ocean.Infrastructure.Util.EnumRoute;
using static Bgt.Ocean.Infrastructure.Util.EnumRun;
using Bgt.Ocean.Models.ActualJob;

namespace Bgt.Ocean.Service.Implementations.Adhoc
{
    public partial class AdhocService
    {


        //private readonly ImastermachineReport

        #region ## New Service JobType Cash Add
        private void CreateATMTransaction(CreateJobAdHocRequest request, bool isUpdate)
        {
            #region ## Declare  Variable   
            /*
             * 806 : Created ATM Withdraw transaction By Ocean Online.
             * 807 : Updated ATM Withdraw transaction By Ocean Online.
             */
            Guid customerLocationGuid = GetCustomerLocationByServiceJobType(request);
            Guid legGuid = GetJobLegByServiceJobType(request);
            Guid jobHead = request.AdhocJobHeaderView.JobGuid.GetValueOrDefault();
            int msgId = isUpdate ? RemoveOldATMTransaction(jobHead, legGuid) : 806;
            #endregion
            #region ## Capability 
            var cap = _masterActualJobHeaderCapabilityRepository.GetMachineCapability(customerLocationGuid);
            List<TblMasterActualJobHeader_Capability> capHead = new List<TblMasterActualJobHeader_Capability>();
            foreach (var c in cap)
            {
                capHead.Add(new TblMasterActualJobHeader_Capability
                {
                    Guid = Guid.NewGuid(),
                    MasterActualJobHeader_Guid = jobHead,
                    SystemMachineCapability_Guid = c.Guid
                });
            }
            _masterActualJobHeaderCapabilityRepository.CreateRange(capHead);
            #endregion

            // read country config sorting cassette.
            var isFlagOrderMCSAscending = _systemEnvironmentMasterCountryRepository.FindAppkeyValueByEnumAppkeyName(request.AdhocJobHeaderView.BrinkSiteGuid, EnumAppKey.FlagOrderMCSAscending);
            foreach (var c in cap.Select(s => s.CapabilityId).Distinct())
            {

                switch (c)
                {
                    case Capability.NoteWithdraw:
                        {
                            CreateNoteWithDraw(request, customerLocationGuid);
                            break;
                        }
                    case Capability.BulkNoteDeposit:
                        {
                            CreateBulkNoteDeposit(request, customerLocationGuid, isFlagOrderMCSAscending);
                            break;
                        }
                    case Capability.Recycling:
                        {
                            CreateRecyclingData(request, customerLocationGuid, isFlagOrderMCSAscending);
                            break;
                        }
                    case Capability.CoinExchange:
                        {
                            CreateCoinExchange(request, customerLocationGuid, isFlagOrderMCSAscending);
                            break;
                        }
                }
            }
            #region ## History
            var insertHistory = new TblMasterHistory_ActualJob
            {
                Guid = Guid.NewGuid(),
                MasterActualJobHeader_Guid = request.AdhocJobHeaderView.JobGuid.GetValueOrDefault(), //headDetail.AdhocJobHeaderView.JobGuid.GetValueOrDefault(),
                MsgID = msgId, //Created job no. {0} date {1} from Adhoc Job by Ocean Online MVC.
                MsgParameter = "",
                UserCreated = request.UserName,
                DatetimeCreated = request.ClientDateTime.DateTime,
                UniversalDatetimeCreated = request.UniversalDatetime
            };
            _masterHistoryActualJobRepository.Create(insertHistory);
            #endregion
        }

        private void CreateCoinExchange(CreateJobAdHocRequest request, Guid customerLocationGuid, bool isFlagOrderMCSAscending)
        {
            CassetteType[] cassetteTypeId = { CassetteType.Hopper, CassetteType.Normal, CassetteType.Coin, CassetteType.AllIn };
            IEnumerable<DenominationOnMachineCassetteView> cassettes = _sfoMasterMachineCassetteRepository.Func_GetCassetteInMachine(customerLocationGuid, cassetteTypeId)?.ToList();

            if (cassettes.Any())
            {
                if (isFlagOrderMCSAscending)
                {
                    cassettes = cassettes.OrderBy(o => o.CassetteTypeID).ThenBy(d => d.DeNoValue).Select(((o, i) => { o.Seq = (i + 1); return o; }));
                }

                /* 
                 * ====== Set ====== 
                 * MCSCoinMachineBalance
                 * MCSCoinCashAdd
                 * MCSCoinCashReturn                 
                 */
                var normalCassette = cassettes.Where(w => w.CassetteTypeID != CassetteType.AllIn);
                if (normalCassette.Any())
                {
                    PrepareCoinExchangeBalanceCashAddCashReturn(request, normalCassette);
                }
                /* 
                 * ====== Set ======
                 * TblMasterActualJobMCSCoinBulkNoteCollect
                 * TblMasterActualJobMCSCoinSuspectFake
                 */
                var allINCassette = GetCassetteAllIn(customerLocationGuid, cassettes.Where(w => w.CassetteTypeID == CassetteType.AllIn), Capability.CoinExchange)?.FirstOrDefault();
                if (allINCassette != null)
                {
                    PrepareCoinExchangeBulkNoteAnduspectFake(request, allINCassette);
                }
            }

        }

        private void PrepareCoinExchangeBulkNoteAnduspectFake(CreateJobAdHocRequest request, DenominationOnMachineCassetteView currency)
        {
            if (currency != null)
            {
                TblMasterActualJobMCSCoinBulkNoteCollect coinBulkNoteHeader = new TblMasterActualJobMCSCoinBulkNoteCollect()
                {
                    Guid = Guid.NewGuid(),
                    MasterActualJobHeader_Guid = request.AdhocJobHeaderView.JobGuid.Value,
                    MasterActualJobServiceStopLegs_Guid = GetJobLegByServiceJobType(request),
                    MasterCurrency_Guid = currency.CurrencyGuid,
                    CurrencyAbbr = currency.CurrencyAbb,
                    UserCreated = request.UserName,
                    DatetimeCreated = request.ClientDateTime.DateTime,
                    UniversalDatetimeCreated = request.UniversalDatetime
                };
                TblMasterActualJobMCSCoinSuspectFake coinSuspectFakeHeader = new TblMasterActualJobMCSCoinSuspectFake()
                {
                    Guid = Guid.NewGuid(),
                    MasterActualJobHeader_Guid = request.AdhocJobHeaderView.JobGuid.Value,
                    MasterActualJobServiceStopLegs_Guid = GetJobLegByServiceJobType(request),
                    MasterCurrency_Guid = currency.CurrencyGuid,
                    CurrencyAbbr = currency.CurrencyAbb,
                    UserCreated = request.UserName,
                    DatetimeCreated = request.ClientDateTime.DateTime,
                    UniversalDatetimeCreated = request.UniversalDatetime
                };
                var deno = _masterDenominationRepository.GetDenoByCurrency(currency.CurrencyGuid, DenominationType.Note, DenominationUnit.Note);
                foreach (var d in deno)
                {
                    var coinBulkNote = new TblMasterActualJobMCSCoinBulkNoteCollectEntry()
                    {
                        Guid = Guid.NewGuid(),
                        MasterCurrency_Guid = d.MasterCurrency_Guid.GetValueOrDefault(),
                        CurrencyAbbr = currency.CurrencyAbb,
                        MasterDenomination_Guid = d.Guid,
                        DenominationValue = (decimal)d.DenominationValue
                    };
                    coinBulkNoteHeader.TblMasterActualJobMCSCoinBulkNoteCollectEntry.Add(coinBulkNote);
                    var coinSuspectFake = new TblMasterActualJobMCSCoinSuspectFakeEntry()
                    {
                        Guid = Guid.NewGuid(),
                        MasterCurrency_Guid = d.MasterCurrency_Guid.GetValueOrDefault(),
                        CurrencyAbbr = currency.CurrencyAbb,
                        MasterDenomination_Guid = d.Guid,
                        DenominationValue = (decimal)d.DenominationValue
                    };
                    coinSuspectFakeHeader.TblMasterActualJobMCSCoinSuspectFakeEntry.Add(coinSuspectFake);
                }
                _masterActualJobMCSCoinBulkNoteCollectRepository.Create(coinBulkNoteHeader);
                _masterActualJobMCSCoinSuspectFakeRepository.Create(coinSuspectFakeHeader);
            }
        }

        private void PrepareCoinExchangeBalanceCashAddCashReturn(CreateJobAdHocRequest request, IEnumerable<DenominationOnMachineCassetteView> cassette)
        {
            var currency = cassette.FirstOrDefault(f => f.CassetteTypeID != CassetteType.AllIn);
            if (currency != null)
            {
                TblMasterActualJobMCSCoinMachineBalance balanceHeader = new TblMasterActualJobMCSCoinMachineBalance
                {
                    Guid = Guid.NewGuid(),
                    MasterActualJobHeader_Guid = request.AdhocJobHeaderView.JobGuid.Value,
                    MasterActualJobServiceStopLegs_Guid = GetJobLegByServiceJobType(request),
                    MasterCurrency_Guid = currency.CurrencyGuid,
                    CurrencyAbbr = currency.CurrencyAbb,
                    UserCreated = request.UserName,
                    DatetimeCreated = request.ClientDateTime.DateTime,
                    UniversalDatetimeCreated = request.UniversalDatetime
                };
                TblMasterActualJobMCSCoinCashAdd cashAddHeader = new TblMasterActualJobMCSCoinCashAdd()
                {
                    Guid = Guid.NewGuid(),
                    MasterActualJobHeader_Guid = request.AdhocJobHeaderView.JobGuid.Value,
                    MasterActualJobServiceStopLegs_Guid = GetJobLegByServiceJobType(request),
                    MasterCurrency_Guid = currency.CurrencyGuid,
                    CurrencyAbbr = currency.CurrencyAbb,
                    UserCreated = request.UserName,
                    DatetimeCreated = request.ClientDateTime.DateTime,
                    UniversalDatetimeCreated = request.UniversalDatetime
                };
                TblMasterActualJobMCSCoinCashReturn cashReturnHeader = new TblMasterActualJobMCSCoinCashReturn()
                {
                    Guid = Guid.NewGuid(),
                    MasterActualJobHeader_Guid = request.AdhocJobHeaderView.JobGuid.Value,
                    MasterActualJobServiceStopLegs_Guid = GetJobLegByServiceJobType(request),
                    MasterCurrency_Guid = currency.CurrencyGuid,
                    CurrencyAbbr = currency.CurrencyAbb,
                    UserCreated = request.UserName,
                    DatetimeCreated = request.ClientDateTime.DateTime,
                    UniversalDatetimeCreated = request.UniversalDatetime
                };
                foreach (var d in cassette)
                {
                    var balance = new TblMasterActualJobMCSCoinMachineBalanceEntry
                    {
                        Guid = Guid.NewGuid(),
                        CassetteSequence = d.Seq,
                        MasterCurrency_Guid = d.CurrencyGuid,
                        CurrencyAbbr = d.CurrencyAbb,
                        DenominationValue = (decimal)d.DeNoValue,
                        MasterDenomination_Guid = d.DeNoGuid,
                        CassetteTypeID = (int)d.CassetteTypeID,
                        SystemCassetteType_Guid = d.CassetteTypeIdGuid,
                        HopperValue = d.CassetteTypeID == CassetteType.Hopper ? (decimal)d.DeNoValue.GetValueOrDefault() * d.Amount.GetValueOrDefault() : 0
                    };
                    balanceHeader.TblMasterActualJobMCSCoinMachineBalanceEntry.Add(balance);
                    var cashAdd = new TblMasterActualJobMCSCoinCashAddEntry
                    {
                        Guid = Guid.NewGuid(),
                        CassetteSequence = d.Seq,
                        MasterCurrency_Guid = d.CurrencyGuid,
                        CurrencyAbbr = d.CurrencyAbb,
                        DenominationValue = (decimal)d.DeNoValue,
                        MasterDenomination_Guid = d.DeNoGuid,
                        SystemCassetteType_Guid = d.CassetteTypeIdGuid,
                        CassetteTypeID = (int)d.CassetteTypeID,
                        HopperValue = d.CassetteTypeID == CassetteType.Hopper ? (decimal)d.DeNoValue.GetValueOrDefault() * d.Amount.GetValueOrDefault() : 0
                    };
                    cashAddHeader.TblMasterActualJobMCSCoinCashAddEntry.Add(cashAdd);

                    //if (d.CassetteTypeID != CassetteType.Coin)
                    //{
                        var cahsReturn = new TblMasterActualJobMCSCoinCashReturnEntry
                        {
                            Guid = Guid.NewGuid(),
                            CassetteSequence = d.Seq,
                            MasterCurrency_Guid = d.CurrencyGuid,
                            CurrencyAbbr = d.CurrencyAbb,
                            DenominationValue = (decimal)d.DeNoValue,
                            MasterDenomination_Guid = d.DeNoGuid,
                            SystemCassetteType_Guid = d.CassetteTypeIdGuid,
                            CassetteTypeID = (int)d.CassetteTypeID,
                            HopperValue = d.CassetteTypeID == CassetteType.Hopper ? (decimal)d.DeNoValue.GetValueOrDefault() * d.Amount.GetValueOrDefault() : 0
                        };
                        cashReturnHeader.TblMasterActualJobMCSCoinCashReturnEntry.Add(cahsReturn);
                    //}
                }
                _masterActualJobMCSCoinMachineBalanceRepository.Create(balanceHeader);
                _masterActualJobMCSCoinCashAddRepository.Create(cashAddHeader);
                _masterActualJobMCSCoinCashReturnRepository.Create(cashReturnHeader);
            }
        }




        #region Recycling
        private void CreateRecyclingData(CreateJobAdHocRequest request, Guid customerLocationGuid, bool isFlagOrderMCSAscending)
        {
            IEnumerable<DenominationOnMachineCassetteView> denoInCassette = null;
            if (isFlagOrderMCSAscending)
            {
                denoInCassette = _sfoMasterMachineCassetteRepository.Func_GetDenoOnMachineCassetteByCustomerLocationGuid(customerLocationGuid, CassetteType.Normal)?
                    .OrderBy(d => d.DeNoValue).Select(((o, i) => { o.Seq = (i + 1); return o; })).ToList();
            }
            else
            {
                denoInCassette = _sfoMasterMachineCassetteRepository.Func_GetDenoOnMachineCassetteByCustomerLocationGuid(customerLocationGuid, CassetteType.Normal)?.ToList();
            }
            if (denoInCassette.Any())
            {
                var currency = denoInCassette.First();
                var jobHead = request.AdhocJobHeaderView.JobGuid.Value;
                var legGuid = GetJobLegByServiceJobType(request);
                TblMasterActualJobMCSRecyclingMachineReport recyReportHead = new TblMasterActualJobMCSRecyclingMachineReport()
                {
                    Guid = Guid.NewGuid(),
                    MasterActualJobHeader_Guid = jobHead,
                    MasterActualJobServiceStopLegs_Guid = legGuid,
                    MasterCurrency_Guid = currency.CurrencyGuid,
                    CurrencyAbbr = currency.CurrencyAbb,
                    UserCreated = request.UserName,
                    DatetimeCreated = request.ClientDateTime.DateTime,
                    UniversalDatetimeCreated = request.UniversalDatetime
                };
                TblMasterActualJobMCSRecyclingActualCount recyAcCountHead = new TblMasterActualJobMCSRecyclingActualCount()
                {
                    Guid = Guid.NewGuid(),
                    MasterActualJobHeader_Guid = jobHead,
                    MasterActualJobServiceStopLegs_Guid = legGuid,
                    MasterCurrency_Guid = currency.CurrencyGuid,
                    CurrencyAbbr = currency.CurrencyAbb,
                    UserCreated = request.UserName,
                    DatetimeCreated = request.ClientDateTime.DateTime,
                    UniversalDatetimeCreated = request.UniversalDatetime
                };
                TblMasterActualJobMCSRecyclingCashRecycling recyCashHead = new TblMasterActualJobMCSRecyclingCashRecycling()
                {
                    Guid = Guid.NewGuid(),
                    MasterActualJobHeader_Guid = jobHead,
                    MasterActualJobServiceStopLegs_Guid = legGuid,
                    MasterCurrency_Guid = currency.CurrencyGuid,
                    CurrencyAbbr = currency.CurrencyAbb,
                    UserCreated = request.UserName,
                    DatetimeCreated = request.ClientDateTime.DateTime,
                    UniversalDatetimeCreated = request.UniversalDatetime
                };
                foreach (var d in denoInCassette.OrderBy(o => isFlagOrderMCSAscending ? o.DeNoValue : o.Seq))
                {
                    TblMasterActualJobMCSRecyclingMachineReportEntry recyReport = new TblMasterActualJobMCSRecyclingMachineReportEntry()
                    {
                        Guid = Guid.NewGuid(),
                        CassetteSequence = d.Seq,
                        MasterCurrency_Guid = d.CurrencyGuid,
                        CurrencyAbbr = d.CurrencyAbb,
                        DenominationValue = (decimal)d.DeNoValue,
                        MasterDenomination_Guid = d.DeNoGuid,
                    };
                    recyReportHead.TblMasterActualJobMCSRecyclingMachineReportEntry.Add(recyReport);

                    TblMasterActualJobMCSRecyclingActualCountEntry recyAcCount = new TblMasterActualJobMCSRecyclingActualCountEntry()
                    {
                        Guid = Guid.NewGuid(),
                        CassetteSequence = d.Seq,
                        MasterCurrency_Guid = d.CurrencyGuid,
                        CurrencyAbbr = d.CurrencyAbb,
                        DenominationValue = (decimal)d.DeNoValue,
                        MasterDenomination_Guid = d.DeNoGuid,
                    };
                    recyAcCountHead.TblMasterActualJobMCSRecyclingActualCountEntry.Add(recyAcCount);
                    TblMasterActualJobMCSRecyclingCashRecyclingEntry recyCash = new TblMasterActualJobMCSRecyclingCashRecyclingEntry()
                    {
                        Guid = Guid.NewGuid(),
                        CassetteSequence = d.Seq,
                        MasterCurrency_Guid = d.CurrencyGuid,
                        CurrencyAbbr = d.CurrencyAbb,
                        DenominationValue = (decimal)d.DeNoValue,
                        MasterDenomination_Guid = d.DeNoGuid,
                    };
                    recyCashHead.TblMasterActualJobMCSRecyclingCashRecyclingEntry.Add(recyCash);
                }

                _masterActualJobMCSRecyclingCashRecyclingRepository.Create(recyCashHead);
                _masterActualJobMCSRecyclingActualCountRepository.Create(recyAcCountHead);
                _masterActualJobMCSRecyclingMachineReportRepository.Create(recyReportHead);
            }

        }

        #endregion Recycling
        #region CreateBulkNote
        /// <summary>
        /// Prepare data of Bulk note deposit capabillity;
        /// </summary>
        /// <param name="request"></param>
        /// <param name="customerLocationGuid"></param>
        /// <param name="isFlagOrderMCSAscending"></param>
        private void CreateBulkNoteDeposit(CreateJobAdHocRequest request, Guid customerLocationGuid, bool isFlagOrderMCSAscending)
        {
            // find deno in cassette as cassette type All-in.
            var data = _sfoMasterMachineCassetteRepository.Func_GetDenoOnMachineCassetteByCustomerLocationGuid(customerLocationGuid, CassetteType.AllIn);
            var denoInCassette = GetCassetteAllIn(customerLocationGuid, data, Capability.BulkNoteDeposit);
            if (denoInCassette.Any())
            {
                var currency = denoInCassette.FirstOrDefault();
                var preRequest = new MachineCashServiceRequest()
                {
                    JobHeaderGuid = request.AdhocJobHeaderView.JobGuid.GetValueOrDefault(),
                    JobLegGuid = GetJobLegByServiceJobType(request),

                    Cassette = denoInCassette,
                    CurrencyGuid = currency.CurrencyGuid,
                    CurrencyAbb = currency.CurrencyAbb,
                    IsAscSequence = isFlagOrderMCSAscending
                };

                // Deposit Collection Report
                var depositCollect = PrePareDataBulkCassette(preRequest);
                _masterActualJobMCSBulkDepositReportRepository.Create(depositCollect);
                //Retract,SupspectFake,Jammed
                var deno = PrePareDataBulkDenomination(preRequest);
                if (deno != null)
                {
                    _masterActualJobMCSBulkSuspectFakeRepository.Create(deno.SuspectFake);
                    _masterActualJobMCSBulkJammedRepository.Create(deno.Jammet);
                    _masterActualJobMCSBulkRetractRepository.Create(deno.Retract);
                }
            }
        }
        private IEnumerable<DenominationOnMachineCassetteView> GetCassetteAllIn(Guid machineGuid, IEnumerable<DenominationOnMachineCassetteView> denoInCassette, Capability capId)
        {
            if (!denoInCassette.Any())
            {
                denoInCassette = _sfoMasterMachineCassetteRepository.Func_GetDefaultCassetteInModelMachine(machineGuid, CassetteType.AllIn, capId);
            }
            return denoInCassette;
        }
        /// <summary>
        /// Prepare data cassette type All-in.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private TblMasterActualJobMCSBulkDepositReport PrePareDataBulkCassette(MachineCashServiceRequest request)
        {
            TblMasterActualJobMCSBulkDepositReport depositReport = new TblMasterActualJobMCSBulkDepositReport()
            {
                Guid = Guid.NewGuid(),
                MasterActualJobHeader_Guid = request.JobHeaderGuid,
                MasterActualJobServiceStopLegs_Guid = request.JobLegGuid,
                MasterCurrency_Guid = request.CurrencyGuid,
                CurrencyAbbr = request.CurrencyAbb,
                UserCreated = request.UserName,
                DatetimeCreated = request.ClientDateTime.DateTime,
                UniversalDatetimeCreated = request.UniversalDatetime
            };
            var cap = request.Cassette.Select(s => new { s.CasseteName, s.Seq, s.MachineCassetteGuid }).Distinct();
            if (!request.IsAscSequence)
            {
                foreach (var c in cap)
                {
                    TblMasterActualJobMCSBulkDepositReportEntry depositReportEntry = new TblMasterActualJobMCSBulkDepositReportEntry()
                    {
                        Guid = Guid.NewGuid(),
                        MasterMCSBulkDepositReport_Guid = depositReport.Guid,
                        SFOTblMasterCassette_Guid = c.MachineCassetteGuid,
                        CassetteName = c.CasseteName,
                        CassetteSequence = c.Seq
                    };
                    depositReport.TblMasterActualJobMCSBulkDepositReportEntry.Add(depositReportEntry);
                }
            }
            else
            {
                int cassetteSeq = 1;
                foreach (var c in cap.OrderBy(o => o.CasseteName))
                {
                    TblMasterActualJobMCSBulkDepositReportEntry depositReportEntry = new TblMasterActualJobMCSBulkDepositReportEntry()
                    {
                        Guid = Guid.NewGuid(),
                        MasterMCSBulkDepositReport_Guid = depositReport.Guid,
                        CassetteName = c.CasseteName,
                        SFOTblMasterCassette_Guid = c.MachineCassetteGuid,
                        CassetteSequence = cassetteSeq
                    };
                    depositReport.TblMasterActualJobMCSBulkDepositReportEntry.Add(depositReportEntry);
                    cassetteSeq++;
                }
            }
            return depositReport;
        }

        /// <summary>
        /// Prepare data Denomination in cassette type All-in.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private PrepareBulkDenomination PrePareDataBulkDenomination(MachineCashServiceRequest request)
        {
            PrepareBulkDenomination result = null;
            var deno = _masterDenominationRepository.GetDenoByCurrency(request.CurrencyGuid, DenominationType.Note, DenominationUnit.Note)?.ToList();

            if (deno != null && deno.Any())
            {
                var retract = new TblMasterActualJobMCSBulkRetract
                {
                    Guid = Guid.NewGuid(),
                    MasterActualJobHeader_Guid = request.JobHeaderGuid,
                    MasterActualJobServiceStopLegs_Guid = request.JobLegGuid,
                    MasterCurrency_Guid = request.CurrencyGuid,
                    CurrencyAbbr = request.CurrencyAbb,
                    UserCreated = request.UserName,
                    DatetimeCreated = request.ClientDateTime.DateTime,
                    UniversalDatetimeCreated = request.UniversalDatetime
                };

                var suspectFake = new TblMasterActualJobMCSBulkSuspectFake
                {
                    Guid = Guid.NewGuid(),
                    MasterActualJobHeader_Guid = request.JobHeaderGuid,
                    MasterActualJobServiceStopLegs_Guid = request.JobLegGuid,
                    MasterCurrency_Guid = request.CurrencyGuid,
                    CurrencyAbbr = request.CurrencyAbb,
                    UserCreated = request.UserName,
                    DatetimeCreated = request.ClientDateTime.DateTime,
                    UniversalDatetimeCreated = request.UniversalDatetime
                };
                var jammed = new TblMasterActualJobMCSBulkJammed
                {
                    Guid = Guid.NewGuid(),
                    MasterActualJobHeader_Guid = request.JobHeaderGuid,
                    MasterActualJobServiceStopLegs_Guid = request.JobLegGuid,
                    MasterCurrency_Guid = request.CurrencyGuid,
                    CurrencyAbbr = request.CurrencyAbb,
                    UserCreated = request.UserName,
                    DatetimeCreated = request.ClientDateTime.DateTime,
                    UniversalDatetimeCreated = request.UniversalDatetime
                };

                foreach (var d in deno)
                {
                    var retractEntry = new TblMasterActualJobMCSBulkRetractEntry
                    {
                        Guid = Guid.NewGuid(),
                        MasterCurrency_Guid = request.CurrencyGuid,
                        CurrencyAbbr = request.CurrencyAbb,
                        MasterDenomination_Guid = d.Guid,
                        DenominationValue = (decimal)d.DenominationValue,
                    };
                    retract.TblMasterActualJobMCSBulkRetractEntry.Add(retractEntry);

                    var suspectFakeEntry = new TblMasterActualJobMCSBulkSuspectFakeEntry
                    {
                        Guid = Guid.NewGuid(),
                        MasterCurrency_Guid = request.CurrencyGuid,
                        CurrencyAbbr = request.CurrencyAbb,
                        MasterDenomination_Guid = d.Guid,
                        DenominationValue = (decimal)d.DenominationValue,

                    };
                    suspectFake.TblMasterActualJobMCSBulkSuspectFakeEntry.Add(suspectFakeEntry);

                    var jammedEntry = new TblMasterActualJobMCSBulkJammedEntry
                    {
                        Guid = Guid.NewGuid(),
                        MasterCurrency_Guid = request.CurrencyGuid,
                        CurrencyAbbr = request.CurrencyAbb,
                        MasterDenomination_Guid = d.Guid,
                        DenominationValue = (decimal)d.DenominationValue,
                    };
                    jammed.TblMasterActualJobMCSBulkJammedEntry.Add(jammedEntry);
                }

                result = new PrepareBulkDenomination()
                {
                    Retract = retract,
                    Jammet = jammed,
                    SuspectFake = suspectFake
                };
            }
            return result;
        }


        #endregion

        #region CreateNoteWithDraw
        private void CreateNoteWithDraw(CreateJobAdHocRequest request, Guid customerLocationGuid)
        {
            // use more than one     
            var denoInCassette = _sfoMasterMachineCassetteRepository.Func_GetDenoOnMachineCassetteByCustomerLocationGuid(customerLocationGuid, CassetteType.Normal).ToList();
            if (denoInCassette.Any())
            {
                var cashHead = SetHeaderCashAdd(request, denoInCassette.FirstOrDefault().CurrencyGuid);
                TblMasterActualJobSumActualCount headAc = cashHead.TbSumAc;
                TblMasterActualJobSumCashAdd headCa = cashHead.TbSumCa;
                TblMasterActualJobSumMachineReport headSumMr = cashHead.TbSumMr;
                TblMasterActualJobSumCashReturn headSumCr = cashHead.TbSumCr;

                #region ## Cassette Screen ##
                var cassetteData = denoInCassette.Select(s => new CassetteModelView
                {
                    Guid = Guid.NewGuid(),
                    MasterCurrency_Guid = s.CurrencyGuid,
                    CurrencyAbbr = s.CurrencyAbb,
                    MasterDenomination_Guid = s.DeNoGuid,
                    CassetteSequence = s.Seq,
                    DenominationValue = (decimal)(s.DeNoValue ?? 0),
                    UserCreated = request.UserName,
                    DatetimeCreated = request.ClientDateTime.DateTime,
                    UniversalDatetimeCreated = request.UniversalDatetime
                });


                cassetteData = OrderCassetteByFlagOrderMCSAscending(cassetteData, request.AdhocJobHeaderView.BrinkSiteGuid);


                var tblAc = cassetteData.ConvertToAtmCashAdd<TblMasterActualJobActualCount>(headAc.Guid);
                _masterActualJobSumActualCountRepository.Create(headAc);
                _masterActualJobActualCountRepository.CreateRange(tblAc);

                var tblCa = cassetteData.ConvertToAtmCashAdd<TblMasterActualJobCashAdd>(headCa.Guid);
                _masterActualJobSumCashAddRepository.Create(headCa);
                _masterActualJobCashAddRepository.CreateRange(tblCa);

                var tblMr = cassetteData.ConvertToAtmCashAdd<TblMasterActualJobMachineReport>(headSumMr.Guid);
                _masterActualJobSumMachineReportRepository.Create(headSumMr);
                _masterActualJobMachineReportRepository.CreateRange(tblMr);
                #endregion
                #region ## Deno Screen
                // Deno in currency
                var currency = denoInCassette.FirstOrDefault();
                var deno = _masterDenominationRepository.GetDenoByCurrency(currency.CurrencyGuid, DenominationType.Note, DenominationUnit.Note)
                                        .Select(s => new TblMasterActualJobCashReturn
                                        {
                                            Guid = Guid.NewGuid(),
                                            MasterActualJobSumCashReturn_Guid = headSumCr.Guid,
                                            MasterCurrency_Guid = s.MasterCurrency_Guid.GetValueOrDefault(),
                                            CurrencyAbbr = currency.CurrencyAbb,
                                            MasterDenomination_Guid = s.Guid,
                                            DenominationValue = (decimal)(s.DenominationValue),
                                            UserCreated = request.UserName,
                                            DatetimeCreated = request.ClientDateTime.DateTime,
                                            UniversalDatetimeCreated = request.UniversalDatetime
                                        }).ToList();
                _masterActualJobSumCashReturnRepository.Create(headSumCr);
                _masterActualJobCashReturnRepository.CreateRange(deno);
                #endregion           
            }
        }

        private IEnumerable<CassetteModelView> OrderCassetteByFlagOrderMCSAscending(IEnumerable<CassetteModelView> cassetteData, Guid BrinkSiteGuid)
        {
            var FlagOrderMCSAscending = _systemEnvironmentMasterCountryRepository.FindAppkeyValueByEnumAppkeyName(BrinkSiteGuid, EnumAppKey.FlagOrderMCSAscending);

            //ACS
            if (FlagOrderMCSAscending)
                cassetteData = cassetteData.OrderBy(o => o.DenominationValue).Select((o, i) => { o.CassetteSequence = (i + 1); return o; });

            //DESC
            //cassetteData.OrderByDescending(o => o.DenominationValue).Select((o, i) => { o.CassetteSequence = i; return o; })

            return cassetteData.ToList();
        }
        #endregion
        public CreateJobAdHocResponse CheckMachineAssociate(CreateJobAdHocRequest request)
        {
            var cuslo = GetCustomerLocationByServiceJobType(request);
            var machine = _sfoMasterMachineRepository.CheckAssociateMachine(cuslo);
            TblSystemMessage msg = new TblSystemMessage();
            if (request.AdhocJobHeaderView.ServiceJobTypeID == IntTypeJob.MCS && !machine)
            {
                msg = _systemMessageRepository.FindByMsgId(-765, request.LanguagueGuid);
                return new CreateJobAdHocResponse(msg)
                {
                    IsWarning = true,
                    JobGuid = request.AdhocJobHeaderView.JobGuid,
                    JobNo = request.AdhocJobHeaderView.JobNo
                };


            }
            else
            {
                return new CreateJobAdHocResponse(msg)
                {
                    IsWarning = false,
                    JobGuid = request.AdhocJobHeaderView.JobGuid,
                    JobNo = request.AdhocJobHeaderView.JobNo
                };

            }




        }
        private int RemoveOldATMTransaction(Guid jobGuid, Guid jobLeg)
        {
            //## Remove capability 
            var cap = _masterActualJobHeaderCapabilityRepository.FindAll(f => f.MasterActualJobHeader_Guid == jobGuid);
            _masterActualJobHeaderCapabilityRepository.RemoveRange(cap);
            //## Remove Cash-add.
            var cashAddDetail = _masterActualJobCashAddRepository.FindAll(f => f.TblMasterActualJobSumCashAdd.MasterActualJobHeader_Guid == jobGuid && f.TblMasterActualJobSumCashAdd.MasterActualJobServiceStopLegs_Guid == jobLeg).ToList();
            _masterActualJobCashAddRepository.RemoveRange(cashAddDetail);
            var cashAddHeader = _masterActualJobSumCashAddRepository.FindAll(f => f.MasterActualJobHeader_Guid == jobGuid && f.MasterActualJobServiceStopLegs_Guid == jobLeg);
            _masterActualJobSumCashAddRepository.RemoveRange(cashAddHeader);

            //## Remove Actual count.
            var actualCount = _masterActualJobActualCountRepository.FindAll(f => f.TblMasterActualJobSumActualCount.MasterActualJobHeader_Guid == jobGuid && f.TblMasterActualJobSumActualCount.MasterActualJobServiceStopLegs_Guid == jobLeg);
            _masterActualJobActualCountRepository.RemoveRange(actualCount);
            var actualCountHeader = _masterActualJobSumActualCountRepository.FindAll(f => f.MasterActualJobHeader_Guid == jobGuid && f.MasterActualJobServiceStopLegs_Guid == jobLeg);
            _masterActualJobSumActualCountRepository.RemoveRange(actualCountHeader);

            //## Remove Cash return.
            var cashReturn = _masterActualJobCashReturnRepository.FindAll(f => f.TblMasterActualJobSumCashReturn.MasterActualJobHeader_Guid == jobGuid && f.TblMasterActualJobSumCashReturn.MasterActualJobServiceStopLegs_Guid == jobLeg);
            _masterActualJobCashReturnRepository.RemoveRange(cashReturn);
            var cashReturnHeader = _masterActualJobSumCashReturnRepository.FindAll(f => f.MasterActualJobHeader_Guid == jobGuid && f.MasterActualJobServiceStopLegs_Guid == jobLeg);
            _masterActualJobSumCashReturnRepository.RemoveRange(cashReturnHeader);

            //## Remove Machine report.
            var machineReport = _masterActualJobMachineReportRepository.FindAll(f => f.TblMasterActualJobSumMachineReport.MasterActualJobHeader_Guid == jobGuid && f.TblMasterActualJobSumMachineReport.MasterActualJobServiceStopLegs_Guid == jobLeg);
            _masterActualJobMachineReportRepository.RemoveRange(machineReport);
            var machineReportHeader = _masterActualJobSumMachineReportRepository.FindAll(f => f.MasterActualJobHeader_Guid == jobGuid && f.MasterActualJobServiceStopLegs_Guid == jobLeg);
            _masterActualJobSumMachineReportRepository.RemoveRange(machineReportHeader);
            return 807;//Updated ATM Withdraw transaction By Ocean Online.
        }
        private string GetLockModeByServiceJobTypeId(int serviceJobTypeID, Guid? countryGuid)
        {
            string lockMode = "R";
            var lockTb = _sfoMasterOTCLockModeRepository.FindAll(f => f.TblSystemServiceJobType.ServiceJobTypeID == serviceJobTypeID && f.SFOTblSystemOTCLockMode.MasterCountry_Guid == countryGuid).ToList();
            if (lockTb.Any())
            {
                lockMode = lockTb.FirstOrDefault().SFOTblSystemOTCLockMode.LockMode;
            }
            return lockMode;
        }
        private CashAddTransactionHeader SetHeaderCashAdd(CreateJobAdHocRequest request, Guid currencyGuid)
        {

            Guid legGuid = GetJobLegByServiceJobType(request);
            var rusult = new CashAddTransactionHeader()
            {
                TbSumAc = new TblMasterActualJobSumActualCount()
                {
                    Guid = Guid.NewGuid(),
                    MasterActualJobHeader_Guid = request.AdhocJobHeaderView.JobGuid.GetValueOrDefault(),
                    MasterActualJobServiceStopLegs_Guid = legGuid,
                    MasterCurrency_Guid = currencyGuid,
                    UserCreated = request.UserName,
                    DatetimeCreated = request.ClientDateTime.DateTime,
                    UniversalDatetimeCreated = request.UniversalDatetime
                },
                TbSumCa = new TblMasterActualJobSumCashAdd()
                {
                    Guid = Guid.NewGuid(),
                    MasterActualJobHeader_Guid = request.AdhocJobHeaderView.JobGuid.GetValueOrDefault(),
                    MasterActualJobServiceStopLegs_Guid = legGuid,
                    MasterCurrency_Guid = currencyGuid,
                    UserCreated = request.UserName,
                    DatetimeCreated = request.ClientDateTime.DateTime,
                    UniversalDatetimeCreated = request.UniversalDatetime
                },
                TbSumCr = new TblMasterActualJobSumCashReturn()
                {
                    Guid = Guid.NewGuid(),
                    MasterActualJobHeader_Guid = request.AdhocJobHeaderView.JobGuid.GetValueOrDefault(),
                    MasterActualJobServiceStopLegs_Guid = legGuid,
                    MasterCurrency_Guid = currencyGuid,
                    UserCreated = request.UserName,
                    DatetimeCreated = request.ClientDateTime.DateTime,
                    UniversalDatetimeCreated = request.UniversalDatetime
                },
                TbSumMr = new TblMasterActualJobSumMachineReport()
                {
                    Guid = Guid.NewGuid(),
                    MasterActualJobHeader_Guid = request.AdhocJobHeaderView.JobGuid.GetValueOrDefault(),
                    MasterActualJobServiceStopLegs_Guid = legGuid,
                    MasterCurrency_Guid = currencyGuid,
                    UserCreated = request.UserName,
                    DatetimeCreated = request.ClientDateTime.DateTime,
                    UniversalDatetimeCreated = request.UniversalDatetime
                }

            };
            return rusult;
        }


        /// <summary>
        /// Support only single customer.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private Guid GetJobLegByServiceJobType(CreateJobAdHocRequest request)
        {

            Guid legGuid;
            switch (request.AdhocJobHeaderView.ServiceJobTypeID)
            {
                /*Set P*/
                case IntTypeJob.P:
                case IntTypeJob.BCP:
                case IntTypeJob.MCS:
                    {
                        if (request.ServiceStopLegPickup.LegGuid.IsNullOrEmpty())
                        {
                            legGuid = ActualJobLegBySeqStop(request.AdhocJobHeaderView.JobGuid.GetValueOrDefault(), 1).Guid;
                        }
                        else
                        {
                            legGuid = request.ServiceStopLegPickup.LegGuid.GetValueOrDefault();
                        }
                        break;
                    }
                /*Set D*/
                case IntTypeJob.D:
                case IntTypeJob.AC:
                case IntTypeJob.AE:
                    {
                        if (request.ServiceStopLegDelivery.LegGuid.IsNullOrEmpty())
                        {
                            legGuid = ActualJobLegBySeqStop(request.AdhocJobHeaderView.JobGuid.GetValueOrDefault(), 2).Guid;
                        }
                        else
                        {
                            legGuid = request.ServiceStopLegDelivery.LegGuid.GetValueOrDefault();
                        }
                        break;
                    }

                default:
                    {
                        legGuid = request.ServiceStopLegPickup.LegGuid.GetValueOrDefault(); break;
                    }
            }
            return legGuid;
        }

        private Guid GetCustomerLocationByServiceJobType(CreateJobAdHocRequest request)
        {

            Guid customerLocationGuid;
            switch (request.AdhocJobHeaderView.ServiceJobTypeID)
            {
                /*Set P*/
                case IntTypeJob.P:
                case IntTypeJob.BCP:
                case IntTypeJob.MCS:
                    {
                        if (request.ServiceStopLegPickup.LocationGuid.IsNullOrEmpty())
                        {
                            customerLocationGuid = ActualJobLegBySeqStop(request.AdhocJobHeaderView.JobGuid.GetValueOrDefault(), 1).MasterCustomerLocation_Guid.GetValueOrDefault();
                        }
                        else
                        {
                            customerLocationGuid = request.ServiceStopLegPickup.LocationGuid.GetValueOrDefault();
                        }
                        break;
                    }
                /*Set D*/
                case IntTypeJob.D:
                case IntTypeJob.AC:
                case IntTypeJob.AE:

                    {
                        if (request.ServiceStopLegDelivery.LocationGuid.IsNullOrEmpty())
                        {
                            customerLocationGuid = ActualJobLegBySeqStop(request.AdhocJobHeaderView.JobGuid.GetValueOrDefault(), 2).MasterCustomerLocation_Guid.GetValueOrDefault();
                        }
                        else
                        {
                            customerLocationGuid = request.ServiceStopLegDelivery.LocationGuid.GetValueOrDefault();
                        }
                        break;
                    }

                default: { customerLocationGuid = request.ServiceStopLegPickup.LocationGuid.GetValueOrDefault(); break; }
            }
            return customerLocationGuid;
        }
        private TblMasterActualJobServiceStopLegs ActualJobLegBySeqStop(Guid jobHeadGuid, int seqStop)
        {
            return _masterActualJobServiceStopLegsRepository.FindByJobHeader(jobHeadGuid).FirstOrDefault(f => f.SequenceStop == seqStop);
        }
        #endregion
        #region ##Joborder Function    
        public SystemMessageView UpdateJobOrderInRun(UpdateJobOrderInRunRequest request)
        {
            try
            {
                if (request.SiteGuid != Guid.Empty && request.RunDailyGuid != Guid.Empty)
                {

                    // update order
                    _masterActualJobHeaderRepository.Func_UpdateJobOrderInRunResource(request.RunDailyGuid, request.FlagReorder, request.MasterRouteGuid, request.WorkDate, request.SiteGuid, request.LanguageGuid, request.UserModified, request.ClientDateTime);

                    // check employee use dolphin
                    var isUseDolphin = _systemEnvironmentMasterCountryRepository.IsUseDolphin(request.SiteGuid, request.RunDailyGuid);
                    if (isUseDolphin)
                    {
                        // get run detail
                        var runDetail = _masterDailyRunResourceRepository.FindById(request.RunDailyGuid);
                        if (runDetail.RunResourceDailyStatusID == StatusDailyRun.DispatchRun)
                        {
                            // push to dolphin
                            DolphinReorderJobsRequest reOrderRequest = new DolphinReorderJobsRequest();
                            reOrderRequest.action = FixStringRoute.Reorder;
                            reOrderRequest.datetime = request.ClientDateTime.ChangeDateTimeForDolphin();
                            reOrderRequest.runResourceDailyGuid = request.RunDailyGuid;
                            reOrderRequest.userAction = request.UserModified;
                            var allLegInRun = _masterActualJobHeaderRepository.Func_GetJobAndJobLegInRun(request.RunDailyGuid)?.Select(s => new { legGuid = s.ServiceStopLegsGuid, jobHeaderGuid = s.Guid, s.JobOrder }).ToList();
                            reOrderRequest.reOrder = allLegInRun.Select(e => new ReorderJobsView()
                            {
                                legGuid = e.legGuid,
                                jobOrder = e.JobOrder
                            });

                            reOrderRequest.routeName = _masterRouteGroupDetailRepository.FindById(runDetail.MasterRouteGroup_Detail_Guid)?.MasterRouteGroupDetailName;
                            var success = _pushToDolphinService.PushToReorderJobs(reOrderRequest);
                            if (success)
                            {
                                // update status after push
                                UpdateStatusSyncToDolphinRequest updateStatus = new UpdateStatusSyncToDolphinRequest();
                                updateStatus.ClientDateTime = request.ClientDateTime.GetValueOrDefault();
                                updateStatus.UserModified = request.UserModified;
                                updateStatus.SyncStatusDolphin = 4;
                                updateStatus.ListJobGuid = allLegInRun.Select(e => new Guid?(e.jobHeaderGuid));
                                _pushToDolphinService.UpdateStatusSyncToDolphin(updateStatus);

                                // insert daily run history
                                DateTimeOffset date = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
                                TblMasterHistory_DailyRunResource historyRun = new TblMasterHistory_DailyRunResource()
                                {
                                    Guid = Guid.NewGuid(),
                                    MasterDailyRunResource_Guid = request.RunDailyGuid,
                                    MsgID = 824,
                                    MsgParameter = new string[] { request.UserModified }.ToJSONString(),
                                    UserCreated = request.UserModified,
                                    DatetimeCreated = request.ClientDateTime.GetValueOrDefault(),
                                    UniversalDatetimeCreated = date
                                };

                                using (var tran = _uow.BeginTransaction())
                                {
                                    _masterHistory_DailyRunResource.Create(historyRun);
                                    _uow.Commit();
                                    tran.Complete();
                                }
                            }
                        }
                    }
                }

                return _systemMessageRepository.FindByMsgId(0, request.LanguageGuid).ConvertToMessageView(true);
            }
            catch (Exception ex)
            {
                // OO error logger
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
                return _systemMessageRepository.FindByMsgId(-184, request.LanguageGuid).ConvertToMessageView();
            }
        }

        public void UpdatePushToDolPhinWhenCreateJob(UpdateJobOrderInRunRequest request)
        {
            var isUseDolphin = _systemEnvironmentMasterCountryRepository.IsUseDolphin(request.SiteGuid, request.RunDailyGuid);
            if (isUseDolphin)
            {
                var runDetail = _masterDailyRunResourceRepository.FindById(request.RunDailyGuid);

                bool isDispatch = runDetail?.RunResourceDailyStatusID.GetValueOrDefault() == StatusDailyRun.DispatchRun;
                if (isDispatch)
                {
                    DolphinCreateJobsRequest pushToDolphinRequest = new DolphinCreateJobsRequest();
                    pushToDolphinRequest.action = FixMessageRoute.create;
                    pushToDolphinRequest.jobGuid = request.JobHeadGuidList;
                    pushToDolphinRequest.routeName = _masterRouteGroupDetailRepository.FindById(runDetail.MasterRouteGroup_Detail_Guid)?.MasterRouteGroupDetailName;
                    pushToDolphinRequest.clientDateTime = request.ClientDateTime.ChangeDateTimeForDolphin();
                    pushToDolphinRequest.runDailyGuid = runDetail.Guid;
                    pushToDolphinRequest.userAction = request.UserModified;

                    var success = _pushToDolphinService.PushJobToDolphin(pushToDolphinRequest);
                    SyncToDolphinRequest his = new SyncToDolphinRequest()
                    {
                        MsgId = 810,
                        Success = success,
                        DailyRunGuid = request.RunDailyGuid,
                        UserModified = request.UserModified,
                        ClientDateTime = request.ClientDateTime.GetValueOrDefault(),
                        ListJobGuid = request.JobHeadGuidList.Cast<Guid?>()
                    };
                    _pushToDolphinService.SetHistoryLogPushToDolphin(his);
                }
            }
        }



        #endregion
        #region ## Private 
        private CreateJobDetailOtcRequest SetModelOtcJobLegDetail(Guid legGuid, Guid customerLocation, Guid brinksSiteGuid)
        {
            var siteobj = _masterSiteRepository.FindById(brinksSiteGuid);
            return new CreateJobDetailOtcRequest
            {
                MaserActualJobLegGuid = legGuid,
                CusLocationGuid = customerLocation,
                OtcBranchName = siteobj?.SiteName
            };

        }
        #endregion
        #region Sequence of Customer location 
        private IEnumerable<AdhocJob_Info> MultiLocation(CreateMultiJobRequest request)
        {
            var locationData = _masterCustomerLocationRepository.FindLocationByListGuid(request.MasterCustomerLocationGuids);
            List<AdhocJob_Info> result = new List<AdhocJob_Info>();
            var jobNoList = GenerateJobNoMultiJob(request.BrinksSiteGuid, request.MasterCustomerLocationGuids.Count());
            int i = 1;
            foreach (var d in locationData.OrderBy(o => o.BranchName))
            {
                result.Add(new AdhocJob_Info
                {
                    LocationGuid = d.Guid,
                    JobNo = jobNoList[i - 1], //Generating depends on Loops
                    JobGuid = Guid.NewGuid(),
                    LocationSeq = i + request.MaxStop,
                    UnassignedBy = !request.IsCreateToRun ? request.UnassignedBy : null,
                    UnassignedDate = !request.IsCreateToRun ? (DateTime?)request.UnassignedDate : null
                });
                i++;
            }
            return result;
        }
        private int MaxJobOrderOnDailyRun(Guid? runGuid)
        {
            int maxStop = 0;
            if (runGuid.HasValue)
            {
                maxStop = _masterActualJobServiceStopLegsRepository.FindByDailyRun(runGuid).Max(m => m.JobOrder).GetValueOrDefault();
            }
            return maxStop;

        }
        #endregion
    }




}

