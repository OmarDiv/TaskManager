using System.Security.Claims;
namespace Sehaty_Plus.Extention
{
    public static class UserExtensions
    {
        public static long GetUserId(this ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            return long.Parse(userId);
        }
    }
}
