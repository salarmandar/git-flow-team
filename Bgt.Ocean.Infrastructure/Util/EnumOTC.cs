
namespace Bgt.Ocean.Infrastructure.Util
{
    public static class EnumOTC
    {
        public static class OTCStatus
        {
            public const string InActive = "00";
            public const string Active = "01";
            public const string Close = "02";
            public const string CloseFail = "03";
            public const string GenerateFail = "04";
        }

        public static class OTCStatus_Guid
        {
            public const string InActive = "4EF39633-3670-4BDB-A0D7-6B727F3F8790";
            public const string Active = "C727818B-97EA-4646-8D0B-E4353DCC2A55";
            public const string Close = "AAF885D2-5807-45C5-9971-6387646EAC14";
            public const string CloseFail = "83D321B9-0A0B-4D54-BC96-11A599A1A7ED";
            public const string GenerateFail = "33A510D0-BD6C-4BC5-B317-FD17BA8DF77B";
        }

        public static class LockType
        {
            public const string SG = "SG";
            public const string Cencon = "CENCON";
            public const string SpinDial = "SPIN_DIAL";
            public const string BrinksBox = "BRINKS_BOX";
        }

        public static class LockTypeName
        {
            public const string SG = "SG";
            public const string Cencon = "Cencon";
            public const string SpinDial = "Spin Dial";
            public const string BrinksBox = "Brink’s Box";
        }
        public static class LockTypeID
        {
            public const string SG = "01";
            public const string Cencon = "02";
            public const string SpinDial = "03";
            public const string BrinksBox = "04";
            public const string Crypto = "05";
        }
       

        public static class LockTypeGuid
        {
            public const string SG = "1586EB8C-186B-4380-A4DE-70FBC5288C3D";
            public const string Cencon = "59884F00-0EB8-4E7F-8591-19671EC57C8A";
            public const string SpinDial = "D6F1A33F-DF13-4F02-8BA7-867B90358CE3";
            public const string Crypto = "700315BC-85C1-4AA4-8583-5D2CEAC556F2";
        }

        public static class ErrorCode
        {
            public const string CloseSealBeforeNewGenerateOTC = "-2049";
        }

        public static class MsgID
        {
            public const int GenerateSuccessAll = 609;
            public const int GenerateErrorAll = -2051;
            public const int GenerateHasSomeError = -2052;
            public const int CloseSuccessAll = 616;
            public const int CloseErrorAll = -2056;
            public const int CloseHasSomeError = -2057;
            public const int OperationFail = -2062;
        }

        public static class FunctionName
        {
            public const string GenerateOTC = "GenerateOTC";
            public const string ReassignOTC = "ReassignOTC";
            public const string CloseSeal = "CloseSeal";
        }

        public static class ActionType
        {
            public const string RequestOTC = "Request OTC";
            public const string CloseLock = "Close Lock";
        }

        public static class SFIErrCode
        {
            public const string RC_ERR_LOCK_ALREADY_OPEN = "RC_ERR_LOCK_ALREADY_OPEN";
            public const string RC_ERR_LOCK_NOT_OPEN = "RC_ERR_LOCK_NOT_OPEN";
            public const string RC_OK = "RC_OK";
            public const string RC_OK_DESC = "The lock close succeeded.";
            public const string SG_ERR_DUP_REQ = "Duplicate operation code.";
            public const string SG_ERR_DUP_GEN_REQ = "Duplicate Operation Code found in History !";
            public const string SG_ERR_LCK_NOT_OPN = "Operation code not found.";
            public const string CC_ERR_LCK_NOT_OPN = "RC_ERR_LOCK_NOT_OPEN";
        }

        public static class UserLockMode
        {
            public const string SingleMode = "01";
            public const string DualMode = "02";
        }

        public static class DolphinLockAction
        {
            public const string GenerateSuccess = "GEN_SUCCESS";
            public const string GenerateFail = "GEN_FAILED";
            public const string CloseSuccess = "CLOSE_SUCCESS";
            public const string CloseFail = "CLOSE_FAILED";
            public const string RegenerateSuccess = "REGEN_SUCCESS";
            public const string RegenerateFail = "REGEN_FAILED";
        }

        public static class OTCAction
        {
            public const string GenerateOTC = "GEN";
            public const string CloseSeal = "CLOSE";
        }

        public static class RequestAction
        {
            public const string Leg = "LEG";
            public const string Lock = "LOCK";
        }

        public static class ResponseAction
        {
            public const string Success = "SUCCESS";
            public const string Fail = "FAIL";
        }

        public static class ConstantValue
        {
            public const string DefaultPIN = "0000";
        }
    }
}
