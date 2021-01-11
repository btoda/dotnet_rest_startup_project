using System;
using System.Runtime.Serialization;

namespace DataService.DataContracts
{
    [DataContract]
    public class Error
    {
        [DataMember]
        public int ErrorId { get; set; }
        [DataMember]
        public string ErrorCode { get; set; }
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public string ErrorText { get; set; }
        [DataMember]
        public string ErrorStackTrace { get; set; }

        public Error()
        {

        }
        public Error(string message)
        {
            this.Message = message;
        }
        public Error(Exception ex)
        {
            if (ex != null)
            {
                this.ErrorText = ex.Message;
                this.ErrorStackTrace = ex.StackTrace != null ? ex.StackTrace.ToString() : null;
            }
        }
    }
}