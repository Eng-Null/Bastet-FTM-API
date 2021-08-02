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
    public class AddressesController : ControllerBase
    {
        private readonly IAddressesRepository repository;
        private readonly ILogger<AddressesController> logger;

        public AddressesController(IAddressesRepository repo, ILogger<AddressesController> log)
        {
            repository = repo;
            logger = log;
        }
        /// <summary>
        /// Create a new client address
        /// </summary>
        /// <param name="ClientDto"> new client address data as json</param>
        /// <returns></returns>
        [HttpPost("{clientId:Guid}")]
        public async Task<IActionResult> CreateAddressAsync(Guid clientId, CreateAddressInfoDto addressDto)
        {
            AddressInfo address = new()
            {
                Id = Guid.NewGuid(),
                Address = addressDto.Address,
                City = addressDto.City,
                State = addressDto.State,
                Zip = addressDto.Zip
            };

            await repository.CreateAddressAsync(clientId, address);
            logger.LogInformation($"{DateTime.UtcNow:hh:mm:ss}: Client Address Created");
            return Ok(address);
        }
        /// <summary>
        /// Delete Client Address
        /// </summary>
        /// <param name="clientId">Client ID</param>
        /// <param name="mobileId">Address ID</param>
        /// <returns></returns>
        [HttpDelete("{clientId:Guid}/{addressId:Guid}")]
        public async Task<IActionResult> DeleteClientAddressAsync(Guid clientId, Guid addressId)
        {
            var existingAddress = await repository.GetAddressAsync(clientId, addressId);
            if (existingAddress is null)
            {
                return NotFound();
            }

            await repository.DeleteAddressAsync(clientId, addressId);
            logger.LogInformation($"{DateTime.UtcNow:hh:mm:ss}: Client's Address Deleted");
            return Ok();
        }
    }
}
