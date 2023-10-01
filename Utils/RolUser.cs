namespace MagicVillaApi.Utils
{
    public enum RolUser
    {
        Admin = 0,
        User = 1,
        Guest = 2
    }

    public static class RolUserExtensions
    {
        public static string GetRoleName(this RolUser role)
        {
            switch (role)
            {
                case RolUser.Admin:
                    return "Admin";
                case RolUser.User:
                    return "User";
                case RolUser.Guest:
                    return "Guest";
                default:
                    throw new ArgumentOutOfRangeException(nameof(role));
            }
        }
    }
}
