using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels;


/// View model used by profile pages to display and update user profile information.

public class UserProfileViewModel
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;

    [EmailAddress]
    public string Email { get; set; } = null!;

    public string Role { get; set; } = null!;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
}
