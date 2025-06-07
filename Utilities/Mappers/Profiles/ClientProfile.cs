using AutoMapper;
using Entity.Model;
using Entity.Dtos.ClientDTO;

namespace Utilities.Mappers.Profiles
{
    public class ClientProfile : Profile
    {
        public ClientProfile()
        {
            CreateMap<Client, ClientDto>().ReverseMap();
            CreateMap<UpdateClientDto, Client>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<DeleteLogicalClientDto, Client>();
        }
    }
}