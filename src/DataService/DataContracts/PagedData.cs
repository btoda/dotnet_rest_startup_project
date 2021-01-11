using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DataService.DataContracts
{
    [DataContract]
    public class PagedData<T>
    {
        [DataMember]
        public List<T> Data { get; set; }
        [DataMember]
        public int ItemsCount { get; set; }
        [DataMember]
        public int PageIndex { get; set; }
        [DataMember]
        public int PageSize { get; set; }
        [DataMember]
        public int PageCount
        {
            get
            {
                if (PageSize > 0 && ItemsCount > 0)
                {
                    return ItemsCount / PageSize + (ItemsCount % PageSize == 0 ? 0 : 1);
                }
                return 0;
            }
        }
        [DataMember]
        public Error Error { get; set; }
    }
}