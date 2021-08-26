using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using BastetAPI.DTOs;
using BastetAPI.Entities;
using BastetFTMAPI.Parameters;
using BastetFTMAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BastetFTMAPI.Controllers
{
    [Authorize(Roles = "Manager, Owner")]
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketsRepository repository;
        private readonly ILogger<TicketsController> logger;
        private readonly IMapper _mapper;

        public TicketsController(ITicketsRepository repo, ILogger<TicketsController> log, IMapper mapper)
        {
            repository = repo;
            logger = log;
            _mapper = mapper;
        }

        [HttpGet("{clientId:Guid}")]
        public async Task<IEnumerable<TicketInfoDto>> GetTicketsAsync([FromQuery] PaginationParameters clientParameters, Guid clientId)
        {
            var tickets = await repository.GetTicketsAsync(clientParameters, clientId);

            var metadata = new
            {
                tickets.TotalCount,
                tickets.PageSize,
                tickets.CurrentPage,
                tickets.TotalPages,
                tickets.HasNext,
                tickets.HasPrevious
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(metadata));
            logger.LogInformation($"{DateTime.UtcNow:hh:mm:ss}: Retrieved Page : ({clientParameters.PageNumber}) and Page Size ({clientParameters.PageSize}) Tickets ");

            return _mapper.Map<IEnumerable<TicketInfoDto>>(tickets);
            //clients.Select(x => x.TicketsAsDto());
        }
        [HttpPost("{clientId:Guid}")]
        public async Task<IActionResult> CreateTicketAsync(Guid clientId, CreateTicketInfoDto ticketDto)
        {
            TicketInfo ticket = new()
            {
                Id = Guid.NewGuid(),
                Airline = ticketDto.Airline,
                Comission = ticketDto.Comission,
                Destination = ticketDto.Destination,
                IssuenceDate = ticketDto.IssuenceDate,
                PNR = ticketDto.PNR,
                TicketFare = ticketDto.TicketFare,
                TicketNumber = ticketDto.TicketNumber,
                TicketSoldFor = ticketDto.TicketSoldFor,
                TicketTotal = ticketDto.TicketTotal,
                ToBePaid = ticketDto.ToBePaid,
                ToBePaidFor = ticketDto.ToBePaidFor,
                Notes = ticketDto.Notes.ConvertAll(y => new NoteInfo { Id = Guid.NewGuid(), Note = y.Note })
            };

            await repository.CreateTicketAsync(clientId, ticket);
            logger.LogInformation($"{DateTime.UtcNow:hh:mm:ss}: Client Ticket Created");
            return Ok(ticket);
        }

        [HttpPost("{clientId:Guid}/{ticketId:Guid}")]
        public async Task<IActionResult> CreateNoteAsync(Guid clientId, Guid ticketId, CreateNoteInfoDto NoteDto)
        {
            NoteInfo note = new()
            {
                Id = Guid.NewGuid(),
                Note = NoteDto.Note
            };

            await repository.CreateTicketsNoteAsync(clientId, ticketId, note);
            logger.LogInformation($"{DateTime.UtcNow:hh:mm:ss}: Client Ticket's Note Created");
            return Ok(note);
        }

        [HttpDelete("{clientId:Guid}/{ticketId:Guid}")]
        public async Task<IActionResult> DeleteTicketAsync(Guid clientId, Guid ticketId)
        {
            var existingTicket = await repository.GetTicketAsync(clientId, ticketId);

            if (existingTicket is null)
            {
                return NotFound();
            }

            await repository.DeleteTicketAsync(clientId, ticketId);
            logger.LogInformation($"{DateTime.UtcNow:hh:mm:ss}: Client's Ticket Deleted");
            return Ok();
        }

        [HttpDelete("{clientId:Guid}/{ticketId:Guid}/{noteId:Guid}")]
        public async Task<IActionResult> DeleteTicketsNoteAsync(Guid clientId, Guid ticketId, Guid noteId)
        {
            var existingTicketsNote = await repository.GetTicketsNoteAsync(clientId, ticketId, noteId);

            if (existingTicketsNote is null)
            {
                return NotFound();
            }

            await repository.DeleteTicketsNoteAsync(clientId, ticketId, noteId);
            logger.LogInformation($"{DateTime.UtcNow:hh:mm:ss}: Client's Ticket's Note Deleted");
            return Ok();
        }

    }
}
