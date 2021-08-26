using System;
using System.Net;
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
    [Authorize(Roles = "Manager, Owner")]
    [Route("api/[controller]")]
    [ApiController]
    public class PhonesController : ControllerBase
    {
        private readonly IPhonesRepository repository;
        private readonly ILogger<PhonesController> logger;
        private readonly IMapper _mapper;

        public PhonesController(IPhonesRepository repo, ILogger<PhonesController> log, IMapper mapper)
        {
            repository = repo;
            logger = log;
            _mapper = mapper;
        }

        [HttpGet("{cId:Guid}/{pId:Guid}")]
        public async Task<IActionResult> GetPhoneAsync(Guid cId, Guid pId)
        {
            var address = await repository.GetPhoneAsync(cId, pId);

            if (address is null)
            {
                logger.LogInformation($"{DateTime.UtcNow:hh:mm:ss}: ({pId}) Address NotFound");
                return NotFound();
            }

            logger.LogInformation($"{DateTime.UtcNow:hh:mm:ss}: ({pId}) Address Returned");
            return Ok(address);
        }

        [HttpPost("{cId:Guid}")]
        public async Task<IActionResult> CreatePhoneAsync(Guid cId, CreatePhoneInfoDto phoneDto)
        {
            PhoneInfo phone = new()
            {
                Id = Guid.NewGuid(),
                Phone = phoneDto.Phone
            };

            await repository.CreatePhoneAsync(cId, phone);
            logger.LogInformation($"{DateTime.UtcNow:hh:mm:ss}: Client Phone Created");
            return CreatedAtAction(nameof(GetPhoneAsync), new { cId, pId = phone.Id }, _mapper.Map<PhoneInfoDto>(phone));
        }

        [HttpDelete("{cId:Guid}/{pId:Guid}")]
        public async Task<IActionResult> DeleteClientPhoneAsync(Guid cId, Guid pId)
        {
            var existingPhone = await repository.GetPhoneAsync(cId, pId);

            if (existingPhone is null)
            {
                logger.LogInformation($"{DateTime.UtcNow:hh:mm:ss}: Client's Phone Not Found");
                return NotFound();
            }

            await repository.DeletePhoneAsync(cId, pId);
            logger.LogInformation($"{DateTime.UtcNow:hh:mm:ss}: Client's Phone Deleted");
            return Ok();
        }
    }
}
