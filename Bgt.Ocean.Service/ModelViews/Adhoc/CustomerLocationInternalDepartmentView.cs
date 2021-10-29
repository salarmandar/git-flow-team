namespace Bgt.Ocean.Service.ModelViews.Adhoc
{
    public class CustomerLocationInternalDepartmentView
    {
        //public System.Guid Guid { get; set; }
        //public string InterDepartmentName { get; set; }
        public System.Guid id { get; set; }         //2018/12/24: CANNOT CHANGE THIS NAME DUE TO NEW AD-HOC STILL USED SOME FUNCTION THAT'S NOT API YET. CANNOT BIND IF CHANGE NAME
        public string text { get; set; }            //2018/12/24: CANNOT CHANGE THIS NAME DUE TO NEW AD-HOC STILL USED SOME FUNCTION THAT'S NOT API YET. CANNOT BIND IF CHANGE NAME
        public bool FlagDefaultOnward { get; set; }
    }
}
