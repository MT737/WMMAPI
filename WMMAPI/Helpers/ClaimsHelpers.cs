using System;
using System.Security.Claims;
using static WMMAPI.Helpers.Globals.ErrorMessages;

namespace WMMAPI.Helpers
{
    public static class ClaimsHelpers
    {
        public static Guid GetUserId(Guid userId, ClaimsPrincipal user)
        {
            return userId = user is not null
                ? Guid.Parse(user.Identity.Name) // Default happy path
                : userId != Guid.Empty
                    ? userId // Allows for setting a user Id for testing purposes
                    : throw new UnauthorizedAccessException(AuthenticationError);
        }
    }
}
