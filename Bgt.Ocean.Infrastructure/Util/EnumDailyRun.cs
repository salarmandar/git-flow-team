
namespace Bgt.Ocean.Infrastructure.Util
{
    public class EnumRun
    {
        public static class StatusDailyRun
        {
            public const int ReadyRun = 1;
            public const int DispatchRun = 2;
            public const int ClosedRun = 3;
            public const int CrewBreak = 4;
        }
        public static class StatusRun
        {
            public const int Active = 1;
            public const int InService = 2;
            public const int PendingforService = 3;
            public const int Standby = 4;
        }
        public static class FixStringRun
        {
            public const string Withdraw = "Withdraw";
            public const string Return = "Return";
        }

        public enum ModeOfTranSport
        {
            [EnumDescription("Vehicle")]
            Vehicle = 1,
            [EnumDescription("Walker")]
            Walker = 2,
            [EnumDescription("Scooter")]
            Scooter = 3
        }
    }
}
