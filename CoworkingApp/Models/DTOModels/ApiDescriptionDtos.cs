namespace CoworkingApp.Models.DTOModels;

public class ApiEndpointDescription
{
    public string HttpMethod { get; set; }
    public string Route { get; set; }
    public bool RequiresAuthentication { get; set; }
    public bool AllowsAnonymous { get; set; }
    public List<ApiParameterDescription> Parameters { get; set; } = new();
    public string ReturnType { get; set; }
}

public class ApiParameterDescription
{
    public string Name { get; set; }
    public string Type { get; set; }
    public bool IsComplexType { get; set; }
    public List<ApiFieldDescription> Fields { get; set; } = new();
}

public class ApiFieldDescription
{
    public string Name { get; set; }
    public string Type { get; set; }
    public bool IsComplexType { get; set; }
    public List<ApiFieldDescription> SubFields { get; set; } = new();
}