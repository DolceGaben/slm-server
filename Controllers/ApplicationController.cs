using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SLM.DTOs.Application;
using SLM.Models;

namespace SLM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuthenticationContext db;
        public ApplicationController(UserManager<ApplicationUser> userManager, AuthenticationContext context)
        {
            _userManager = userManager;
            db = context;
        }
        [HttpGet]
        [Authorize]
        [Route("GetLandlordApplications")]
        public async Task<IActionResult> GetLandlordApplicationsListAsync(string status)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);

            var myApplications = db.Applications.Where(x => x.Status == status && x.LandlordId == user.Id);

            var result = from app in myApplications
                         join house in db.Houses on app.HouseId equals house.Id
                         join person in _userManager.Users on app.TenantId equals person.Id
                         select new
                         {
                             Id = app.Id,
                             TenantName = person.FullName,
                             HouseName = house.Name,
                             status = app.Status
                                
                         };
            return Ok(result.ToList());

        }

        [HttpPost]
        [Authorize]
        [Route("SendApplication")]
        public async Task<IActionResult> SendApplicationAsync(House house)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);
            if (db.Applications.FirstOrDefault(x => x.HouseId == house.Id) == null){
                Application application = new Application { TenantId = user.Id, LandlordId = house.LandlordsId, Status = "waiting", HouseId = house.Id };
                db.Applications.Add(application);
                await db.SaveChangesAsync();
            }
            else
            {
                return BadRequest(new { Error = "This application already exist" });
            }

            return Ok(new { Succeeded = "Created" });

        }

        [HttpPost]
        [Authorize(Roles ="Landlord")]
        [Route("UpdateApplication")]
        public IActionResult UpdateApplication(ApplicationInput input)
        {
            var myApplication = db.Applications.FirstOrDefault(x => x.Id == input.Id);
            myApplication.Status = input.Status;
            db.Applications.Update(myApplication);
            db.SaveChanges();
            return Ok(new { Succeeded = "Updated" });

        }





    }
}