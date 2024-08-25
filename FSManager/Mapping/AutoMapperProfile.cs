using AutoMapper;

namespace FSManager.Mapping;

public class AutoMapperProfile : Profile {
    public AutoMapperProfile() {
        CreateMap<CardModel, GetCard>()
            .ForMember(
                c => c.Collection, 
                o => o.MapFrom(c => c.Collection.Key)
            )
        ;
    }
}