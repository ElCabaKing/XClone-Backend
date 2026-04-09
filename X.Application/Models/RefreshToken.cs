namespace X.Application.Models;

public class RefreshToken
{
	public required Guid CollaboratorId { get; set; }
	public required TimeSpan ExpirationInDays { get; set; }
}
