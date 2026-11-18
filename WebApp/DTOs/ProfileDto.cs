namespace WebApp.DTOs
{
    
    /// View-facing DTO used by profile pages when sending editable profile data from JavaScript to the controller.
   
    public class ProfileDto
    {
        public int Id { get; set; }

        public string? Email { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? PhoneNumber { get; set; }
    }
}