using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using BastetAPI.DTOs;
using BastetAPI.Entities;
using BastetAPI.Repositories;
using BastetFTMAPI.Parameters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BastetAPI.Controllers
{   //[Authorize(Roles = "Manager")]
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly IClientsRepository repository;
        private readonly ILogger<ClientsController> logger;
        private readonly IMapper _mapper;

        public ClientsController(IClientsRepository repo, ILogger<ClientsController> log, IMapper mapper)
        {
            repository = repo;
            logger = log;
            _mapper = mapper;
        }

        #region HttpGet Commands
        [HttpGet]
        public async Task<IEnumerable<ClientDto>> GetClientsAsync([FromQuery] PaginationParameters clientParameters, string name = null)
        {
            var clients = await repository.GetClientsAsync(clientParameters, name);

            var metadata = new
            {
                clients.TotalCount,
                clients.PageSize,
                clients.CurrentPage,
                clients.TotalPages,
                clients.HasNext,
                clients.HasPrevious
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(metadata));
            logger.LogInformation($"{DateTime.UtcNow:hh:mm:ss}: Retrieved Page : ({clientParameters.PageNumber}) and Page Size ({clientParameters.PageSize}) Clients ");

            return _mapper.Map<IEnumerable<ClientDto>>(clients);
            //clients.Select(x => x.ClientAsDto());
        }

        [HttpGet("Names")]
        public async Task<IEnumerable<ScuffedClientDto>> GetClientsNamesAsync([FromQuery] PaginationParameters clientParameters, string name = null)
        {
            var clients = await repository.GetClientsAsync(clientParameters, name);

            var metadata = new
            {
                clients.TotalCount,
                clients.PageSize,
                clients.CurrentPage,
                clients.TotalPages,
                clients.HasNext,
                clients.HasPrevious
            };

            var data = clients.Select(x => new ScuffedClient { Id = x.Id, Firstname = x.Firstname, Lastname = x.Lastname, Suffix = x.Suffix }).ToList();
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(metadata));
            logger.LogInformation($"{DateTime.UtcNow:hh:mm:ss}: Retrieved Page : ({clientParameters.PageNumber}) and Page Size ({clientParameters.PageSize}) Clients ");

            return _mapper.Map<IEnumerable<ScuffedClientDto>>(data);
            //data.Select(x => x.ScuffedClientAsDto());
        }

        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetClientAsync(Guid id)
        {
            var client = await repository.GetClientAsync(id);

            if (client is null)
            {
                logger.LogInformation($"{DateTime.UtcNow:hh:mm:ss}: Client Not Found ");
                return NotFound();
            }

            logger.LogInformation($"{DateTime.UtcNow:hh:mm:ss}: Client Found");
            return Ok(_mapper.Map<ClientDto>(client));
        }
        #endregion HttpGet Commands
        #region HttpPost Commands
        [HttpPost]
        public async Task<IActionResult> CreateClientAsync(CreateClientDto ClientDto)
        {
            Client client = new()
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTimeOffset.UtcNow,
                UpdatedDate = DateTimeOffset.UtcNow,
                Firstname = ClientDto.Firstname,
                Lastname = ClientDto.Lastname,
                Suffix = ClientDto.Suffix,
                Addresses = ClientDto.Addresses.ConvertAll(x => new AddressInfo
                {
                    Id = Guid.NewGuid(),
                    Address = x.Address,
                    City = x.City,
                    State = x.State,
                    Zip = x.Zip
                }),
                PhoneNumbers = ClientDto.PhoneNumbers.ConvertAll(x => new PhoneInfo { Id = Guid.NewGuid(), Phone = x.Phone }),
                Emails = ClientDto.Emails.ConvertAll(x => new EmailInfo { Id = Guid.NewGuid(), Email = x.Email }),
                Tickets = ClientDto.Tickets.ConvertAll(x => new TicketInfo
                {
                    Id = Guid.NewGuid(),
                    Airline = x.Airline,
                    Comission = x.Comission,
                    Destination = x.Destination,
                    IssuenceDate = x.IssuenceDate,
                    PNR = x.PNR,
                    TicketFare = x.TicketFare,
                    TicketNumber = x.TicketNumber,
                    TicketSoldFor = x.TicketSoldFor,
                    TicketTotal = x.TicketTotal,
                    ToBePaid = x.ToBePaid,
                    ToBePaidFor = x.ToBePaidFor,
                    Notes = x.Notes.ConvertAll(y => new NoteInfo { Id = Guid.NewGuid(), Note = y.Note })
                }),
                Notes = ClientDto.Notes.ConvertAll(x => new NoteInfo { Id = Guid.NewGuid(), Note = x.Note })
            };

            await repository.CreateClientAsync(client);
            logger.LogInformation($"{DateTime.UtcNow:hh:mm:ss}: Client Created");
            return CreatedAtAction(nameof(GetClientAsync), new { id = client.Id }, _mapper.Map<ClientDto>(client));
        }

        [HttpPost("List")]
        public async Task<IActionResult> CreateClientsAsync(List<CreateClientDto> ClientDto)
        {
            List<Task> tasks = new();

            foreach (CreateClientDto _clientDto in ClientDto)
            {
                Client client = new()
                {
                    Id = Guid.NewGuid(),
                    CreatedDate = DateTimeOffset.UtcNow,
                    UpdatedDate = DateTimeOffset.UtcNow,
                    Firstname = _clientDto.Firstname,
                    Lastname = _clientDto.Lastname,
                    Suffix = _clientDto.Suffix,
                    Addresses = _clientDto.Addresses.ConvertAll(x => new AddressInfo
                    {
                        Id = Guid.NewGuid(),
                        Address = x.Address,
                        City = x.City,
                        State = x.State,
                        Zip = x.Zip
                    }),
                    PhoneNumbers = _clientDto.PhoneNumbers.ConvertAll(x => new PhoneInfo { Id = Guid.NewGuid(), Phone = x.Phone }),
                    Emails = _clientDto.Emails.ConvertAll(x => new EmailInfo { Id = Guid.NewGuid(), Email = x.Email }),
                    Tickets = _clientDto.Tickets.ConvertAll(x => new TicketInfo
                    {
                        Id = Guid.NewGuid(),
                        Airline = x.Airline,
                        Comission = x.Comission,
                        Destination = x.Destination,
                        IssuenceDate = x.IssuenceDate,
                        PNR = x.PNR,
                        TicketFare = x.TicketFare,
                        TicketNumber = x.TicketNumber,
                        TicketSoldFor = x.TicketSoldFor,
                        TicketTotal = x.TicketTotal,
                        ToBePaid = x.ToBePaid,
                        ToBePaidFor = x.ToBePaidFor,
                        Notes = x.Notes.ConvertAll(y => new NoteInfo { Id = Guid.NewGuid(), Note = y.Note })
                    }),
                    Notes = _clientDto.Notes.ConvertAll(x => new NoteInfo { Id = Guid.NewGuid(), Note = x.Note })
                };
                tasks.Add(repository.CreateClientAsync(client));
            }

            await Task.WhenAll(tasks);
            logger.LogInformation($"{DateTime.UtcNow:hh:mm:ss}:({tasks.Count}) Client Created");
            return StatusCode(201);
        }
        #endregion HttpPost Commands
        #region HttpPut Commands
        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> UpdateClientAsync(Guid id, UpdateClientDto ClientDto)
        {
            var existingClient = await repository.GetClientAsync(id);

            if (existingClient is null)
            {
                logger.LogInformation($"{DateTime.UtcNow:hh:mm:ss}: Client ({id}) Not Found");
                return NotFound();
            }

            existingClient.Firstname = ClientDto.Firstname;
            existingClient.Lastname = ClientDto.Lastname;
            existingClient.Suffix = ClientDto.Suffix;

            await repository.UpdateClientAsync(existingClient);
            logger.LogInformation($"{DateTime.UtcNow:hh:mm:ss}: Client Updated");
            return Ok();
        }
        #endregion HttpPut Commands
        #region HttpDelete Commands
        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> DeleteClientAsync(Guid id)
        {
            var existingClient = await repository.GetClientAsync(id);

            if (existingClient is null)
            {
                logger.LogInformation($"{DateTime.UtcNow:hh:mm:ss}: Client ({id}) Not Found");
                return NotFound();
            }

            await repository.DeleteClientAsync(id);
            logger.LogInformation($"{DateTime.UtcNow:hh:mm:ss}: Client Deleted");
            return Ok();
        }
        #endregion HttpDelete Commands
    }
}
