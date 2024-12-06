namespace HSOne.Core.Models.System
{
    public class ChangeMyPasswordRequest
    {
        public required string CurrentPassword { get; set; }
        public required string NewPassword { get; set; }
    }
}
