using AutoMapper;
using RMS.Application.Queries.RequestBC;
using RMS.Core;
using RMS.Core.Domain.RequestBC;

namespace RMS.Application.Queries
{
    public class MapDomainToQueryResult : Profile, IMapperProfile
    {
        public int Order => 1;

        public MapDomainToQueryResult()
        {
            CreateMap<Request, GetRequestResult>()
                .ForMember(d => d.StatusName, m => m.MapFrom(s => s.Status.Name))
                .ForMember(d => d.StatusDescription, m => m.MapFrom(s => s.Status.Description));

            CreateMap<RequestStatus, GetRequestStatusResult>();
        }
    }
}
