using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CommandAPI.Data;
using CommandAPI.Dtos;
using CommandAPI.Models;
using LoggerService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CommandAPI.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        //comments
        private readonly ICommandAPIRepo _commandAPIRepo;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;

        public CommandsController(ILoggerManager logger, 
            ICommandAPIRepo commandAPIRepo,
            IMapper mapper)
        {
            _commandAPIRepo = commandAPIRepo;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetAllCommands()
        {
            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(_commandAPIRepo.GetAllCommands()));
        }

        [Authorize]
        [HttpGet("{id}", Name = "GetCommandById")]
        public ActionResult<CommandReadDto> GetCommandById(int id)
        {
            var commandItem = _commandAPIRepo.GetCommandById(id);
            if (commandItem == null)
            {
                _logger?.LogWarn("command not found");
                return NotFound();
            }
            return Ok(_mapper.Map<CommandReadDto>(commandItem));
        }

        [HttpPost]
        public ActionResult<CommandReadDto> CreateCommand(CommandCreateDto commandCreateDto)
        {
            var commandModel = _mapper.Map<Command>(commandCreateDto);
            _commandAPIRepo.CreateCommand(commandModel);
            _commandAPIRepo.SaveChanges();
            var commandReadDto = _mapper.Map<CommandReadDto>(commandModel);
            return CreatedAtRoute(
                nameof(GetCommandById),
                new { commandReadDto.Id },
                commandReadDto);

        }

        [HttpPut("{id}")]
        public ActionResult UpdateCommand(int id, CommandUpdateDto cmd)
        {
            var commandModelFromRepo = _commandAPIRepo.GetCommandById(id);
            if (commandModelFromRepo == null)
            {
                _logger?.LogWarn("command not found");
                return NotFound();
            }
            _mapper.Map(cmd, commandModelFromRepo);
            _commandAPIRepo.UpdateCommand(commandModelFromRepo);
            _commandAPIRepo.SaveChanges();
            return NoContent();

        }

        [HttpPatch("{id}")]
        public ActionResult PartialCommandUpdate(int id,
                JsonPatchDocument<CommandUpdateDto> patchDoc)
        {
            var commandModelFromRepo = _commandAPIRepo.GetCommandById(id);
            if (commandModelFromRepo == null)
            {
                _logger?.LogWarn("command not found");
                return NotFound();
            }
            var commandToPatch = _mapper.Map<CommandUpdateDto>(commandModelFromRepo);
            patchDoc.ApplyTo(commandToPatch, ModelState);
            if (!TryValidateModel(commandToPatch))
            {
                return ValidationProblem(ModelState);
            }
            _mapper.Map(commandToPatch, commandModelFromRepo);
            _commandAPIRepo.UpdateCommand(commandModelFromRepo);
            _commandAPIRepo.SaveChanges();
            return NoContent();

        }

        [HttpDelete("{id}")]
        public ActionResult DeleteCommand(int id)
        {
            var commandModelFromRepo = _commandAPIRepo.GetCommandById(id);
            if (commandModelFromRepo == null)
            {
                _logger?.LogWarn("command not found");
                return NotFound();
            }

            _commandAPIRepo.DeleteCommand(commandModelFromRepo);
            _commandAPIRepo.SaveChanges();
            return NoContent();
        }
    }
}