using System;
using System.Linq;
using DataService.Features;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DataService.Swagger
{
    public class ActionVisibilityOperationFilter : Swashbuckle.AspNetCore.SwaggerGen.IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {

            var actionName = context.MethodInfo.Name;
            var reflectedType = context.MethodInfo.ReflectedType;
            var declaringType = context.MethodInfo.DeclaringType;
            if (declaringType.Name.StartsWith("GenericController"))
            {
                // by convention the name of the method and the name of the visibility enum match
                // so we try to parse the name of the method in the visibility enum so we can compare afterwards
                GenericControllerActionVisibility currentActionVisibilityCheck = GenericControllerActionVisibility.All;
                Enum.TryParse(actionName, true, out currentActionVisibilityCheck);
                var types = declaringType.GetGenericArguments();
                if (types.Length == 1)
                {
                    var actionsVisibility = Features.EntityTypes.ActionVisibility.ContainsKey(types[0].FullName) ?
                    Features.EntityTypes.ActionVisibility[types[0].FullName] : GenericControllerActionVisibility.All;
                    if (actionsVisibility != GenericControllerActionVisibility.All &&
                        currentActionVisibilityCheck != GenericControllerActionVisibility.All &&
                        (currentActionVisibilityCheck & actionsVisibility) == 0)
                    {
                        operation.Tags.Add(new OpenApiTag()
                        {
                            Name = "REMOVE"
                        });
                    }
                }
            }
        }
    }
    public class ActionVisibilityDocumentFilter : Swashbuckle.AspNetCore.SwaggerGen.IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            // remove all operations marked with the REMOVE tag
            foreach (var value in swaggerDoc.Paths.Values)
            {
                foreach (var operation in value.Operations)
                {
                    if (operation.Value.Tags.Any(a => a.Name == "REMOVE"))
                    {
                        value.Operations.Remove(operation.Key);
                    }
                }
            }
        }
    }
}