using Microsoft.AspNetCore.Identity;
using TaskManager.Domain.Enums;
namespace TaskManager.Domain.Entities
{
    public class ApplicationUser : IdentityUser<long>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public Gender Gender { get; set; }
        public DateOnly RegisteredDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        public bool IsActive { get; set; } = true;
        public string? ProfilePicture { get; set; }

        public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
        public ICollection<Project> Projects { get; set; } = [];
    }
}



