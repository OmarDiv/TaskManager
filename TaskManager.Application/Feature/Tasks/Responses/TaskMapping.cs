using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TaskManager.Application.Common.Localizations;
using TaskManager.Application.Feature.Tasks.Commands.CreateTask;
using TaskManager.Application.Feature.Tasks.Commands.UpdateTask;
using TaskManager.Application.Feature.Tasks.Responses;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Entities.Common;

namespace TaskManager.Application.Feature.Tasks.Responses
{
    public class TaskMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<ProjectTask, TaskResponse>()
                .Map(dest => dest.Title,
                     src => src.TitleSet != null && src.TitleSet.Localization != null
                         ? (src.TitleSet.Localization.FirstOrDefault(l => l.Key == CultureInfo.CurrentCulture.Name).Value ?? string.Empty)
                         : string.Empty)
                .Map(dest => dest.Titles, src => src.TitleSet != null ? src.TitleSet.Localization : null)
                .Map(dest => dest.Description,
                     src => src.DescriptionSet != null && src.DescriptionSet.Localization != null
                         ? (src.DescriptionSet.Localization.FirstOrDefault(l => l.Key == CultureInfo.CurrentCulture.Name).Value ?? string.Empty)
                         : string.Empty)
                .Map(dest => dest.Descriptions, src => src.DescriptionSet != null ? src.DescriptionSet.Localization : null);

            config.NewConfig<CreateTask, ProjectTask>()
                .Map(dest => dest.TitleSet, src => src.Title)
                .Map(dest => dest.DescriptionSet, src => src.Description);

            config.NewConfig<UpdateTask, ProjectTask>()
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.TitleSet)
                .Ignore(dest => dest.DescriptionSet)
                .AfterMapping((src, dest) =>
                {
                    dest.TitleSet ??= new LocalizationSet();
                    dest.TitleSet.UpdateFromDto(src.Title);
  
                    dest.DescriptionSet ??= new LocalizationSet();
                    dest.DescriptionSet.UpdateFromDto(src.Description);
                });
        }
    }
}
