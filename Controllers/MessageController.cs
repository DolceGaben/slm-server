using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SLM.DTOs.Message;
using SLM.Models;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using Microsoft.EntityFrameworkCore;

namespace SLM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuthenticationContext db;
        public MessageController(UserManager<ApplicationUser> userManager, AuthenticationContext context)
        {
            _userManager = userManager;
            db = context;
        }
        [HttpGet]
        [Authorize]
        [Route("GetTenantList")]
        public async Task<IActionResult> GetTenantListAsync()
        {
         
            var tenants = from tenant in await _userManager.GetUsersInRoleAsync("Tenant")
                          select new { tenant.Email, tenant.FullName };

            return Ok(tenants.ToList());

        }

        [HttpGet]
        [Authorize]
        [Route("GetLandlordList")]
        public async Task<IActionResult> GetLandlordListAsync()
        {

            var landlords = from landlord in await _userManager.GetUsersInRoleAsync("Landlord")
                          select new { landlord.Email, landlord.FullName };

            return Ok(landlords.ToList());

        }

        [HttpPost]
        [Authorize]
        [Route("SendMessage")]
        public async Task<IActionResult> SendMessage(MessageInput input)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);


            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(user.FullName,user.Email));
            message.To.Add(new MailboxAddress("SLM",input.Email));
            message.Subject= "Subject "+input.Email;
            message.Body = new TextPart("plain")
            {
                Text = input.Message
            };
            using(var client = new SmtpClient())
            {
                client.CheckCertificateRevocation = false;
                client.Connect("smtp.gmail.com", 587, false);
                client.Authenticate("dolcegaben@gmail.com", "Peretzz+100500");
                client.Send(message);
                client.Disconnect(true);
            }
             
            return Ok( new { Succeeded = "Sent" });
        }
    }
}