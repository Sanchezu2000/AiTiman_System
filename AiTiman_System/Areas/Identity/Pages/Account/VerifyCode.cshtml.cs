using AiTiman_System.Areas.Identity.Data;
using AiTIman_System.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace AiTiman_System.Areas.Identity.Pages.Account
{
    public class VerifyCodeModel : PageModel
    {
        private readonly UserManager<AiTimanUser> _userManager;
        private readonly SignInManager<AiTimanUser> _signInManager;

        public VerifyCodeModel(UserManager<AiTimanUser> userManager, SignInManager<AiTimanUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public string Code { get; set; }

        [BindProperty(SupportsGet = true)]
        public string UserId { get; set; }

        public void OnGet(string userId)
        {
            UserId = userId;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(UserId) || string.IsNullOrEmpty(Code))
            {
                ModelState.AddModelError(string.Empty, "Invalid verification attempt.");
                return Page();
            }

            var user = await _userManager.FindByIdAsync(UserId);
            if (user != null && user.VerificationCode == Code)
            {
                // Update user roles and other properties
                user.Roles.Clear();
                user.Roles.Add(user.UnverifiedRole);
                user.IsTwoFactorEnabled = true;
                user.VerificationCode = null; // Clear the code after successful verification
                await _userManager.UpdateAsync(user);

                // Sign the user in and redirect to the dashboard
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToPage("/Dashboard");
            }

            // If the code is invalid, redisplay the form with an error message
            ModelState.AddModelError(string.Empty, "Invalid verification code.");
            return Page();
        }
    }
}
