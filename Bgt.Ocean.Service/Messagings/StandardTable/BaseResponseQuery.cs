namespace Bgt.Ocean.Service.Messagings.StandardTable
{
    public class BaseResponseQuery
    {
        public string guid { get; set; }
        public string countryAbb { get; set; }

        /// <summary>
        /// UTC +0
        /// </summary>
        public string createdDatetime { get; set; }
        public string createdUser { get; set; }

        /// <summary>
        /// UTC +0
        /// </summary>
        public string modifiedDatetime { get; set; }
        public string modifiedUser { get; set; }
    }
}
