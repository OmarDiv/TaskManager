namespace TaskManager.Application.Common.Authentication
{
    public class JwtOptions
    {
        public static string SectionName => "Jwt";
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public int ExpirationInMinutes { get; set; }
    }
}
