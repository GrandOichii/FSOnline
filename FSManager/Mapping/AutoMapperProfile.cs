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

        CreateMap<CardCollection, GetCollection>()
            .ForMember(
                c => c.CardCount,
                o => o.MapFrom(
                    c => c.Cards.Count
                )
            )
            .ForMember(
                c => c.Name,
                o => o.MapFrom(
                    c => c.Key
                )
            )
        ;
    }
}