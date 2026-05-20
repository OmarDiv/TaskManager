using Microsoft.AspNetCore.Identity;

namespace TaskManager.Infrastructure.Persistence.Data
{
    public static class DefaultUsers
    {
        public static class Admin
        {
            public const long Id = 1;
            public const string Email = "admin@taskmanager.com";
            public const string PasswordHash = "AQAAAAIAAYagAAAAENbAD+udZ2X1bEmN/mP4cH0YEEpVaaVq6/5FXf8hys0WsIl1PFMic3ZuU+DfKsvdJQ=="; // Password: Password123!
            public const string SecurityStamp = "019a72b4-22b5-752d-99a9-70b863ac6dae";
            public const string ConcurrencyStamp = "019a72b4-22b5-752d-99a9-70b93dfe3258";
        }
    }
}
