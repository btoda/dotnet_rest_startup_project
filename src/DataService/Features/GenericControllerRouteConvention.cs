using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace DataService.Features
{
    public class GenericControllerRouteConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            if (controller.ControllerType.IsGenericType)
            {
                var x = controller.ControllerType.GenericTypeArguments;
                var cname = controller.ControllerName;
                var genericType = controller.ControllerType.GenericTypeArguments[0];
                var typeName = genericType.Name;
                var str = genericType.ToString();
                controller.ControllerName = typeName;
            }
        }
    }
}