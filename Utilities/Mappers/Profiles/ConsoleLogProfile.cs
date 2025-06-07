using AutoMapper;
using Entity.Model;
using Entity.Dtos.ConsoleLogDTO;

namespace Utilities.Mappers.Profiles
{
    public class ConsoleLogProfile : Profile
    {
        public ConsoleLogProfile()
        {
            CreateMap<ConsoleLog, ConsoleLogDto>().ReverseMap();
        }
    }
}