using AutoMapper;
using RMS.Application.Commands.RequestBC;
using RMS.Application.Queries.RequestBC;
using RMS.Core;

namespace RequestsManagementSystem.Mapping
{
    public class MapCommandsAndQueries : Profile, IMapperProfile
    {
        public int Order => 1;

        public MapCommandsAndQueries()
        {
            CreateMap<GetRequestResult, UpdateRequest>();
            CreateMap<AddRequest, UpdateRequest>();
            CreateMap<UpdateRequest, AddRequest>();
        }
    }
}
