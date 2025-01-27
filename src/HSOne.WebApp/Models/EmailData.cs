namespace HSOne.WebApp.Models
{
    public class EmailData
    {
        public required string FromEmail { get; set; }
        public required string FromName { get; set; }
        public required string ToEmail { get; set; }
        public required string ToName { get; set; }
        public required string Subject { get; set; }
        public required string Content { get; set; }
        public string? Template { get; set; }
        public dynamic? TemplateData { get; set; }
    }
}
