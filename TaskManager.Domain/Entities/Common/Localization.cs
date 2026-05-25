namespace TaskManager.Domain.Entities.Common
{
    public class Localization
    {
        public long Id { get; set; }
        public string Key { get; set; } = string.Empty;    // "ar" | "en"
        public string Value { get; set; } = string.Empty;
        public long LocalizationSetId { get; set; }
        public LocalizationSet LocalizationSet { get; set; } = default!;
    }
}
