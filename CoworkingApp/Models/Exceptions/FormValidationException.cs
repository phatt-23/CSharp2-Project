namespace CoworkingApp.Models.Exceptions;

public class FormValidationException(string message, string properyName) : Exception(message)
{
    public readonly string PropertyName = properyName;
}
