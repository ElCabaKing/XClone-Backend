
public class GenerateResponse
{
    public static GenericResponse<T> Success<T>(T data, string message = "Success")
    {
        return new GenericResponse<T>(data, message, true);
    }

    public static GenericResponse<T> Failure<T>(string message = "An error occurred")
    {
        return new GenericResponse<T>(default!, message, false);
    }
}