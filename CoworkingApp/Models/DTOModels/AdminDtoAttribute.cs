namespace CoworkingApp.Models.DTOModels;

    
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class AdminDtoAttribute : Attribute
{
    public bool IsAdmin => true;
}