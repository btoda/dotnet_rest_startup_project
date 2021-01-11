namespace DataService.MSBuildExtensions
{
    [System.AttributeUsage(System.AttributeTargets.Assembly, Inherited = false, AllowMultiple = false)]
    sealed class TitleAttribute : System.Attribute
    {
        public string Value { get; }
        public TitleAttribute(string configTitle)
        {
            this.Value = configTitle;
        }
    }
}