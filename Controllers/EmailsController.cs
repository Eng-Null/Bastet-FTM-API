using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BastetAPI.DTOs;
using BastetAPI.Entities;
using BastetFTMAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BastetFTMAPI.Controllers
{
    [Authorize(Roles = "Manager, Owner, Guest")]
    [Route("api/[controller]")]
    [ApiController]
    public class EmailsController : ControllerBase
    {
        private readonly IEmailRepository repository;
        private readonly ILogger<EmailsController> logger;
        private readonly IMapper _mapper;

        public EmailsController(IEmailRepository repo, ILogger<EmailsController> log, IMapper mapper)
        {
            repository = repo;
            logger = log;
            _mapper = mapper;
        }

        [HttpGet("{cId:Guid}/{eId:Guid}")]
        public async Task<IActionResult> GetEmailAsync(Guid cId, Guid eId)
        {
            var email = await repository.GetEmailAsync(cId, eId);

            if (email is null)
            {
                logger.LogInformation($"{DateTime.UtcNow:hh:mm:ss}: ({eId}) Email NotFound");
                return NotFound();
            }

            logger.LogInformation($"{DateTime.UtcNow:hh:mm:ss}: ({eId}) Email Returned");
            return Ok(email);
        }

        [HttpPost("{cId:Guid}")]
        public async Task<IActionResult> CreateEmailAsync(Guid cId, CreateEmailInfoDto emailDto)
        {
            EmailInfo email = new()
            {
                Id = Guid.NewGuid(),
                Email = emailDto.Email
            };

            await repository.CreateEmailAsync(cId, email);
            logger.LogInformation($"{DateTime.UtcNow:hh:mm:ss}: Client Email Created");
            return CreatedAtAction(nameof(GetEmailAsync), new { cId, eId = email.Id }, _mapper.Map<EmailInfoDto>(email));
        }

        [HttpDelete("{cId:Guid}/{eId:Guid}")]
        public async Task<IActionResult> DeleteClientEmailAsync(Guid cId, Guid eId)
        {
            var existingEmail = await repository.GetEmailAsync(cId, eId);

            if (existingEmail is null)
            {
                logger.LogInformation($"{DateTime.UtcNow:hh:mm:ss}: ({cId}) Email NotFound");
                return NotFound();
            }
            await repository.DeleteEmailAsync(cId, eId);
            
            var data = new List<EmailInfo>
            {
                new EmailInfo
                {
                    Id = existingEmail.Id,
                    Email = existingEmail.Email
                }
            };
            var table = data.ToStringTable(u => u.Id,
                                                u => u.Email);
            logger.LogInformation($"{DateTime.UtcNow:hh:mm:ss}: Client's Email Deleted\n{table}");
            return Ok();
        }
    }
}
