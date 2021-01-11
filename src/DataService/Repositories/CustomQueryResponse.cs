using System.Collections.Generic;

namespace DataService.Repositories
{
    public class CustomQueryResponse
    {
        public List<string> Columns { get; set; }
        public List<Dictionary<string, object>> Data { get; set; }
    }
}