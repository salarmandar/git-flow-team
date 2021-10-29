using System.Collections.Generic;

namespace Bgt.Ocean.Models.Nemo.NemoSync
{
    public class NemoErrorMessage
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public IEnumerable<ErrorDetail> ErrorDetail { get; set; }
    }

    public class ErrorDetail
    {
        public string Error { get; set; }
    }
}
