using System.Collections.Generic;
using AutoMapper;
using Commander.Data;
using Commander.Dtos;
using Commander.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Commander.Controllers
{
    // api/commands
    [Route("api/commands")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommanderRepo _repo;

        public IMapper _mapper { get; }

        public CommandsController(ICommanderRepo repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        // GET api/commands
        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetAllCommands()
        {
            var commandItems = _repo.GetAllCommands();
            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commandItems));
        }

        // GET api/commands/{id}
        [HttpGet("{id}", Name = "GetCommandById")]
        public ActionResult<CommandReadDto> GetCommandById(int id)
        {
            var commandItem = _repo.GetCommandById(id);
            if (commandItem == null) return NotFound();
            return Ok(_mapper.Map<CommandReadDto>(commandItem));
        }

        // POST api/commands
        [HttpPost]
        public ActionResult<CommandReadDto> CreateCommand(CommandCreateDto commandCreateDto)
        {
            // Map command
            var commandModel = _mapper.Map<Command>(commandCreateDto);
            _repo.CreateCommand(commandModel);

            // Save to DB
            _repo.SaveChanges();

            // Map to CommandReadDto
            var commandReadDto = _mapper.Map<CommandReadDto>(commandModel);

            // Return route of saved command
            return CreatedAtRoute(nameof(GetCommandById), new { Id = commandReadDto.Id }, commandReadDto);
        }

        // PUT api/commands/{id}
        [HttpPut("{id}")]
        public ActionResult UpdateCommand(int id, CommandUpdateDto commandUpdateDto)
        {
            // Check resource exists
            var commandModelFromRepo = _repo.GetCommandById(id);
            if (commandModelFromRepo == null) return NotFound();

            // Map to the Update DTO
            _mapper.Map(commandUpdateDto, commandModelFromRepo);

            // Update command, if it has been implemented
            _repo.UpdateCommand(commandModelFromRepo);

            // Save the changes to DB
            _repo.SaveChanges();

            return NoContent();
        }

        // PATCH api/commands/{id}
        [HttpPatch("{id}")]
        public ActionResult PartialCommandUpdate(int id, JsonPatchDocument<CommandUpdateDto> patchDoc)
        {
            // Check resource exists
            var commandModelFromRepo = _repo.GetCommandById(id);
            if (commandModelFromRepo == null) return NotFound();

            // Convert
            var commandToPatch = _mapper.Map<CommandUpdateDto>(commandModelFromRepo);

            // Apply the patch
            patchDoc.ApplyTo(commandToPatch, ModelState);
            if (!TryValidateModel(commandToPatch)) return ValidationProblem(ModelState);

            // Update our data
            _mapper.Map(commandToPatch, commandModelFromRepo);
            _repo.UpdateCommand(commandModelFromRepo);
            _repo.SaveChanges();

            return NoContent();
        }

        // DELETE api/commands/{id}
        [HttpDelete("id")]
        public ActionResult DeleteCommand(int id)
        {
            // Check resource exists
            var commandModelFromRepo = _repo.GetCommandById(id);
            if (commandModelFromRepo == null) return NotFound();

            // Delete command
            _repo.DeleteCommand(commandModelFromRepo);
            _repo.SaveChanges();

            return NoContent();
        }

    }
}