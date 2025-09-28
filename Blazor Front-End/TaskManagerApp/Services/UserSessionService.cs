using System.Collections.Generic;

public class UserSessionService
{
    public string? UserName { get; set; }
    public string? UserEmail { get; set; }
    public List<int> RegisteredEventIds { get; set; } = new();
}