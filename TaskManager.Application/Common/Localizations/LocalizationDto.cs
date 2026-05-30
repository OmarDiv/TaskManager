#nullable disable warnings

namespace TaskManager.Application.Common.Localizations
{
    public class LocalizationDto
    {
        public long? Id { get; set; }  // e.g., "ar" for Arabic, "en" for English
        public string? Key { get; set; }  // e.g., "ar" for Arabic, "en" for English
        public string? Value { get; set; }
        public long? LocalizationSetsId { get; set; }
    }

}
#nullable restore warnings
