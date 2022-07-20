using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;

namespace BlazorAuthenticationDemoApp.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        /// <summary>
        /// Constructor helps to inject the user manager and sign in manager with ASP.Net Core
        /// </summary>
        /// <param name="signInManager"></param>
        /// <param name="userManager"></param>
        public RegisterModel(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }
        public string ReturnUrl { get; set; }

        /// <summary>
        /// Gets executed when the request comes in for this page
        /// </summary>
        public void OnGet()
        {
            ReturnUrl = Url.Content("~/");
        }

        /// <summary>
        /// All the razor pages normally have the above OnGet and, OnPostAsync. 
        /// OnPostAsync is executed when the user posts something to the server
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostAsync() 
        {
            ReturnUrl = Url.Content("~/");
            if (ModelState.IsValid) 
            {
                var identity = new IdentityUser { UserName = Input.UserName, Email = Input.Email };
                var usercreateResult = await _userManager.CreateAsync(identity, Input.Password);

                var claim = new Claim("city", Input.City.ToLower());
                var claimsResult = await _userManager.AddClaimAsync(identity, claim);

                var role = new IdentityRole(Input.Role);
                var addRoleResult = await _roleManager.CreateAsync(role);

                var addUserToRoleResult = await _userManager.AddToRoleAsync(identity, Input.Role);

                if (usercreateResult.Succeeded && claimsResult.Succeeded
                    && ValidateAddRole(addRoleResult) && addUserToRoleResult.Succeeded)
                {
                    await _signInManager.SignInAsync(identity, isPersistent: false); // Here isPersistent is to tell the browser to persist the browser cookie for future or else to dump it when it's closed
                    return LocalRedirect(ReturnUrl); //Redirects to our home page    
                }
                else 
                {
                    bool errorsAfterUserCreation = false;

                    foreach (var error in usercreateResult.Errors) 
                    {
                        ModelState.AddModelError("RegistrationError", error.Description);
                    }
                    foreach (var error in claimsResult.Errors) 
                    {
                        ModelState.AddModelError("RegistrationError", error.Description);
                        errorsAfterUserCreation = true;
                    }
                    if (!ValidateAddRole(addRoleResult))
                    {
                        foreach (var error in addRoleResult.Errors)
                        {
                            ModelState.AddModelError("RegistrationError", error.Description);
                            errorsAfterUserCreation = true;
                        }
                    }
                    foreach (var error in addUserToRoleResult.Errors) 
                    {
                        ModelState.AddModelError("RegistrationError", error.Description);
                        errorsAfterUserCreation = true;
                    }

                    if (errorsAfterUserCreation) 
                    {
                        await _userManager.DeleteAsync(identity);
                    }

                    return Page();
                }
            }

            return Page();
        }

        public bool ValidateAddRole(IdentityResult? identityResult) 
        {
            if (identityResult != null) 
            {
                if (identityResult.Succeeded)
                {
                    return true;
                }
                else 
                {
                    if (identityResult.Errors != null ) 
                    {
                        foreach (var error in identityResult.Errors) 
                        {
                            if (error.Code.Contains("DuplicateRoleName"))
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public class InputModel 
        {
            [Required]
            public string UserName { get; set; }


            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Required]
            [Compare("Password", ErrorMessage = "Confirm password doesn't match, Type again !")]
            public string ConfirmPassword { get; set; }

            [Required]
            public string City { get; set; }

            [Required]
            public string Role { get; set; }
        }
    }
}
