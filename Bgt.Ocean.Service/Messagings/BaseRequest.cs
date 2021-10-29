namespace Bgt.Ocean.Service.Messagings
{
    public interface IBaseRequest
    {
        RequestBase Data { get; }
    }
    public class BaseRequest : IBaseRequest
    {
        public BaseRequest()
        {
            Data = new RequestBase();
        }
        public RequestBase Data { get; set; }
    }
}
