public static class Permissions
{
    public static string Type { get; } = "permissions";

    // ======================= Specializations =======================
    public const string GetSpecializations = "specializations:read";
    public const string AddSpecialization = "specializations:add";
    public const string UpdateSpecialization = "specializations:update";
    public const string DeleteSpecialization = "specializations:delete";
    public const string ToggleSpecializationActive = "specializations:toggle";

    // ======================= Patients =======================
    public const string GetPatients = "patients:read";
    public const string GetPatientDetails = "patients:details";
    public const string AddPatient = "patients:add";
    public const string UpdatePatient = "patients:update";
    public const string DeletePatient = "patients:delete";

    // ======================= Doctors =======================
    public const string GetDoctors = "doctors:read";
    public const string GetDoctorDetails = "doctors:details";
    public const string AddDoctor = "doctors:add";
    public const string UpdateDoctor = "doctors:update";
    public const string DeleteDoctor = "doctors:delete";
    public const string VerifyDoctor = "doctors:verify";

    // ======================= Clinics =======================
    public const string GetClinics = "clinics:read";
    public const string AddClinic = "clinics:add";
    public const string UpdateClinic = "clinics:update";
    public const string DeleteClinic = "clinics:delete";
    public const string ToggleClinicActive = "clinics:toggle";

    // ======================= Appointments =======================
    public const string GetAppointments = "appointments:read";
    public const string AddAppointment = "appointments:add";
    public const string UpdateAppointment = "appointments:update";
    public const string CancelAppointment = "appointments:cancel";
    public const string ConfirmAppointment = "appointments:confirm";

    // ======================= Users =======================
    public const string GetUsers = "users:read";
    public const string AddUser = "users:add";
    public const string UpdateUser = "users:update";
    public const string DeleteUser = "users:delete";
    public const string ToggleUserActive = "users:toggle";

    // ======================= Roles =======================
    public const string GetRoles = "roles:read";
    public const string AddRole = "roles:add";
    public const string UpdateRole = "roles:update";
    public const string DeleteRole = "roles:delete";
    public const string ManageRolePermissions = "roles:permissions";

    // ======================= Reports & Analytics =======================
    public const string ViewReports = "reports:read";
    public const string ExportReports = "reports:export";
    public const string ViewAnalytics = "analytics:read";

    // ======================= System Settings =======================
    public const string ManageSettings = "settings:manage";
    public const string ViewLogs = "logs:read";


    public static IList<string?> GetAllPermissions() =>
        typeof(Permissions)
            .GetFields()
            .Where(f => f.IsLiteral && !f.IsInitOnly)
            .Select(x => x.GetValue(x) as string)
            .ToList();

    ///// <summary>
    ///// Get permissions grouped by module
    ///// </summary>
    //public static Dictionary<string, List<string?>> GetGroupedPermissions()
    //{
    //    var permissions = typeof(Permissions)
    //        .GetFields()
    //        .Where(f => f.IsLiteral && !f.IsInitOnly)
    //        .Select(x => x.GetValue(x) as string)
    //        .Where(x => x != null)
    //        .ToList();

    //    return permissions
    //        .GroupBy(p => p!.Split(':')[0])
    //        .ToDictionary(
    //            g => g.Key,
    //            g => g.ToList()!
    //        );
    //}
}