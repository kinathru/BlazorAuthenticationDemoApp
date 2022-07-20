using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace BlazorAuthenticationDemoApp.Services
{
    public class IdentityValidationProvider<TUser> : RevalidatingServerAuthenticationStateProvider where TUser : class
    {
        public readonly IServiceScopeFactory _scopeFactory; // To get a scoped instance of the User Manager Service
        public readonly IdentityOptions _options; 

        public IdentityValidationProvider(ILoggerFactory loggerFactory,
            IServiceScopeFactory scopeFactory, 
            IOptions<IdentityOptions> optionsAccessor  ) : base(loggerFactory)
        {
            _scopeFactory = scopeFactory;
            _options = optionsAccessor.Value;
        }

        // Interval in which it periodically checks if the Auth State has changed
        protected override TimeSpan RevalidationInterval => TimeSpan.FromSeconds(30);

        protected override async Task<bool> ValidateAuthenticationStateAsync(AuthenticationState authenticationState, CancellationToken cancellationToken)
        {
            // Get the user manager from a new scope to ensure it fetches fresh data
            var scope = _scopeFactory.CreateScope();
            try
            {
                var usermanager = scope.ServiceProvider.GetRequiredService<UserManager<TUser>>();
                return await ValidateSecurityStampAsync(usermanager, authenticationState.User);
            }
            finally 
            {
                if (scope is IAsyncDisposable asyncDisposable)
                {
                    await asyncDisposable.DisposeAsync();
                }
                else 
                {
                    scope.Dispose();
                }
            }
        }

        private async Task<bool> ValidateSecurityStampAsync(UserManager<TUser> usermanager, ClaimsPrincipal principal)
        {
            var user = await usermanager.GetUserAsync(principal); // Get the user from the database
            if (user == null)
            {
                return false; // There is no such user
            }
            else if (!usermanager.SupportsUserSecurityStamp)
            {
                return true;
            }
            else 
            {
                var principalStamp = principal.FindFirstValue(_options.ClaimsIdentity.SecurityStampClaimType);
                var userStamp = await usermanager.GetSecurityStampAsync(user);
                return principalStamp == userStamp;
            }
        }
    }
}
