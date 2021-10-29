
namespace Bgt.Ocean.Service.Messagings.ServiceRequest
{
    public class ResponseCreateServiceRequest
    {
        /// <summary>
        /// 1 = Success, 0 = Invalid request, -1 = Error
        /// </summary>
        public string responseCode { get; set; } = "1";
        public string responseMessage { get; set; } = "Success";
        /// <summary>
        /// Number of row
        /// </summary>
        public int rows { get; set; } = 0;
    }
}
