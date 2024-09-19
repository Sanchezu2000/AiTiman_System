using AiTiman_System.Areas.Identity.Data;
using AiTiman_System.Entities;
using AiTiman_System.Models;
using AiTiman_System.Services;
using AiTIman_System.Areas.Identity.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AiTiman_System.Controllers
{
    public class AdminController : Controller
    {
        private readonly SignInManager<AiTimanUser> _signInManager;
        private readonly UserManager<AiTimanUser> _userManager;
        private readonly ILogger<AdminController> _logger;
        private readonly UserProfileService _userProfileService;

        public AdminController(SignInManager<AiTimanUser> signInManager,
                               UserManager<AiTimanUser> userManager,
                               ILogger<AdminController> logger,
                               UserProfileService userProfileService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _userProfileService = userProfileService;
        }

        public IActionResult AdminDashboard()
        {
            return View();
        }

        public IActionResult AdminEditProfile()
        {
            return View();
        }

        public async Task<IActionResult> AdminProfile()
        {
            // Get the currently logged-in user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found");
            }

            // Fetch the user's profile using user.Id instead of userName
            var userProfile = await _userProfileService.GetUserProfile(user.Id);
            if (userProfile == null)
            {
                return NotFound("User profile not found");
            }

            // Map the user profile to the ProfileModel
            var model = MapUserProfileToModel(userProfile);

            return View(model);
        }

        public ProfileModel MapUserProfileToModel(UserProfile userProfile)
        {
            return new ProfileModel
            {
                UserName = userProfile.UserName,
                FirstName = userProfile.FirstName,
                LastName = userProfile.LastName,
                CompleteAddress = userProfile.CompleteAddress,
                Religion = userProfile.Religion,
                PhoneNumber = userProfile.PhoneNumber,
                Gender = userProfile.Gender,
                Status = userProfile.Status,
                Email = userProfile.Email,
                Address = userProfile.Address,
                UnverifiedRole = userProfile.UnverifiedRole,
                GuardianName = userProfile.GuardianName,
                ProfilePictureUrl = userProfile.ProfilePictureUrl,
                SignPictureUrl = userProfile.SignPictureUrl,
                CreatedAt = userProfile.CreatedAt,
                UpdatedAt = userProfile.UpdatedAt,
                Birthdate = userProfile.Birthdate
            };
        }
    }
}
