using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using ElectronicStore.Ultilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using News.Models;
using News.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using FacebookClient = Facebook.FacebookClient;

namespace News.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ExternalLoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<ExternalLoginModel> _logger;
        private readonly IFacebookClient _facebookClient;
        private readonly IFacebookService _facebookService;

        public ExternalLoginModel(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ILogger<ExternalLoginModel> logger,
            IFacebookService facebookService,
            IFacebookClient facebookClient)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _facebookService = facebookService;
            _facebookClient = facebookClient;
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
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            public string Name { get; set; }

            public string Address { get; set; }

            [Required]
            public string Picture { get; set; }
            [Required]
            public string PhoneNumber { get; set; }

            [Required] public string Roles { get; set; } = SD.CustomerEndUser;
        }

        public IActionResult OnGetAsync()
        {
            return RedirectToPage("./Login");
        }

        public IActionResult OnPost(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToPage("./Login", new {ReturnUrl = returnUrl });
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor : true);
            if (result.Succeeded)
            {
                _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);
                return LocalRedirect(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return RedirectToPage("./Lockout");
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ReturnUrl = returnUrl;
                LoginProvider = info.LoginProvider;
                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                {
                    var jsonLocation = $"https://graph.facebook.com/v4.0/me?fields=location&access_token=EAAPnNKpsP1YBANRFZCeQKxUIuYRUmtPefBJIFZCTR2lxMqvJZBVyEN297HRBLWknFk72HJrLS15N8fzj08SSoUOvV9uqvSrn9ZCy41so0Ylwrqo095uP78GrnME1dVnXmWKTygtDwHdwYAX59VEFsVj3itHjQgSYKygoLQiH4uVmUgGSeNZCYFttZCGJ3H9dkZD";
                    var accessToken = "EAAPnNKpsP1YBAE9n3cMK9pDdRVHyb9Dnpie46RNsiH37wSylAGFsA0rKqA1FeBtNCk84IUW0xsDU4OPnBFWKSPnMtVnlWReH7JAJmV4ToAUBTnxQuLWKFHDMBPjf6iSFOkQ5ownHT9bCGFZAJ7sHTM0CGuPE518P19bCQTVf1sM4ZAGTsjM3bKAhBBPTwMYZBigGmxf4T0guAdp0ikonmZBC1dNZAPbsgyvkoZCcyxhAZDZD";
                    WebClient client = new WebClient();
                    Stream stream = client.OpenRead(jsonLocation);
                    StreamReader reader = new StreamReader(stream);
                    var jObject = JObject.Parse(reader.ReadLine());
                    var address = jObject["location"]["name"].ToString();
                    


                    Input = new InputModel
                    {
                        Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                        Name = info.Principal.FindFirstValue(ClaimTypes.Name),
                        Picture = $"https://graph.facebook.com/{info.Principal.FindFirstValue(ClaimTypes.NameIdentifier)}/picture?type=large",
                        PhoneNumber = info.Principal.FindFirstValue(ClaimTypes.MobilePhone),
                        Address = address
                    };
                }
                return Page();
            }
        }

        public async Task<IActionResult> OnPostConfirmationAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            // Get the information about the user from the external login provider
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information during confirmation.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            Input.Picture =
                $"https://graph.facebook.com/{info.Principal.FindFirstValue(ClaimTypes.NameIdentifier)}/picture?type=large";
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    PhoneNumber = Input.PhoneNumber,
                    Picture = Input.Picture,
                    Address = Input.Address,
                    isAdmin = false

                };
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            LoginProvider = info.LoginProvider;
            ReturnUrl = returnUrl;
            return Page();
        }
    }
}
