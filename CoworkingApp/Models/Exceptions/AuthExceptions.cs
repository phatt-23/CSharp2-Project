using CoworkingApp.Models.DtoModels;

namespace CoworkingApp.Models.Exceptions;

public class EmailTakenException(string message) : Exception(message);

public class WrongPasswordException(string message) : Exception(message)
{
    public readonly string PropertyName = nameof(UserLoginRequestDto.Password);
}
