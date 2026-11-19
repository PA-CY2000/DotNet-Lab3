namespace Ass3.Helpers;

public sealed record CurrentUser(int UserId, string FullName, string Username, string RoleName, int? DeskId);

public static class SessionManager
{
    public static CurrentUser? User { get; private set; }
    public static bool IsAuthenticated => User is not null;
    public static void SignIn(CurrentUser user) => User = user;
    public static void SignOut() => User = null;
}
