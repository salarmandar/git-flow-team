namespace Bgt.Ocean.Models.RunControl
{
    public class ValidateTruckLiabilityLimitResponse
    {
        public bool isSuccess { get; set; }
        public string message { get; set; }
        public bool flagExceed { get; set; }
    }
}
