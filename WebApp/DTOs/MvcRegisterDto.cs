namespace WebApp.DTOs
{
    
    /// View-facing DTO used by the MVC register form to send registration details to the API.
   
    public class MvcRegisterDto
    {
        public string Username { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string RepeatPassword { get; set; } = null!;
    }
}