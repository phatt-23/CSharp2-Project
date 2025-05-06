using System.Diagnostics.CodeAnalysis;

namespace CoworkingApp.Models.DtoModels;

public class ApiEndpointsResponseDto
{
    public required Dictionary<string, PathItem> Endpoints { get; set; }
    public required Dictionary<string, Schema> Schemas { get; set; }
}

public class PathItem
{
    public Operation? Get { get; set; }
    public Operation? Post { get; set; }
    public Operation? Put { get; set; }
    public Operation? Delete { get; set; }
}

public class Operation
{
    public bool RequiresAuthentication { get; set; }
    public List<string> Authorities { get; set; } = [];
    public List<Parameter> Parameters { get; set; }
    public Response Response { get; set; }
}

public class Parameter
{
    public string Name { get; set; }
    public string In { get; set; }
    public bool Required { get; set; }
    public Schema Schema { get; set; }
}

public class RequestBody
{
    public Dictionary<string, Schema> Content { get; set; }
    public bool Required { get; set; }
}

public class Response
{
    public Dictionary<string, Schema> Content { get; set; }
}

// polymorphic type, either primitive or object or reference
public class Schema
{
    public string Type { get; set; }
    public Dictionary<string, Schema> Properties { get; set; } = new();
    public List<bool> Required { get; set; } = new();
    public bool IsRange { get; set; } = false;
    public bool IsDictionary { get; set; } = false;
    public string? KeyType { get; set; } = null;
    public string? ValueType { get; set; } = null;
    public bool IsList { get; set; } = false;
    public string Reference { get; set; } // $ref
}

// $references
public class Components
{
    public Dictionary<string, Schema> Schemes { get; set; }  = new();
}


