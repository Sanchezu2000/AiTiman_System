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
using MongoDB.Driver;
using AiTiman_System.Models;
using AiTiman_System.Data;
using AiTIman_System.Areas.Identity.Data;
using AiTiman_System.Entities;

namespace AiTiman_System.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<AiTimanUser> _signInManager;
        private readonly UserManager<AiTimanUser> _userManager;
        private readonly IUserStore<AiTimanUser> _userStore;
        private readonly IUserEmailStore<AiTimanUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IMongoCollection<UserProfile> _userProfileCollection; // MongoDB Collection for UserProfile

        public RegisterModel(
            UserManager<AiTimanUser> userManager,
            IUserStore<AiTimanUser> userStore,
            SignInManager<AiTimanUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            IMongoDatabase database) // Inject MongoDB database
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _userProfileCollection = database.GetCollection<UserProfile>("UserProfiles"); // Initialize MongoDB collection
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

            [Display(Name = "License Number")]
            public string LicenseNumber { get; set; }

            [Display(Name = "Employee Number")]
            public string EmployeeNumber { get; set; }
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
                user.LicenseNumber = Input.LicenseNumber;
                user.EmployeeNumber = Input.EmployeeNumber;
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
                        "WELCOME Ka-AiTiman!",
                        $"Thank you for registering with AiTiman! To complete your account setup and verify your identity, please use the verification code below. " +
                        $"Your verification code is: {verificationCode}");

                    // Update the user with the verification code
                    await _userManager.UpdateAsync(user);

                    var userId = await _userManager.GetUserIdAsync(user);

                    // Automatically create and save the UserProfile
                    var age = user.Age;
                    string ageGroupClassification;

                    if (age < 18)
                    {
                        ageGroupClassification = "Minor";
                    }
                    else if (age > 59)
                    {
                        ageGroupClassification = "Senior";
                    }
                    else if (age >= 18 && age <= 59)
                    {
                        ageGroupClassification = "Adult";
                    }
                    else
                    {
                        ageGroupClassification = "Unknown";
                    }

                    var userProfile = new UserProfile
                    {
                        UserName = Input.userName,
                        FirstName = "First Name",
                        LastName = "Last Name",
                        CompleteAddress = Input.Address,
                        Age = user.Age.ToString(),
                        Email = Input.Email,
                        ProfilePictureUrl = "/images/Timan/user.png", // Default profile picture
                        SignPictureUrl = "/images/Timan-Img/dr_sign.png", // Default signature
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        Status = "Active",
                        UnverifiedRole = Input.Roles,
                        Address = Input.Address,
                        Birthdate = Input.Birthdate,
                        AgeGroupClassification = ageGroupClassification
                    };

                    // Save the UserProfile in the MongoDB collection
                    await _userProfileCollection.InsertOneAsync(userProfile);

                    // Logging to verify if the profile is saved
                    var savedProfile = await _userProfileCollection.Find(u => u.UserName == Input.userName).FirstOrDefaultAsync();

                    if (savedProfile == null)
                    {
                        _logger.LogError("UserProfile could not be saved to the database.");
                    }
                    else
                    {
                        _logger.LogInformation($"UserProfile saved successfully with UserName: {savedProfile.UserName}");
                    }

                    // Redirect to the VerifyCode page to complete verification
                    return RedirectToPage("VerifyCode", new { userId = userId });
                }

                foreach (var error in result.Errors)
                {
                    _logger.LogError("Error creating user: {Error}", error.Description);
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed; redisplay the form
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
