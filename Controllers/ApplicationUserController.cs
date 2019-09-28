using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SLM.DTOs.ApplicationUser;
using SLM.Models;

namespace SLM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationUserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationSettings _applicationSettings;

        public ApplicationUserController(
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager,
            IOptions<ApplicationSettings> appSettings
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _applicationSettings = appSettings.Value;

        }

        [HttpPost]
        [Route("Register")]
        //POST : /api/ApplicationUser/Register
        public async Task <Object> PostApplicationUser(ApplicationUserInput input)
        {
             
            var applicationUser = new ApplicationUser() {
                UserName = input.UserName,
                Email = input.Email,
                FullName = input.FullName
          
            };
            try
            {
                var result = await _userManager.CreateAsync(applicationUser, input.Password);
                await _userManager.AddToRoleAsync(applicationUser, input.Role);
                return Ok(result);
            }
            catch(Exception ex)
            {
                throw ex;
            }

        }
        [HttpPost]
        [Route("Login")]
        //POST : /api/ApplicationUser/Login

        public async Task<IActionResult> Login(LoginInput input)
        {
            var user = await _userManager.FindByNameAsync(input.UserName);
            if (user != null && await _userManager.CheckPasswordAsync(user,input.Password))
            {
                var role = await _userManager.GetRolesAsync(user);
                IdentityOptions _identityOptions = new IdentityOptions();

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim("UserID",user.Id.ToString()),
                        new Claim(_identityOptions.ClaimsIdentity.RoleClaimType,role.FirstOrDefault())
                    }),
                    Expires = DateTime.UtcNow.AddHours(6),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_applicationSettings.JWT_Secret.ToString())), SecurityAlgorithms.HmacSha256Signature)
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                var token = tokenHandler.WriteToken(securityToken);
                return Ok(new { token });
            }
            else
            {
                return BadRequest(new { message = "Username or password is incorrect" });
            }

        }


    }
}