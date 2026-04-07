namespace X.Domain.Exceptions;

public class NotFoundException(string message) : Exception(message)
{
}