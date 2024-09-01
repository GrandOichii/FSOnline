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

        CreateMap<CardModel, GetCardWithRelations>()
            .ForMember(
                c => c.Collection, 
                o => o.MapFrom(c => c.Collection.Key)
            )
            .ForMember(
                c => c.Relations,
                o => o.MapFrom(
                    c => Enumerable.Concat(
                        c.Relations,
                        c.RelatedTo
                    )
                )
            )
        ;

        CreateMap<CardRelation, GetCardRelation>()
        ;
    }
}