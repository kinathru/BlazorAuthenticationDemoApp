namespace BlazorAuthenticationDemoApp.Data.Users
{
    public class RoleDetail
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }

        public RoleDetail()
        {
        }

        public RoleDetail(string roleId, string roleName)
        {
            RoleId = roleId;
            RoleName = roleName;
        }

        public override bool Equals(object? obj)
        {
            return obj is RoleDetail detail &&
                   RoleId == detail.RoleId &&
                   RoleName == detail.RoleName;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(RoleId, RoleName);
        }
    }
}
