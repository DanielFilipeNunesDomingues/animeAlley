using System.ComponentModel.DataAnnotations;

namespace animeAlley.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public int StatusCode { get; set; }
        public string? ErrorTitle { get; set; }
        public string? ErrorMessage { get; set; }
        public string? ErrorDescription { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        public bool ShowTechnicalDetails { get; set; } = false;
        public string? ExceptionMessage { get; set; }
        public string? OriginalPath { get; set; }
        public DateTime ErrorTime { get; set; } = DateTime.Now;
    }
}