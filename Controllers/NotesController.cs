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
    public class NotesController : ControllerBase
    {
        private readonly INotesRepository repository;
        private readonly ILogger<NotesController> logger;

        public NotesController(INotesRepository repo, ILogger<NotesController> log)
        {
            repository = repo;
            logger = log;
        }

        /// <summary>
        /// Create a new client NoteInfo
        /// </summary>
        /// <param name="ClientDto"> new client NoteInfo data as json</param>
        /// <returns></returns>
        [HttpPost("{clientId:Guid}")]
        public async Task<IActionResult> CreateNoteAsync(Guid clientId, CreateNoteInfoDto NoteDto)
        {
            NoteInfo note = new()
            {
                Id = Guid.NewGuid(),
                Note = NoteDto.Note
            };
            await repository.CreateNoteAsync(clientId, note);
            logger.LogInformation($"{DateTime.UtcNow:hh:mm:ss}: Client Note Created");
            return Ok(note);
        }
        /// <summary>
        /// Delete Client Note
        /// </summary>
        /// <param name="clientId">Client ID</param>
        /// <param name="noteId">Note ID</param>
        /// <returns></returns>
        [HttpDelete("{clientId:Guid}/{noteId:Guid}")]
        public async Task<IActionResult> DeleteClientNoteAsync(Guid clientId, Guid noteId)
        {
            var existingNote = await repository.GetNoteAsync(clientId, noteId);

            if (existingNote is null)
            {
                return NotFound();
            }

            await repository.DeleteNoteAsync(clientId, noteId);
            logger.LogInformation($"{DateTime.UtcNow:hh:mm:ss}: Client Note Deleted");
            return Ok();
        }

    }
}
