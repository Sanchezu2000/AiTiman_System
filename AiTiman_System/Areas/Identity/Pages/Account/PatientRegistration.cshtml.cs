using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using AiTiman_System.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using AiTiman_System.Attributes;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;  // For MongoClient and IMongoCollection
using AiTiman_System.Models;
using AiTiman_System.Data;
using AiTIman_System.Areas.Identity.Data;
using Amazon.Runtime.Internal.Endpoints.StandardLibrary;

namespace AiTiman_System.Areas.Identity.Pages.Account
{
    public class PatientRegistration : PageModel
    {
        private readonly SignInManager<AiTimanUser> _signInManager;
        private readonly UserManager<AiTimanUser> _userManager;
        private readonly IUserStore<AiTimanUser> _userStore;
        private readonly IUserEmailStore<AiTimanUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public PatientRegistration(
            UserManager<AiTimanUser> userManager,
            IUserStore<AiTimanUser> userStore,
            SignInManager<AiTimanUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

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
            [Display(Name = "userName")]
            public string userName { get; set; }

            [Required]
            [DataType(DataType.Date)]
            [Display(Name = "Birthdate")]
            public DateTime Birthdate { get; set; }

            public int Age => DateTime.Now.Year - Birthdate.Year - (DateTime.Now.DayOfYear < Birthdate.DayOfYear ? 1 : 0);

            [Required]
            [Display(Name = "Address")]
            public string Address { get; set; }

            [Required]
            [Display(Name = "Roles")]
            public string Roles { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.userName, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                user.Birthdate = Input.Birthdate;
                user.Address = Input.Address;
                user.Age = DateTime.Now.Year - Input.Birthdate.Year;
                user.EmailConfirmed = true;

                // Set the user's initial role to "PendingVerification"
                user.Roles.Add("PendingVerification");

                // Store the desired role in a temporary field for verification (e.g., "Patient", "HealthProvider", or "Admin")
                user.UnverifiedRole = Input.Roles;

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    // Generate and store the verification code
                    var verificationCode = GenerateVerificationCode();
                    user.VerificationCode = verificationCode;

                    // Send the verification code to the user via email
                    await _emailSender.SendEmailAsync(
                        Input.Email,
                        "Thank you for registering with AiTiman! To complete your account setup and verify your identity, please use the verification code below. Enter the code on the verification page to finalize your registration. If you didn't request this, please disregard this message.",
                        $"Your verification code is: {verificationCode}");

                    // Update the user with the verification code
                    await _userManager.UpdateAsync(user);

                    var userId = await _userManager.GetUserIdAsync(user);

                    // Redirect to the VerifyCode page to complete verification
                    return RedirectToPage("VerifyCode", new { userId = userId });
                }

                foreach (var error in result.Errors)
                {
                    _logger.LogError("Error creating user: {Error}", error.Description);
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay the form
            return Page();
        }





        private AiTimanUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<AiTimanUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(AiTimanUser)}'. " +
                    $"Ensure that '{nameof(AiTimanUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<AiTimanUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<AiTimanUser>)_userStore;
        }

        // Helper method to generate a unique verification code
        private string GenerateVerificationCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString(); // Generates a 6-digit code
        }
    }
}
