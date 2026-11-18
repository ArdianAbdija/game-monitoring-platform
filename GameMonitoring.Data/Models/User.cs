using System;
using System.Collections.Generic;

namespace GameMonitoring.Data.Models;


/// Entity model that represents an application user account, including login details, role, profile fields, and reviews.

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Role { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? PhoneNumber { get; set; }

    
    /// Performs the Review operation for this class.
   
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
