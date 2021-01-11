namespace DataService.Features
{
    public enum GenericControllerActionVisibility
    {
        All = 0,
        GetAll = 1,
        GetAllQuerySelector = 2,
        GetAllPaged = 4,
        GetAllPagedQuerySelector = 8,
        Find = 16,
        Create = 32,
        Update = 64,
        Delete = 128
    }
}