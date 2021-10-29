using Bgt.Ocean.Service.ModelViews.Systems;

namespace Bgt.Ocean.Service.Messagings.QuotationService
{
    public class SaveCopyProductResponse
    {
        public bool IsDuplicate { get; set; }
        public SystemMessageView Message { get; set; }
    }
}
