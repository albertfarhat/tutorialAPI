using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CommandAPI.Data;
using CommandAPI.Dtos;
using CommandAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CommandAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandAPIRepo _commandAPIRepo;
        private readonly IMapper _mapper;

        public CommandsController(ICommandAPIRepo commandAPIRepo,IMapper mapper)
        {
            _commandAPIRepo = commandAPIRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetAllCommands()
        {
            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(_commandAPIRepo.GetAllCommands()));
        }

        [HttpGet("{id}")]
        public ActionResult<CommandReadDto> GetCommandById(int id)
        {
            var commandItem = _commandAPIRepo.GetCommandById(id);
            if (commandItem == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<CommandReadDto>(commandItem));
        }
    }
}