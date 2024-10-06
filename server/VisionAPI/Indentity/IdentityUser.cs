using System.Security.Claims;
using Vision.Data.Interfaces;

namespace VisionAPI.Indentity;

public class IdentityUser : IUser
{
    private readonly IHttpContextAccessor _accessor;

    public IdentityUser(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    public string Name => _accessor.HttpContext!.User!.Identity!.Name!;

    public Guid? GetUserId()
    {
        return IsAuthenticated() ? Guid.Parse(_accessor.HttpContext!.User.GetUserId()!) : Guid.Empty;
    }

    public string GetUserEmail()
    {
        return IsAuthenticated() ? _accessor.HttpContext!.User.GetUserEmail() : "";
    }

    public string GetUserNome()
    {
        return IsAuthenticated() ? _accessor.HttpContext!.User.GetUserNome() : "";
    }

    public bool IsAuthenticated()
    {
        return _accessor.HttpContext!.User!.Identity!.IsAuthenticated;
    }

    public bool IsInRole(string role)
    {
        return _accessor.HttpContext!.User!.IsInRole(role);
    }

    public IEnumerable<Claim> GetClaimsIdentity()
    {
        return _accessor.HttpContext!.User.Claims;
    }
}

public static class ClaimsPrincipalExtensions
{
    public static string? GetUserId(this ClaimsPrincipal principal)
    {
        if (principal == null)
            throw new ArgumentException("Ocorreu um erro", nameof(principal));

        var claim = principal.FindFirst(ClaimTypes.NameIdentifier);

        return claim!.Value;
    }

    public static string GetUserEmail(this ClaimsPrincipal principal)
    {
        if (principal == null)
            throw new ArgumentException("Ocorreu um erro", nameof(principal));

        var claim = principal.FindFirst(ClaimTypes.Email);
        return claim!.Value;
    }

    public static string GetUserNome(this ClaimsPrincipal principal)
    {
        if (principal == null)
            throw new ArgumentException("Ocorreu um erro", nameof(principal));

        var claim = principal.FindFirst(ClaimTypes.Name);
        return claim!.Value;
    }
}
