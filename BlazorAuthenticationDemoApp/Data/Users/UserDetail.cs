using Microsoft.AspNetCore.Identity;
using System.Text;

namespace BlazorAuthenticationDemoApp.Data.Users
{
    public class UserDetail
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string? OldPassword { get; set; }
        public string? NewPassword { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }

        public List<string> AssignedUserRoles { get; set; } = new List<string>();
        public List<string> UnassignedUserRoles { get; set; } = new List<string>();
        public Dictionary<string, string> UserClaims { get; set; } = new Dictionary<string, string>();

        public string GetUserRolesString() 
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < AssignedUserRoles.Count; i++) 
            {
                sb.Append(AssignedUserRoles[i]);
                if (i != AssignedUserRoles.Count - 1) 
                {
                    sb.Append(Environment.NewLine);
                }
            }
            return sb.ToString();
        }

        public string GetUserClaimsString() 
        {
            int count = 0;
            StringBuilder sb = new StringBuilder();
            foreach (string claimCode in UserClaims.Keys) 
            {
                sb.Append(claimCode).Append("\t-\t").Append(UserClaims[claimCode]);
                if (count++ != UserClaims.Count - 1) 
                {
                    sb.Append(Environment.NewLine);
                }
            }
            return sb.ToString();
        }


    }
}
