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
                c => c.RelatedCards,
                o => o.MapFrom(c => 
                    Enumerable.Concat(
                        c.RelatedTo.Select(rel => rel.RelatedTo),
                        c.Relations.Select(rel => rel.RelatedCard)
                    )
                )
            )
        ;
    }
}