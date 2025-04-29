namespace CoworkingApp.Models.DtoModels;

    
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class AdminDtoAttribute : Attribute
{
    public bool IsAdmin => true;
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class PublicDtoAttribute : Attribute
{
    public bool IsAdmin => false;
}
