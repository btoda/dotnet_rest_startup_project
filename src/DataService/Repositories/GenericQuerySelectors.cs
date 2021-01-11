using System.Collections.Generic;

namespace DataService.Repositories
{
    public class GenericQuerySelectors
    {
        public List<GenericQueryFilter> Filters { get; set; }
        public List<GenericQuerySortOrder> SortOrders { get; set; }
        public List<string> Includes { get; set; }

        public GenericQuerySelectors()
        {
            Filters = new List<GenericQueryFilter>();
            SortOrders = new List<GenericQuerySortOrder>();
            Includes = new List<string>();
        }
    }

    public class GenericQueryFilter
    {
        public string FieldName { get; set; }
        public string OpName { get; set; }
        public string MethodName { get; set; }
        public string FilterValue { get; set; }

        public GenericQueryFilter()
        {

        }

        public GenericQueryFilter(string field, string op, string method, string value)
        {
            this.FieldName = field;
            this.OpName = op;
            this.MethodName = method;
            this.FilterValue = value;
        }
    }

    public class GenericQuerySortOrder
    {
        public string FieldName { get; set; }
        public string OrderDirection { get; set; }
    }
}