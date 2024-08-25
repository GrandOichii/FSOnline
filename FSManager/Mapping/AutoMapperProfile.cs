using AutoMapper;

namespace FSManager.Mapping;

public class AutoMapperProfile : Profile {
    public AutoMapperProfile() {
        CreateMap<CardModel, GetCard>()
            .ForMember(
                c => c.Collection, 
                o => o.MapFrom(c => c.Collection.Key)
            )
            .ForMember(
                c => c.ImageUrl,
                o => o.MapFrom(
                    (src, dest, destMember, resContext) => dest.ImageUrl = src.Images.First(img => img.Collection.Key == (string)resContext.Items["ICK"]).Source
                )
            )
        ;
    }
}