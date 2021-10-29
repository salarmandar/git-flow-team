
namespace Bgt.Ocean.Service.ModelViews.PricingRules
{
    public class TreeViewBase
    {
        /// <summary>
        /// For expand tree view on UI
        /// Warning: case sensitive
        /// </summary>
        public bool expanded { get { return true; } }
        public bool @checked { get; set; }
        public bool enable { get; set; } = true;
    }
}
