using System.Collections.Generic;
using System.Linq;
using TaskManager.Domain.Entities.Common;

namespace TaskManager.Application.Common.Localizations
{
    /// <summary>
    /// Helper class to manage synchronization between Localization DTOs and Domain Entities.
    /// </summary>
    public static class LocalizationMappingExtensions
    {
        /// <summary>
        /// Converts a list of DTOs to a new LocalizationSet.
        /// Usage: var set = dtoList.ToLocalizationSet();
        /// </summary>
        public static LocalizationSet? ToLocalizationSet(this List<LocalizationDto>? dtos)
        {
            if (dtos?.Any(n => !string.IsNullOrWhiteSpace(n.Value)) != true)
                return null;

            var set = new LocalizationSet();
            set.UpdateFromDto(dtos);
            return set;
        }

        /// <summary>
        /// Extension method to update an existing LocalizationSet from a list of DTOs.
        /// </summary>
        public static void UpdateFromDto(this LocalizationSet? set, List<LocalizationDto>? dtos)
        {
            if (dtos == null || set == null) return;
            
            set.Localization ??= new List<Localization>();
            set.Localization.UpdateList(dtos);
        }

        /// <summary>
        /// Extension method to perform the actual Sync logic on a localization collection.
        /// </summary>
        public static void UpdateList(this ICollection<Localization> list, List<LocalizationDto> dtos)
        {
            if (dtos == null) return;

            var incoming = dtos
                .Where(dto => !string.IsNullOrWhiteSpace(dto.Value))
                .Select(dto => new { Key = dto.Key ?? string.Empty, Value = dto.Value.Trim() })
                .ToList();

            var incomingKeys = incoming.Select(i => i.Key).ToHashSet();
            var itemsToRemove = list.Where(el => !incomingKeys.Contains(el.Key)).ToList();
            
            foreach (var item in itemsToRemove)
            {
                list.Remove(item);
            }

            foreach (var inc in incoming)
            {
                var existingItem = list.FirstOrDefault(el => el.Key == inc.Key);
                
                if (existingItem != null)
                {
                    if (existingItem.Value != inc.Value)
                    {
                        existingItem.Value = inc.Value;
                    }
                }
                else
                {
                    list.Add(new Localization
                    {
                        Key = inc.Key,
                        Value = inc.Value
                    });
                }
            }
        }
    }
}
