namespace DataService.MSBuildExtensions
{
    [System.AttributeUsage(System.AttributeTargets.Assembly, Inherited = false, AllowMultiple = false)]
    sealed class DescriptionAttribute : System.Attribute
    {
        public string Value { get; }
        public DescriptionAttribute(string configTitle)
        {
            this.Value = configTitle;
        }
    }
}