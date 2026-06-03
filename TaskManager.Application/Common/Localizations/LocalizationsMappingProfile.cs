using TaskManager.Domain.Entities.Common;
namespace TaskManager.Application.Common.Localizations;


public class LocalizationsMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Localization, LocalizationDto>()
        .Map(dest => dest.Key, src => src.Key)
        .Map(dest => dest.LocalizationSetsId, src => src.LocalizationSetId);
        
        config.NewConfig<LocalizationDto, Localization>()
            .Map(dest => dest.Key, src => src.Key);

        config.NewConfig<List<LocalizationDto>, LocalizationSet>()
              .MapWith(src => src.ToLocalizationSet());
    }
}
