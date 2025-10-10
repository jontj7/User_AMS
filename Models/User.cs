namespace User_AMS.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    public int RoleId { get; set; }
    public Role? Role { get; set; }

    public int StatusId { get; set; }
    public Status? Status { get; set; }

    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
}
