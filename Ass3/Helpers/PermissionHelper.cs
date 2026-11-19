namespace Ass3.Helpers;

public static class PermissionHelper
{
    public static bool IsInRole(params string[] roles) => SessionManager.User is { } user && roles.Contains(user.RoleName, StringComparer.OrdinalIgnoreCase);
    public static void Demand(params string[] roles)
    {
        if (!IsInRole(roles)) throw new UnauthorizedAccessException("You do not have permission to perform this action.");
    }
}
