using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SLM.DTOs.UserProfile;
using SLM.Models;

namespace SLM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public UserProfileController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        [HttpGet]
        [Authorize]
        //GET : /api/UserProfile

        public async Task<Object> GetUserProfile()
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);
            return new
            {
                user.FullName,
                user.Email,
                user.UserName
            };
        }
        [HttpPost]
        [Authorize]
        [Route("EditProfile")]
        //POST :/api/EditUserProfile
        public async Task<IActionResult> EditProfile(EditUserInput input)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                user.Email = input.Email;
                user.FullName = input.FullName;
                user.UserName = input.UserName;
               var result = await _userManager.UpdateAsync(user);
                return Ok(result);
            }
            return BadRequest(new { message = "Error by editing user"});
            
        }

    }
}