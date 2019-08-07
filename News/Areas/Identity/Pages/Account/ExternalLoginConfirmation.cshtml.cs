using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using News.Models.VIewModels;

namespace News.Areas.Identity.Pages.Account
{
    public class ExternalLoginConfirmationModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<ExternalLoginModel> _logger;

        public ExternalLoginConfirmationModel(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, ILogger<ExternalLoginModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }
	    [BindProperty]
        public InputModel Input { get; set; }

        public string LoginProvider { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            public ExternalLoginConfirmationViewModel ExternalLoginConfirmationVM { get; set; }

        }
        public IActionResult OnGet()
        {
            return RedirectToPage("./Login");
        }
        public IActionResult OnPost(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Page("./ExternalLoginConfirmation", pageHandler: "OnGetCallbackAsync", values: new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
            }
            var info = _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return Page();
            }
            // Sign in the user with this external login provider if the user already has a login.  
            var result = await _signInManager.ExternalLoginSignInAsync(info.Result.LoginProvider, info.Result.ProviderKey, isPersistent: false);
            if (result.Succeeded)
            {
                _logger.LogInformation(5, "User logged in with {Name} provider.", info.Result.LoginProvider);
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }
            if (result.RequiresTwoFactor)
            {
                return RedirectToPage("./LoginWith2fa", new {returnUrl = ReturnUrl});
            }
            if (result.IsLockedOut)
            {
                return RedirectToPage("./Lockout");
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.  
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["LoginProvider"] = info.Result.LoginProvider;
                var email = info.Result.Principal.FindFirstValue(ClaimTypes.Email);
                var name = info.Result.Principal.FindFirstValue(ClaimTypes.Name);
                var dob = info.Result.Principal.FindFirstValue(ClaimTypes.DateOfBirth);
                var gender = info.Result.Principal.FindFirstValue(ClaimTypes.Gender);
                var identifier = info.Result.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
                var picture = $"https://graph.facebook.com/{identifier}/picture?type=large";
                return RedirectToPage("./ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel
                {
                    Email = email, //User Email  
                    Name = name, //user Display Name  
                    DOB = dob.ToString(), //User DOB  
                    Gender = gender, //User Gender  
                    Picture = picture //User Profile Image  
                });
            }
        }
    }
}