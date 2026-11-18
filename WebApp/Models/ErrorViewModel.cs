namespace WebApp.Models
{
    
    /// View model used by the error page to display the request ID when an error occurs.
   
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        
        /// Performs the IsNullOrEmpty operation for this class.
       
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
