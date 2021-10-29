

namespace Bgt.Ocean.Models.RunControl
{
    public class TabServiceDetailView
    {
        //CIT
        public SvdCitDelviery SVD_CITDelivery { get; set; }
        //Note Withdraw
        public SvdNoteWithdrawMachineReport SVD_MachineReport { get; set; }
        public SvdNoteWithdrawActualCount SVD_ActualCount { get; set; }

        public SvdNoteWithdrawCashAdd SVD_CashAdd { get; set; }
        public SvdNoteWithdrawCashReturn SVD_CashReturn { get; set; }

        //Recycling
        public SvdRecyclingMachineReportWODispense SVD_Recycling_MachineReportWODispense { get; set; }
        public SvdRecyclingActualCount SVD_Recycling_ActualCount { get; set; }
        public SvdRecyclingCashRecycling SVD_Recycling_CashRecycling { get; set; }

        //Bulk Note Deposit
        public SvdBulkNoteDepositDepositReport SVD_BulkNoteDeposit_DepositReport { get; set; }
        public SvdBulkNoteDepositRetract SVD_BulkNoteDeposit_Retract { get; set; }

        public SvdBulkNoteDepositSuspectFake SVD_BulkNoteDeposit_SuspectFake { get; set; }
        public SvdBulkNoteDepositSuspectFakeDetail SVD_BulkNoteDeposit_SuspectFakeDetail { get; set; }

        public SvdBulkNoteDepositJammed SVD_BulkNoteDeposit_Jammed { get; set; }
        public SvdBulkNoteDepositJammedDetail SVD_BulkNoteDeposit_JammedDetail { get; set; }

        //Coin Exchange
        public SvdCoinExchangeMachineBalance SVD_CoinExchange_MachineBalance { get; set; }
        public SvdCoinExchangeCashAdd SVD_CoinExchange_CashAdd { get; set; }
        public SvdCoinExchangeCashReturn SVD_CoinExchange_CashReturn { get; set; }
        public SvdCoinExchangeBulkNote SVD_CoinExchange_BulkNote { get; set; }
        public SvdCoinExchangeSuspectFake SVD_CoinExchange_SuspectFake { get; set; }

        //Small Bag Deposit
        public SvdSmallBagDepositSmallBag SVD_SmallBagDeposit_SmallBag { get; set; }
        public SvdSmallBagDepositSmallBagCollection svD_SmallBagDeposit_SmallBagCollection { get; set; }
        //Bulk Cash Deposit [Obsolete]
        public SvdBulkCashDepositBulkCash SVD_BulkCashDeposit_BulkCash { get; set; } = new SvdBulkCashDepositBulkCash();
        //Other
        public SvdCapturedCard SVD_CapturedCard { get; set; }
        public SvdChecklist SVD_Checklist { get; set; }
    }

}
