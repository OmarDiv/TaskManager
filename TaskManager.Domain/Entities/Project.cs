using TaskManager.Domain.Entities.Common;

namespace TaskManager.Domain.Entities
{
    public class Project
    {
        public long Id { get; set; }
        public long? NameSetId { get; set; }
        public LocalizationSet NameSet { get; set; } = default!;
        public long? DescriptionSetId { get; set; }
        public LocalizationSet DescriptionSet { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public long? CreatedById { get; set; }
        public ApplicationUser CreatedBy { get; set; } = default!;
        public ICollection<ProjectTask> Tasks { get; set; } = [];
    }
}
