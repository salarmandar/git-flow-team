namespace Bgt.Ocean.Infrastructure.Util
{
    public enum EnumMOT
    {
        Undefined = 0,
        Vehicle = 1,
        Walker = 2,
        Scooter = 3
    }

    public enum EnumMaintenanceStatus
    {
        Undefined = 0,//<-- use for filter show all status 
        InProgress = 1,
        Closed = 2,
        Cancel = 3,
        Normal = 99
    }

    public enum EnumDiscountType
    {
        Undefined = 0,
        Currency = 2,
        Percentage = 1
    }

    public enum EnumRunResourceStatus
    {
        Undefined = 0,
        [EnumDescription("F9A33DDA-B77C-4E95-B61E-41735830B39A")]
        Active = 1,
        [EnumDescription("89E99DBB-71A0-449B-9306-C0988F7FB07A")]
        InService = 2,
        [EnumDescription("F36DFCB2-B232-4D25-8A94-37584D5CA724")]
        PendingforService = 3,
        [EnumDescription("F2C817E4-C0D0-48D3-A4C7-3EF206E698CA")]
        Standby = 4,
        [EnumDescription("24E7B609-6FAD-41EE-B4BF-A942977BEDB3")]
        Closed = 5
    }

    public enum EnumFleetOption
    {
        [EnumDescription("Summary")]
        Summary = 0,
        [EnumDescription("Maintenance")]
        Maintenance = 1,
        [EnumDescription("Gasoline")]
        Gasoline = 2,
        [EnumDescription("Accident")]
        Accident = 3
    }

    public enum EnumMaintenanceState
    {
        Estimate = 0,
        Actual = 1
    }
    public enum EnumFleetOperator
    {
        [EnumDescription("<")]
        LessThan = 0,
        [EnumDescription("<=")]
        LessThanEqual = 1,
        [EnumDescription("=")]
        Equal = 2,
        [EnumDescription("!=")]
        NotEqual = 3,
        [EnumDescription(">")]
        GreaterThan = 4,
        [EnumDescription(">=")]
        GreaterThanEqual = 5
    }
}
