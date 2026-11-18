namespace WebApp.DTOs
{
    
    /// View-facing DTO used by the MVC login form to send login details to the API.
   
    public class MvcLoginDto
    {
        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;
    }
}