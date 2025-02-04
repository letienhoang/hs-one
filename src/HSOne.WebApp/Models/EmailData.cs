namespace HSOne.WebApp.Models
{
    public class EmailData
    {
        public string? FromEmail { get; set; }
        public string? FromName { get; set; }
        public required string ToEmail { get; set; }
        public string? ToName { get; set; }
        public required string Subject { get; set; }
        public required string Content { get; set; }
        public string? Template { get; set; }
        public dynamic? TemplateData { get; set; }
    }
}
