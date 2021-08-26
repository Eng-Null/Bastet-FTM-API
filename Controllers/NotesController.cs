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
    public class NotesController : ControllerBase
    {
        private readonly INotesRepository repository;
        private readonly ILogger<NotesController> logger;
        private readonly IMapper _mapper;

        public NotesController(INotesRepository repo, ILogger<NotesController> log, IMapper mapper)
        {
            repository = repo;
            logger = log;
            _mapper = mapper;
        }

        [HttpGet("{cId:Guid}/{nId:Guid}")]
        public async Task<IActionResult> GetNoteAsync(Guid cId, Guid nId)
        {
            var note = await repository.GetNoteAsync(cId, nId);

            if (note is null)
            {
                logger.LogInformation($"{DateTime.UtcNow:hh:mm:ss}: ({nId}) Note NotFound");
                return NotFound();
            }

            logger.LogInformation($"{DateTime.UtcNow:hh:mm:ss}: ({nId}) Note Returned");
            return Ok(note);
        }

        [HttpPost("{cId:Guid}")]
        public async Task<IActionResult> CreateNoteAsync(Guid cId, CreateNoteInfoDto NoteDto)
        {
            NoteInfo note = new()
            {
                Id = Guid.NewGuid(),
                Note = NoteDto.Note
            };

            await repository.CreateNoteAsync(cId, note);
            logger.LogInformation($"{DateTime.UtcNow:hh:mm:ss}: Client Note Created");
            return CreatedAtAction(nameof(GetNoteAsync), new { cId, nId = note.Id }, _mapper.Map<NoteInfoDto>(note));
        }

        [HttpDelete("{cId:Guid}/{nId:Guid}")]
        public async Task<IActionResult> DeleteClientNoteAsync(Guid cId, Guid nId)
        {
            var existingNote = await repository.GetNoteAsync(cId, nId);

            if (existingNote is null)
            {
                logger.LogInformation($"{DateTime.UtcNow:hh:mm:ss}: Client {cId} Not Found");
                return NotFound();
            }
            await repository.DeleteNoteAsync(cId, nId);

            var data = new List<NoteInfo>
            {
                new NoteInfo
                {
                    Id = existingNote.Id,
                    Note = existingNote.Note
                }
            };
            var table = data.ToStringTable(u => u.Id,
                                                u => u.Note);
            logger.LogInformation($"{DateTime.UtcNow:hh:mm:ss}: Client Note Deleted\n{table}");
            return Ok();
        }

    }
}
