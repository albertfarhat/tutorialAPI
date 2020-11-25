using AutoMapper;
using CommandAPI.Dtos;
using CommandAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommandAPI
{
    public class CommandsProfile:Profile
    {
        public CommandsProfile()
        {
            CreateMap<Command, CommandReadDto>();
        }
    }
}
