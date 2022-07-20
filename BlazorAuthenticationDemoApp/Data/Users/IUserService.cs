using Microsoft.AspNetCore.Identity;

namespace BlazorAuthenticationDemoApp.Data.Users
{
    public interface IUserService
    {
        Task<List<UserDetail>> GetAllUsers();

        Task<UserDetail> GetUser(string userId);
        Task<UserDetail> GetUserByName(string username);
        Task<bool> CreateUser(UserDetail userDetail);
        Task<bool> UpdateUser(string userId, UserDetail userDetail);
        Task<bool> UpdatePassword(string userId, UserDetail userDetail);
        Task<bool> SetNewPassword(string userId, UserDetail userDetail);
        Task<bool> DeleteUser(string userId);
        Task<HashSet<RoleDetail>> GetAllUserRoles();

        Task<bool> CreateRole(RoleDetail roleDetail);
        Task<bool> UpdateRole(RoleDetail userDetail);
        Task<bool> DeleteRole(string roleId);

    }
}
