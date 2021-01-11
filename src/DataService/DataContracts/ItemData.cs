using System;
using System.Runtime.Serialization;

namespace DataService.DataContracts
{
    [DataContract]
    public class ItemData<T>
    {
        [DataMember]
        public T Data { get; set; }
        [DataMember]
        public Error Error { get; set; }

        public void AddError(string message)
        {
            if (Error == null)
            {
                Error = new Error();
                Error.Message = message;
                Error.ErrorId = 100;
            }
            else
            {
                Error.Message += "\n" + message;
            }
        }

        public void AddError(Exception ex)
        {
            var message = ex.Message + "\n" + ex.StackTrace;
            if (Error == null)
            {
                Error = new Error();
                Error.Message = message;
                Error.ErrorId = 100;
            }
            else
            {
                Error.Message += "\n" + message;
            }
        }
    }
}