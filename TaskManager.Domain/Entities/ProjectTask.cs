using System;
using TaskManager.Domain.Entities.Common;
using TaskManager.Domain.Enums;

namespace TaskManager.Domain.Entities
{
    public class ProjectTask
    {
        public long Id { get; set; }
        public long? TitleSetId { get; set; }
        public LocalizationSet TitleSet { get; set; } = default!;
        public long? DescriptionSetId { get; set; }
        public LocalizationSet DescriptionSet { get; set; } = default!;
        public Status Status { get; set; } = Status.Todo;
        public DateTime? DueDate { get; set; }
        public Priority Priority { get; set; } = Priority.Medium;

        // Foreign key to Project
        public long? ProjectId { get; set; }
        public Project Project { get; set; } = default!;
    }
}
