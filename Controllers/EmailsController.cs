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
    public class EmailsController : ControllerBase
    {
        private readonly IEmailRepository repository;
        private readonly ILogger<EmailsController> logger;

        public EmailsController(IEmailRepository repo, ILogger<EmailsController> log)
        {
            repository = repo;
            logger = log;
        }
        /// <summary>
        /// Create a new client EmailInfo
        /// </summary>
        /// <param name="ClientDto"> new client EmailInfo data as json</param>
        /// <returns></returns>
        [HttpPost("{clientId:Guid}")]
        public async Task<IActionResult> CreateEmailAsync(Guid clientId, CreateEmailInfoDto emailDto)
        {
            EmailInfo email = new()
            {
                Id = Guid.NewGuid(),
                Email = emailDto.Email
            };

            await repository.CreateEmailAsync(clientId, email);
            logger.LogInformation($"{DateTime.UtcNow:hh:mm:ss}: Client Email Created");
            return Ok(email);
        }
        /// <summary>
        /// Delete Client EmailInfo
        /// </summary>
        /// <param name="clientId">Client ID</param>
        /// <param name="emailId">EmailInfo ID</param>
        /// <returns></returns>
        [HttpDelete("{clientId:Guid}/{emailId:Guid}")]
        public async Task<IActionResult> DeleteClientEmailAsync(Guid clientId, Guid emailId)
        {
            var existingEmail = await repository.GetEmailAsync(clientId, emailId);

            if (existingEmail is null)
            {
                return NotFound();
            }

            await repository.DeleteEmailAsync(clientId, emailId);
            logger.LogInformation($"{DateTime.UtcNow:hh:mm:ss}: Client's Email Deleted");
            return Ok();
        }
    }
}
