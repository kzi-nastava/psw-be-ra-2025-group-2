using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Security.Claims;

namespace Explorer.Stakeholders.Infrastructure.Authentication;

public static class ClaimsPrincipalExtensions
{
    public static long PersonId(this ClaimsPrincipal user)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));

        var claim = user.FindFirst("personId"); 
        if (claim == null) throw new Exception("PersonId claim not found");
        
        return long.Parse(claim.Value);
    }

    public static long UserId(this ClaimsPrincipal user)
    {
        if(user == null) throw new ArgumentException(nameof(user));

        var claim = user.FindFirst("id");
        if (claim == null) throw new Exception("UserId claim not found");

        return long.Parse(claim.Value);
    }
}
