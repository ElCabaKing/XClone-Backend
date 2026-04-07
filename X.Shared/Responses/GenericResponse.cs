namespace X.Shared.Responses;
public class GenericResponse<T>()
{
    public T Data { get; set; }
    public string Message { get; set; } 
    public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
    public List<string> Errors { get; set; } = [];
}