namespace TaskManager.Infrastructure.Persistence.Data
{
    public static class DefaultRoles
    {
        public static class Admin
        {
            public const long Id = 1;
            public const string Name = nameof(Admin);
            public const string ConcurrencyStamp = "019a72b4-22b5-752d-99a9-70bdfa2b942c";
        }
        public static class Member
        {
            public const long Id = 2;
            public const string Name = nameof(Member);
            public const string ConcurrencyStamp = "019a72b4-22b6-7d48-ae78-8573b711cae0";
        }
    }
}
