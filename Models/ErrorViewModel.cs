namespace GyanGanga.Web.Models 
{ 
    // Model for error pages 
    public class ErrorViewModel 
    { 
        public string? RequestId { get; set; } 
 
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId); 
    } 
} 
