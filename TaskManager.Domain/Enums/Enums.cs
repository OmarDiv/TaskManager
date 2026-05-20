using System.ComponentModel.DataAnnotations;

namespace TaskManager.Domain.Enums
{
    public enum Status
    {
        [Display(Name = "To Do")]
        Todo = 1,

        [Display(Name = "In Progress")]
        InProgress = 2,

        [Display(Name = "Completed")]
        Completed = 3
    }

    public enum Priority
    {
        [Display(Name = "Low")]
        Low = 1,

        [Display(Name = "Medium")]
        Medium = 2,

        [Display(Name = "High")]
        High = 3
    }
    public enum Gender
    {
        [Display(Name = nameof(Male))]
        Male = 1,

        [Display(Name = nameof(Female))]
        Female
    }
    public enum UserType
    {
        [Display(Name = nameof(Admin))]
        Admin = 1,
        [Display(Name = nameof(Doctor))]
        Doctor = 2,
        [Display(Name = nameof(Patient))]
        Patient = 3
    }
}
