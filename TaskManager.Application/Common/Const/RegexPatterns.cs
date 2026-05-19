namespace TaskManager.Application.Common.Const
{
    public class RegexPatterns
    {
        public const string Password = "(?=(.*[0-9]))(?=.*[\\!@#$%^&*()\\\\[\\]{}\\-_+=~`|:;\"'<>,./?])(?=.*[a-z])(?=(.*[A-Z]))(?=(.*)).{8,}";
        public const string EgyptPhoneNumer = @"^(?:\+?20|0)?1[0-2,5]\d{8}$";
    }
}
