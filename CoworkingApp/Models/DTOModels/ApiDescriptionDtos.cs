namespace CoworkingApp.Models.DtoModels;

public class ApiEndpointDescription
{
    public required string HttpMethod { get; set; }
    public required string Route { get; set; }
    public bool RequiresAuthentication { get; set; }
    public bool AllowsAnonymous { get; set; }
    public List<ApiParameterDescription> Parameters { get; set; } = new();
    public required string ReturnType { get; set; }
}

public class ApiParameterDescription
{
    public required string Name { get; set; }
    public required string Type { get; set; }
    public bool IsComplexType { get; set; }
    public List<ApiFieldDescription> Fields { get; set; } = [];
}

public class ApiFieldDescription
{
    public required string Name { get; set; }
    public required string Type { get; set; }
    public bool IsComplexType { get; set; }
    public List<ApiFieldDescription> SubFields { get; set; } = [];
}