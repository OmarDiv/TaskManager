using System;
using TaskManager.Domain.Enums;

namespace TaskManager.Domain.Entities
{
    public class ProjectTask
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Status Status { get; set; } = Status.Todo;
        public DateTime? DueDate { get; set; }
        public Priority Priority { get; set; } = Priority.Medium;

        // Foreign key to Project
        public long ProjectId { get; set; }
        public Project Project { get; set; } = default!;
    }
}
