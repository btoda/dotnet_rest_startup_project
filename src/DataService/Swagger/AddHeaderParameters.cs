using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DataService.Swagger
{
    public class AddRequiredHeaderParameters : IOperationFilter
    {
        public static List<string> GenericControllerGetMethods = new List<string>{
            "GetAll", "GetAllPaged", "GetAllQuerySelector", "GetAllPagedQuerySelector", "Find"
        };
        public static List<string> GenericControllerAllMethods = new List<string>
        {
            "GetAll", "GetAllPaged", "GetAllQuerySelector", "GetAllPagedQuerySelector", "Find", "Create", "Update", "Delete"
        };
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();
            var actionDescriptor = context.ApiDescription.ActionDescriptor;
            var actionName = context.MethodInfo.Name;
            var reflectedType = context.MethodInfo.ReflectedType;
            var declaringType = context.MethodInfo.DeclaringType;

            if (declaringType.Name.StartsWith("GenericController")
            || (declaringType.BaseType != null && declaringType.BaseType.Name.StartsWith("GenericController")))
            {
                // this is a generic controller
                // only add the fields param to GetAll, GetAllPaged and Find functions
                if (GenericControllerGetMethods.Contains(actionName))
                {
                    operation.Parameters.Add(new OpenApiParameter
                    {
                        Name = "DataFields",
                        In = ParameterLocation.Header,
                        Required = false,
                        Description = "Fields to retrieve, leave empty for all",
                        Schema = new OpenApiSchema
                        {
                            Type = "String",
                            Default = null
                        }
                    });
                }
            }
        }
    }
}