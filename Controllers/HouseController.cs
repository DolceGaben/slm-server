using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SLM.DTOs.House;
using SLM.Models;

namespace SLM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HouseController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuthenticationContext db;
        public HouseController(UserManager<ApplicationUser> userManager, AuthenticationContext context)
        {
            _userManager = userManager;
            db = context;
        }

        [HttpPost]
        [Authorize]
        [Route("CreateHouse")]
        public async Task<IActionResult> CreateHouse(CreateOrUpdateHouseInput input)
            {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);
            if(user != null)
            {
                House myHouse = new House { LandlordsId = user.Id, Address = input.Address, Name = input.Name };
                await db.AddAsync(myHouse);
                await db.SaveChangesAsync();
                return Ok(new { Succeeded = "Success" });

            }

            return BadRequest();
        }


        [HttpPost]
        [Authorize]
        [Route("UpdateHouse")]
        public async Task<IActionResult> UpdateHouse(CreateOrUpdateHouseInput input)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var myHouse = db.Houses.FirstOrDefault(x => x.Id == input.Id);
                myHouse.Name = input.Name;
                myHouse.Address = input.Address;
                db.Update(myHouse);
                await db.SaveChangesAsync();
                return Ok(new { Succeeded = "Success" });

            }

            return BadRequest();
        }


        [HttpGet]
        [Authorize]
        [Route("GetLandlordHouses")]
        public async Task<List<House>> GetHouses(string filter)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);

            List<House> houses = new List<House>();
            if (!String.IsNullOrEmpty(filter))
            {
                houses = db.Houses.Where(x => x.LandlordsId == user.Id && x.Name.Contains(filter) || x.Address.Contains(filter)).ToList();
            }
            else
            {
                houses = db.Houses.Where(x => x.LandlordsId == user.Id).ToList();
            }

            return houses;

        }

        [HttpGet]
        [Authorize]
        [Route("GetTenantHouses")]
        public async Task<List<House>> GetTenantHouses(string filter)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);

            List<House> houses = new List<House>();
     
                if (!String.IsNullOrEmpty(filter))
                {
                    houses = db.Houses.Where(x =>  x.Name.Contains(filter) || x.Address.Contains(filter)).ToList();
                }
                else
                {
                    houses = db.Houses.ToList();
                }

            return houses;

        }

        [HttpGet]
        [Authorize]
        [Route("GetTenantAcceptedHouses")]
        public async Task<IActionResult> GetTenantAcceptedHouses(string filter)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);

            List<House> houses = new List<House>();

            if (!String.IsNullOrEmpty(filter))
            {
                houses = db.Houses.Where(x => x.Name.Contains(filter) || x.Address.Contains(filter)).ToList();
            }
            else
            {
                houses = db.Houses.ToList();
            }

            var result = from house in houses
                         join app in db.Applications on house.Id equals app.HouseId
                         join tenant in _userManager.Users on app.TenantId equals tenant.Id
                         where app.Status == "accepted"
                         select new
                         {
                             Id = house.Id,
                             Name = house.Name,
                             Status = app.Status,
                             Address = house.Address

                         };

            return Ok(result.ToList());

        }

        [HttpGet]
        [Authorize]
        [Route("GetHouseById")]
        public IActionResult GetHouseById(int id)
        {
            var myHouse =  db.Houses.FirstOrDefault(x=>x.Id == id);

            return Ok(myHouse);
        }

        [HttpGet]
        [Authorize]
        [Route("DeleteHouse")]
        public IActionResult DeleteHouse(int id)
        {
            var myHouse = db.Houses.FirstOrDefault(x => x.Id == id);

            db.Houses.Remove(myHouse);
            db.SaveChanges();

            return Ok(new { Succeeded = "Success" });
        }


    }
}