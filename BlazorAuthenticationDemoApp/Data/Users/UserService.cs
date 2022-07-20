using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BlazorAuthenticationDemoApp.Data.Users
{
    public class UserService : IUserService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<List<UserDetail>> GetAllUsers()
        {
            List<UserDetail> UserDetails = new List<UserDetail>();
            var users = await _userManager.Users.ToListAsync();
            foreach (var user in users)
            {
                IList<string> roles = await _userManager.GetRolesAsync(user);
                IList<Claim> claims = await _userManager.GetClaimsAsync(user);

                UserDetail userDetail = GetUserDetail(user, roles, claims);
                UserDetails.Add(userDetail);

            }
            return UserDetails;
        }

        private static UserDetail GetUserDetail(IdentityUser user, IList<string> roles, IList<Claim> claims)
        {
            UserDetail userDetail = new UserDetail();
            userDetail.UserId = user.Id;
            userDetail.UserName = user.UserName;
            userDetail.PasswordHash = user.PasswordHash;
            userDetail.Email = user.Email;
            userDetail.PhoneNumber = user.PhoneNumber;
            userDetail.AccessFailedCount = user.AccessFailedCount;
            userDetail.LockoutEnabled = user.LockoutEnabled;
            userDetail.AssignedUserRoles = roles.ToList();
            foreach (var claim in claims)
            {
                userDetail.UserClaims.Add(claim.Type, claim.Value);
            }
            return userDetail;
        }

        public async Task<UserDetail> GetUser(string userId)
        {
            IdentityUser currentUser = await _userManager.FindByIdAsync(userId);
            IList<string> roles = await _userManager.GetRolesAsync(currentUser);
            IList<Claim> claims = await _userManager.GetClaimsAsync(currentUser);

            UserDetail userDetail = GetUserDetail(currentUser, roles, claims);

            return userDetail;
        }

        public async Task<bool> CreateUser(UserDetail userDetail)
        {
            IdentityUser user = new IdentityUser();
            user.UserName = userDetail.UserName;
            user.Email = userDetail.Email;
            user.PhoneNumber = userDetail.PhoneNumber;
            user.LockoutEnabled = userDetail.LockoutEnabled;

            var userCreateResult = await _userManager.CreateAsync(user, userDetail.NewPassword);

            if (userCreateResult.Succeeded)
            {
                foreach (var role in userDetail.AssignedUserRoles)
                {
                    await _userManager.AddToRoleAsync(user, role);
                }
                return true;
            }
            return false;
        }

        public async Task<bool> UpdateUser(string userId, UserDetail userDetail)
        {
            IdentityUser user = await _userManager.FindByIdAsync(userId);
            if (user != null) 
            {
                user.Email = userDetail.Email;
                user.PhoneNumber = userDetail.PhoneNumber;
                user.LockoutEnabled = userDetail.LockoutEnabled;

                var userUpdateResult = await _userManager.UpdateAsync(user);

                if (userUpdateResult.Succeeded)
                {
                    foreach (var role in userDetail.AssignedUserRoles)
                    {
                        await _userManager.AddToRoleAsync(user, role);
                    }
                    return true;
                }
            }

            return false;
        }
        
        public async Task<bool> UpdatePassword(string userId, UserDetail userDetail)
        {
            IdentityUser user = await _userManager.FindByIdAsync(userId);
            if (user != null) 
            {
                var userUpdateResult = await _userManager.ChangePasswordAsync(user, userDetail.OldPassword, userDetail.NewPassword);
                return userUpdateResult.Succeeded;
            }

            return false;
        }

        public async Task<bool> DeleteUser(string userId)
        {
            IdentityUser identityUser = await _userManager.FindByIdAsync(userId);
            if (identityUser != null) 
            {
                var result = await _userManager.DeleteAsync(identityUser);
                return result.Succeeded;
            }
            return false;
        }

        public async Task<HashSet<RoleDetail>> GetAllUserRoles()
        {
            HashSet<RoleDetail> roles = new HashSet<RoleDetail>();
            List<IdentityRole> identityRoles = await _roleManager.Roles.ToListAsync();
            foreach (IdentityRole role in identityRoles)
            {
                roles.Add(new RoleDetail(role.Id, role.Name));
            }
            return roles;
        }

        public async Task<bool> CreateRole(RoleDetail roleDetail)
        {
            var role = new IdentityRole(roleDetail.RoleName);
            var addRoleResult = await _roleManager.CreateAsync(role);
            return addRoleResult.Succeeded;
        }

        public async Task<bool> UpdateRole(RoleDetail roleDetail)
        {
            IdentityRole identityRole = await _roleManager.FindByIdAsync(roleDetail.RoleId);
            if (identityRole != null) 
            {
                identityRole.Name = roleDetail.RoleName;
                var roleUpdateResult = await _roleManager.UpdateAsync(identityRole);
                return roleUpdateResult.Succeeded;
            }
            return false;
        }

        public async Task<bool> DeleteRole(string roleId)
        {
            IdentityRole identityRole = await _roleManager.FindByIdAsync(roleId);
            if (identityRole != null)
            {
                var roleDeteleResult = await _roleManager.DeleteAsync(identityRole);
                return roleDeteleResult.Succeeded;
            }
            return false;
        }

        public async Task<bool> SetNewPassword(string userId, UserDetail userDetail)
        {
            IdentityUser user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var changePwdResult = await _userManager.RemovePasswordAsync(user);
                if (changePwdResult.Succeeded) 
                {
                    changePwdResult = await _userManager.AddPasswordAsync(user, userDetail.NewPassword);
                }
                return changePwdResult.Succeeded;
            }

            return false;
        }

        public async Task<UserDetail> GetUserByName(string username)
        {
            IdentityUser currentUser = await _userManager.FindByNameAsync(username);
            IList<string> roles = await _userManager.GetRolesAsync(currentUser);
            IList<Claim> claims = await _userManager.GetClaimsAsync(currentUser);

            UserDetail userDetail = GetUserDetail(currentUser, roles, claims);

            return userDetail;
        }
    }
}
