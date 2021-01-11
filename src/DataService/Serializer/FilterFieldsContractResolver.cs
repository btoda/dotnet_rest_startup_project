using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Data.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DataService.Serializer
{
    public class FilterFieldsContractResolver : DefaultContractResolver
    {
        private readonly Dictionary<Type, string[]> typeFields = null;
        // Compute the name of the metadata properties, and store it statically as a cache
        public static string[] _metaDataProperties = null;
        public static string[] MetaDataProperties
        {
            get
            {
                if (_metaDataProperties == null)
                {
                    _metaDataProperties = (typeof(Metadata)).GetProperties().Select(a => a.Name).ToArray();
                }
                return _metaDataProperties;
            }
        }
        public FilterFieldsContractResolver(Dictionary<Type, string[]> typeFields)
        {
            this.typeFields = typeFields;
        }
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var allProps = base.CreateProperties(type, memberSerialization);
            // remove metadata fields
            if ((typeof(Metadata)).IsAssignableFrom(type))
            {
                allProps = allProps.Where(a => !MetaDataProperties.Contains(a.PropertyName)).ToList();
            }

            if (typeFields.ContainsKey(type))
            {
                string[] fields = typeFields[type];
                if (fields == null || fields.Length == 0) return allProps;
                return allProps.Where(a => fields.Contains(a.PropertyName)).ToList();
            }


            return allProps;
        }
    }
}