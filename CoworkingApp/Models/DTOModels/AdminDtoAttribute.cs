namespace CoworkingApp.Models.DtoModels;

    
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class AdminDtoAttribute : Attribute;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class PublicDtoAttribute : Attribute;


[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class PublicDataDto : PublicDtoAttribute;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class AdminDataDto : AdminDtoAttribute;


[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class PublicRequestDto : PublicDtoAttribute;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class AdminRequestDto : AdminDtoAttribute;


[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class PublicResponseDto : PublicDtoAttribute;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class AdminResponseDto : AdminDtoAttribute;


