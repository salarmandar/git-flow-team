
namespace Bgt.Ocean.Infrastructure.Util
{
    public static class EnumUser
    {
        public enum AuthenType
        {
            Email,
            Domain,
            Local
        }

        public static class RoleType
        {
            public const int GlobalAdmin = 1;
            public const int LocalAdmin = 2;
            public const int BrinksUser = 3;
            public const int CustomerAdminCountry = 4;
            public const int CustomerUser = 5;
            public const int CustomerAdminGlobal = 6;
        }

        public enum PasswordLength
        {
            [EnumDescription("Auto")]
            AutoGenerate,
            [EnumDescription("Manual")]
            Manual
        }

        public enum EnumMenuCommandId
        {
            [EnumDescription("Verify Vault Balance")]
            VerifyVaultBalance = 11702
        }
    }
}
