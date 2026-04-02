using System;
using X.Domain.Enums;

namespace X.Domain.Entities;

public class UserEntity(Guid id, string username, string email, string passwordHash, DateTime createdAt, bool isVerified, UserStatusEnum statusId, string firstName, string lastName, string? profilePictureUrl)
{
    public Guid Id { get; set; } = id;
    public string Username { get; set; } = username;
    public string Email { get; set; } = email;
    public string PasswordHash { get; set; } = passwordHash;
    public DateTime CreatedAt { get; set; } = createdAt;
    public bool IsVerified { get; set; } = isVerified;
    public UserStatusEnum StatusId { get; set; } = statusId;
    public string FirstName { get; set; } = firstName;
    public string LastName { get; set; } = lastName;
    public string? ProfilePictureUrl { get; set; } = profilePictureUrl;
}
