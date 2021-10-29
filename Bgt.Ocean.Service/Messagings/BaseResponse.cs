using Bgt.Ocean.Service.ModelViews.Systems;

namespace Bgt.Ocean.Service.Messagings
{
    public class BaseResponse
    {
        public virtual bool IsSuccess { get; set; }
        public virtual  bool IsWarning { get; set; }
        public virtual string Message { get; set; }
        public virtual string Title { get; set; }
        public virtual int MsgID { get; set; }
        public int? Total { get; set; }
        /// <summary>
        /// setup message view
        /// </summary>
        /// <param name="msg"></param>
        public void SetMessageView(SystemMessageView msg)
        {
            IsSuccess = msg.IsSuccess;
            IsWarning = msg.IsWarning;
            Message = msg.MessageTextContent;
            Title = msg.MessageTextTitle;
            MsgID = msg.MsgID;
            
        }

        /// <summary>
        /// setup message view with single parameter
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="textParam"></param>
        public void SetMessageView(SystemMessageView msg, string textParam)
        {
            Message = msg.MessageTextContent;
            Title = msg.MessageTextTitle;
            MsgID = msg.MsgID;

            if (textParam != null)
            {
                Message = string.Format(msg.MessageTextContent, textParam);
            }
        }

        /// <summary>
        /// setup message view with parameter
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="textParams"></param>
        public void SetMessageView(SystemMessageView msg, string[] textParams)
        {
            Message = msg.MessageTextContent;
            Title = msg.MessageTextTitle;
            MsgID = msg.MsgID;

            if (textParams != null)
            {
                Message = string.Format(msg.MessageTextContent, textParams);
            }
        }
    }
}
