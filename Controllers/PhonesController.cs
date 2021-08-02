using System;
using System.Threading.Tasks;
using BastetAPI.DTOs;
using BastetAPI.Entities;
using BastetFTMAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BastetFTMAPI.Controllers
{
    [Authorize(Roles = "Manager")]
    [Route("api/[controller]")]
    [ApiController]
    public class PhonesController : ControllerBase
    {
        private readonly IPhonesRepository repository;
        private readonly ILogger<PhonesController> logger;

        public PhonesController(IPhonesRepository repo, ILogger<PhonesController> log)
        {
            repository = repo;
            logger = log;
        }

        /// <summary>
        /// Create a new client Phone Number
        /// </summary>
        /// <param name="ClientDto"> new client Phone Number data as json</param>
        /// <returns></returns>
        [HttpPost("{clientId:Guid}")]
        public async Task<IActionResult> CreatePhoneAsync(Guid clientId, CreatePhoneInfoDto phoneDto)
        {
            PhoneInfo Phone = new()
            {
                Id = Guid.NewGuid(),
                Phone = phoneDto.Phone
            };

            await repository.CreatePhoneAsync(clientId, Phone);
            logger.LogInformation($"{DateTime.UtcNow:hh:mm:ss}: Client Phone Created");
            return Ok(Phone);
        }
        /// <summary>
        /// Delete Client Phone
        /// </summary>
        /// <param name="clientId">Client ID</param>
        /// <param name="mobileId">Phone ID</param>
        /// <returns></returns>
        [HttpDelete("{clientId:Guid}/{phoneId:Guid}")]
        public async Task<IActionResult> DeleteClientPhoneAsync(Guid clientId, Guid phoneId)
        {
            var existingPhone = await repository.GetPhoneAsync(clientId, phoneId);

            if (existingPhone is null)
            {
                return NotFound();
            }

            await repository.DeletePhoneAsync(clientId, phoneId);
            logger.LogInformation($"{DateTime.UtcNow:hh:mm:ss}: Client's Phone Deleted");
            return Ok();
        }
    }
}
