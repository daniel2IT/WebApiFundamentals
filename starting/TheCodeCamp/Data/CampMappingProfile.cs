using AutoMapper;
using TheCodeCamp.Models;

namespace TheCodeCamp.Data
{
    public class CampMappingProfile : Profile
    {
        public CampMappingProfile()
        {
            // Rename VenueName , to just Venue
            CreateMap<Camp, CampModel>()
                .ForMember(c => c.Venue, opt => opt.MapFrom(m => m.Location.VenueName))
                .ReverseMap();

            CreateMap<Talk, TalkModel>().ReverseMap();

            CreateMap<Speaker, SpeakerModel>().ReverseMap();
        }
    }
}
