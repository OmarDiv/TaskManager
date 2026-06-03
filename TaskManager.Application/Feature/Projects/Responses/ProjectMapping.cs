using System.Globalization;
using TaskManager.Application.Common.Localizations;
using TaskManager.Application.Feature.Projects.Commands.CreateProject;
using TaskManager.Application.Feature.Projects.Commands.UpdateProject;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Entities.Common;

namespace TaskManager.Application.Feature.Projects.Responses
{
    public class ProjectMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Project, ProjectResponse>()
                .Map(dest => dest.Name,
                     src => src.NameSet != null && src.NameSet.Localization != null
                         ? (src.NameSet.Localization.FirstOrDefault(l => l.Key == CultureInfo.CurrentCulture.Name).Value ?? string.Empty)
                         : string.Empty)
                .Map(dest => dest.Names, src => src.NameSet != null ? src.NameSet.Localization : null)
                .Map(dest => dest.Description,
                     src => src.DescriptionSet != null && src.DescriptionSet.Localization != null
                         ? (src.DescriptionSet.Localization.FirstOrDefault(l => l.Key == CultureInfo.CurrentCulture.Name).Value ?? string.Empty)
                         : string.Empty)
                .Map(dest => dest.Descriptions, src => src.DescriptionSet != null ? src.DescriptionSet.Localization : null);

            config.NewConfig<CreateProject, Project>()
                .Map(dest => dest.NameSet, src => src.Name)
                .Map(dest => dest.DescriptionSet, src => src.Description)
                .Map(dest => dest.CreatedById, src => src.UserId);

            config.NewConfig<UpdateProject, Project>()
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.NameSet)
                .Ignore(dest => dest.DescriptionSet)
                .AfterMapping((src, dest) =>
                {
                    dest.NameSet ??= new LocalizationSet();
                    dest.NameSet.UpdateFromDto(src.Name);
                    
                    dest.DescriptionSet ??= new LocalizationSet();
                    dest.DescriptionSet.UpdateFromDto(src.Description);
                });
        }
    }
}
