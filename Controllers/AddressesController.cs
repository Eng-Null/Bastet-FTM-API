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
    public class AddressesController : ControllerBase
    {
        private readonly IAddressesRepository repository;
        private readonly ILogger<AddressesController> logger;
        private readonly IMapper _mapper;

        public AddressesController(IAddressesRepository repo, ILogger<AddressesController> log, IMapper mapper)
        {
            repository = repo;
            logger = log;
            _mapper = mapper;
        }
        [HttpGet("{cId:Guid}/{aId:Guid}")]
        public async Task<IActionResult> GetAddressAsync(Guid cId, Guid aId)
        {
            var address = await repository.GetAddressAsync(cId, aId);

            if (address is null)
            {
                logger.LogInformation($"{DateTime.UtcNow:hh:mm:ss}: ({aId}) Address NotFound");
                return NotFound();
            }

            logger.LogInformation($"{DateTime.UtcNow:hh:mm:ss}: ({aId}) Address Returned");
            return Ok(address);
        }

        [HttpPost("{cId:Guid}")]
        public async Task<IActionResult> CreateAddressAsync(Guid cId, CreateAddressInfoDto addressDto)
        {
            AddressInfo address = new()
            {
                Id = Guid.NewGuid(),
                Address = addressDto.Address,
                City = addressDto.City,
                State = addressDto.State,
                Zip = addressDto.Zip
            };

            await repository.CreateAddressAsync(cId, address);
            logger.LogInformation($"{DateTime.UtcNow:hh:mm:ss}: Client Address Created");
            return CreatedAtAction(nameof(GetAddressAsync), new { cId, aId = address.Id }, _mapper.Map<AddressInfoDto>(address));
        }

        [HttpDelete("{cId:Guid}/{aId:Guid}")]
        public async Task<IActionResult> DeleteClientAddressAsync(Guid cId, Guid aId)
        {
            var existingAddress = await repository.GetAddressAsync(cId, aId);
            if (existingAddress is null)
            {
                logger.LogInformation($"{DateTime.UtcNow:hh:mm:ss}: Client's {cId} Not Found");
                return NotFound();
            }
            await repository.DeleteAddressAsync(cId, aId);

            var data = new List<AddressInfo>
            {
                new AddressInfo
                {
                    Id = existingAddress.Id,
                    Address= existingAddress.Address,
                    City = existingAddress.City,
                    State = existingAddress.State,
                    Zip = existingAddress.Zip
                }
            };
            var table = data.ToStringTable(u => u.Id,
                                                u => u.Address,
                                                u => u.City,
                                                u => u.State,
                                                u => u.Zip);

            logger.LogInformation($"{DateTime.UtcNow:hh:mm:ss}: Client's Address Deleted\n{table}");
            return Ok();
        }
    }
}
