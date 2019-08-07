using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ElectronicStore.Ultilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using News.Models;

namespace News.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly HostingEnvironment _hostingEnvironment;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager,
            HostingEnvironment hostingEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _hostingEnvironment = hostingEnvironment;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [Display(Name = "Phone Number")]
            public string PhoneNumber { get; set; }

            [Required]
            [Display(Name = "Address")]
            public string Address { get; set; }

            [Required]
            [Display(Name = "Full Name")]
            public string Name { get; set; }

            [Required]
            [Display(Name = "Admin")]
            public bool isAdmin { get; set; }

            [Display(Name = "Avatar")]
            public string Picture { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            if (files.Count != 0)
            {
                var uploads = Path.Combine(webRootPath, SD.ImageFolderPost);
                var extension = Path.GetExtension(files[0].FileName);

                using (var filestream = new FileStream(Path.Combine(uploads, Input.Name + extension), FileMode.Create))
                {
                    files[0].CopyTo(filestream);
                }

                Input.Picture = @"\" + SD.ImageFolderPost + @"\" + Input.Name + extension;
            }
            else
            {
                //when user does not upload image
                var uploads = Path.Combine(webRootPath, SD.ImageUserFolder + @"\" + SD.DefaultUserImage);
                System.IO.File.Copy(uploads, webRootPath + @"\" + SD.ImageUserFolder + @"\" + Input.Name + ".png");
                Input.Picture = @"\" + SD.ImageUserFolder + @"\" + Input.Name + ".png";
            }

            if (ModelState.IsValid)
            {
                var user = new User { UserName = Input.Email, Email = Input.Email,PhoneNumber = Input.PhoneNumber,Address = Input.Address,isAdmin = Input.isAdmin, Name = Input.Name,Picture = Input.Picture};
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {

                    if (!await _roleManager.RoleExistsAsync(SD.AdminEndUser))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.AdminEndUser));
                    }
                    if (!await _roleManager.RoleExistsAsync(SD.CustomerEndUser))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.CustomerEndUser));
                    }

                    if (Input.isAdmin)
                    {
                        await _userManager.AddToRoleAsync(user, SD.AdminEndUser);
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, SD.CustomerEndUser);
                    }


                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { userId = user.Id, code = code },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
