using System.Collections.Generic;
using System.Reflection;

namespace DataService.Features
{
    public class EntityTypes
    {
        public static List<TypeInfo> Types { get; set; } = new List<TypeInfo>();
        public static Dictionary<string, GenericControllerActionVisibility> ActionVisibility { get; set; }
            = new Dictionary<string, GenericControllerActionVisibility>();
    }
}