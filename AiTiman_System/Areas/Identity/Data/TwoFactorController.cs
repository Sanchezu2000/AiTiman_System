using AiTIman_System.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

public class TwoFactorController : Controller
{
    private readonly UserManager<AiTimanUser> _userManager;
    private readonly SignInManager<AiTimanUser> _signInManager;

    public TwoFactorController(UserManager<AiTimanUser> userManager, SignInManager<AiTimanUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet]
    public IActionResult VerifyCode(string userId)
    {
        ViewBag.UserId = userId;
        return View();  // This will look for VerifyCode.cshtml in Views/TwoFactor/
    }

    [HttpPost]
    public async Task<IActionResult> VerifyCode(string userId, string code)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null && user.VerificationCode == code)
        {
            user.Roles.Clear();
            user.Roles.Add(user.UnverifiedRole);
            user.IsTwoFactorEnabled = true;
            user.VerificationCode = null;
            user.UnverifiedRole = null;              //Clear the code after successful verification
            await _userManager.UpdateAsync(user);

            // Redirect to the login page within the Identity area
            return RedirectToAction("Login", "Account");
        }

        // If the code is invalid, add an error message and redisplay the verification page
        ModelState.AddModelError(string.Empty, "Invalid verification code.");
        ViewBag.UserId = userId; // Pass the userId back to the view
        return View(); // This will look for VerifyCode.cshtml in Views/TwoFactor/
    }
}