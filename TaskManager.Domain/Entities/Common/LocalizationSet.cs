namespace TaskManager.Domain.Entities.Common
{
    public class LocalizationSet
    {
        public long Id { get; set; }
        public ICollection<Localization> Localization { get; set; } = [];
    }
}