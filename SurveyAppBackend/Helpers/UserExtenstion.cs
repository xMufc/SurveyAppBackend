using System.Security.Claims;

namespace SurveyAppBackend.Helpers
{
    public static class UserExtenstion
    {
        /// <summary>
        /// Helper method to extract the user ID from the claims of the authenticated user
        /// </summary>
        /// <param name="user">The claims principal representing the authenticated user</param>
        /// <returns>The user ID if available; otherwise, null</returns>
        public static string? GetUserId(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
