namespace HSOne.Core.ConfigOptions
{
    public class JwtTokenSettings
    {
        public required string Key { get; set; }
        public required string Issuer { get; set; }
        public int ExpireInHours { get; set; }
    }
}