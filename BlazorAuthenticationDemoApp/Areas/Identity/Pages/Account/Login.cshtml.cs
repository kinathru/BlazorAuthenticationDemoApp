using BlazorAuthenticationDemoApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace BlazorAuthenticationDemoApp.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        // Here we need only the sign in manager service, and the user manager is not needed becase
        // we are not going to create any users here.
        private readonly SignInManager<IdentityUser> _signInManager;

        private readonly DataContext _ctx;
         
        public LoginModel(SignInManager<IdentityUser> signInManager, DataContext ctx)
        {
            _signInManager = signInManager;
            _ctx = ctx;
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

        public async Task<IActionResult> OnPostAsync() 
        {
            ReturnUrl = Url.Content("~/");

            if (ModelState.IsValid) 
            {
                var result = await _signInManager.PasswordSignInAsync(Input.UserName, Input.Password,
                                    isPersistent: false,lockoutOnFailure:false);

                if (result.Succeeded) 
                {
                    return LocalRedirect(ReturnUrl);
                }
            }

            return Page();//If the login is unsuccessful then show the same page to the user
        }

        public class InputModel
        {
            [Required]
            public string UserName { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }
    }
}
