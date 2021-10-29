namespace Bgt.Ocean.Models.CustomerLocation
{
    public class CustomerLocationInternalDepartmentModel
    {
        public System.Guid id { get; set; }         //2018/12/24: CANNOT CHANGE THIS NAME DUE TO NEW AD-HOC STILL USED SOME FUNCTION THAT'S NOT API YET. CANNOT BIND IF CHANGE NAME
        public string text { get; set; }            //2018/12/24: CANNOT CHANGE THIS NAME DUE TO NEW AD-HOC STILL USED SOME FUNCTION THAT'S NOT API YET. CANNOT BIND IF CHANGE NAME
        public bool FlagDefaultOnward { get; set; }
        public bool FlagISA { get; set; } = false;
    }
}
