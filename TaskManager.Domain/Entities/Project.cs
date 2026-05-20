using System;
using System.Collections.Generic;

namespace TaskManager.Domain.Entities
{
    public class Project
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public long CreatedById { get; set; }
        public ApplicationUser CreatedBy { get; set; } = default!;
        public ICollection<ProjectTask> Tasks { get; set; } = [];
    }
}
