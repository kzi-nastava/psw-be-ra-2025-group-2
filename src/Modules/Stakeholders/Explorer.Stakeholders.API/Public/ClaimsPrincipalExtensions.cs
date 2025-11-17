using System;
using System.Security.Claims;

namespace Explorer.Stakeholders.API.Public
{
    public static class ClaimsPrincipalExtensions
    {
        public static long PersonId(this ClaimsPrincipal user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            var claim = user.FindFirst("person_id"); 
            if (claim == null) throw new Exception("PersonId claim not found");

            return long.Parse(claim.Value);
        }
    }
}
