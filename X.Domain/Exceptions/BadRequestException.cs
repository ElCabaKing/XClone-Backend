namespace X.Domain.Exceptions;

public class BadRequestException(string message) : Exception(message)
{
}