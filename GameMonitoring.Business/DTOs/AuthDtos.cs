namespace GameMonitoring.Business.DTOs;


/// DTO used by the API login endpoint to receive the username and password entered by a user.

public class LoginDto
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}


/// DTO used by the API register endpoint to receive the information needed to create a new user account.

public class RegisterDto
{
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}


/// DTO returned after a successful login; it contains the user identity, role, and JWT token used by the client.

public class LoginResponseDto
{
    public int UserId { get; set; }
    public string Token { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Role { get; set; } = null!;
}


/// DTO used when a user requests to change their password through the API.

public class ChangePasswordDto
{
    public string Username { get; set; } = null!;
    public string OldPassword { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}
