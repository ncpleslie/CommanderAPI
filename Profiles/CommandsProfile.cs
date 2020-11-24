using AutoMapper;
using Commander.Dtos;
using Commander.Models;

namespace Commander.Profiles
{
    public class CommandsProfile : Profile
    {
        public CommandsProfile()
        {
            // API -> DTO
            CreateMap<Command, CommandReadDto>();
            CreateMap<Command, CommandUpdateDto>();

            // DTO -> API
            CreateMap<CommandCreateDto, Command>();
            CreateMap<CommandUpdateDto, Command>();
        }
    }
}