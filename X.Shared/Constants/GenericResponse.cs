
public class GenericResponse<T>(T data, string message = "Success", bool success = true)
{
    public T Data { get; set; } = data;
    public string Message { get; set; } = message;
    public bool Success { get; set; } = success;
}