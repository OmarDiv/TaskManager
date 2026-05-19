using System.ComponentModel.DataAnnotations;

namespace TaskManager.Domain.Enums
{
    public enum Gender
    {
        [Display(Name = nameof(Male))]
        Male = 1,

        [Display(Name = nameof(Female))]
        Female
    }
    public enum BloodType
    {
        [Display(Name = nameof(A_Positive))]
        A_Positive,
        [Display(Name = nameof(A_Negative))]
        A_Negative,
        [Display(Name = nameof(B_Positive))]
        B_Positive,
        [Display(Name = nameof(B_Negative))]
        B_Negative,
        [Display(Name = nameof(AB_Positive))]
        AB_Positive,
        [Display(Name = nameof(AB_Negative))]
        AB_Negative,
        [Display(Name = nameof(O_Positive))]
        O_Positive,
        [Display(Name = nameof(O_Negative))]
        O_Negative
    }

    public enum DoctorTitle
    {
        [Display(Name = nameof(Resident))]
        Resident,       // طبيب مقيم
        [Display(Name = nameof(Specialist))]
        Specialist,     // أخصائي
        [Display(Name = nameof(Consultant))]
        Consultant,     // استشاري
        [Display(Name = nameof(Professor))]
        Professor       // أستاذ دكتور
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
