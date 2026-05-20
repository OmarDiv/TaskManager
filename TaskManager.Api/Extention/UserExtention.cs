using System.Security.Claims;
namespace TaskManager.Api.Extention
{
    public static class UserExtensions
    {
        public static long GetUserId(this ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub");
            return userId is null ? 0 : long.Parse(userId);
        }
    }
}
