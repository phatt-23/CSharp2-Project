using CoworkingApp.Models.DataModels;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace CoworkingApp.Models.Misc;



public static class ClaimsPrincipalExtensions
{
    public static bool IsAuthenticated(this ClaimsPrincipal claimsPrincipal) 
        => claimsPrincipal.Identity?.IsAuthenticated ?? false;

    public static int? GetUserId(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value.TryParseToInt();

    public static string? GetEmail(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.FindFirst(ClaimTypes.Email)?.Value;
}
