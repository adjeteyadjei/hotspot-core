namespace Hotvenues.Data
{
    public class Privileges
    {
        //Permissions on the User Model
        public const string UserCreate = "User.Create";
        public const string UserUpdate = "User.Update";
        public const string UserRead = "User.Read";
        public const string UserDelete = "User.Delete";

        //Permissions on the Role
        public const string RoleCreate = "Role.Create";
        public const string RoleUpdate = "Role.Update";
        public const string RoleRead = "Role.Read";
        public const string RoleDelete = "Role.Delete";

        //Permissions on systems configuration
        public const string MessagePortal = "MessagePortal";
        public const string Dashboard = "Dashboard";
        public const string Report = "Report";
        public const string Setting = "Setting";
        public const string Administration = "Administration";
    }

    public class AmountFormats
    {
        public static string Currency = "GHC##,###.00";
        public static string Amount = "##,###.00";
    }

    public class ConfigKeys
    {
        public static string SenderName = "SenderName";
        public static string ApiKey = "ApiKey";
    }
}
