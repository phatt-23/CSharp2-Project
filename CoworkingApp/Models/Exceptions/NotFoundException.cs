namespace CoworkingApp.Models.Exceptions;

public class NotFoundException(string message) : Exception(message)
{
}
