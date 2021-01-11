using System.Reflection;
using System.Runtime.Versioning;
using DataService.MSBuildExtensions;
using Microsoft.AspNetCore.Hosting;

namespace DataService.Models
{
    public class IndexModel
    {
        public string SwaggerController { get; set; }
        public string SwaggerAction { get; set; }

        public string ServiceName { get; set; }
        public string ServiceDescription { get; set; }
        public string Version { get; set; }
        public string Framework { get; set; }
        public string IPAddress { get; set; }

        public string Environment { get; set; }


        public IndexModel( IWebHostEnvironment env)
        {
            SwaggerController = "/swagger";
            SwaggerAction = "";

            Version = Assembly.GetEntryAssembly().GetName().Version.ToString();
            Framework = Assembly.GetEntryAssembly().GetCustomAttribute<TargetFrameworkAttribute>().FrameworkName;
            ServiceName = Assembly.GetEntryAssembly().GetCustomAttribute<TitleAttribute>().Value.Replace("\"",string.Empty);
            ServiceDescription = Assembly.GetEntryAssembly().GetCustomAttribute<DescriptionAttribute>().Value.Replace("\"",string.Empty);


            Environment = env.EnvironmentName;
        }
    }
}