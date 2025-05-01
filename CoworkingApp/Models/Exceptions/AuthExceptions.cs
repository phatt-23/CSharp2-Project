using CoworkingApp.Models.DtoModels;

namespace CoworkingApp.Models.Exceptions;

// Login

public class EmailTakenException(string message) : FormValidationException(message, nameof(UserRegisterRequestDto.Email));

public class WrongPasswordException(string message) : FormValidationException(message, nameof(UserLoginRequestDto.Password));

// Registration

public class PasswordMismatchException(string message) : FormValidationException(message, nameof(UserRegisterRequestDto.ConfirmPassword));
