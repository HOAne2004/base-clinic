using BaseClinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace BaseClinic.DataAccess
{
    public class ClinicDbContext : DbContext
    {
        public ClinicDbContext(DbContextOptions<ClinicDbContext> options) : base(options)
        {
        }
        public DbSet<Account> Accounts => Set<Account>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Permission> Permissions => Set<Permission>();
        public DbSet<AccountRole> AccountRoles => Set<AccountRole>();
        public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
        public DbSet<Patient> Patients => Set<Patient>();
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<Doctor> Doctors => Set<Doctor>();
        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<DoctorSchedule> DoctorSchedules => Set<DoctorSchedule>();
        public DbSet<Encounter> Encounters => Set<Encounter>();
        public DbSet<Queue> Queues => Set<Queue>();
        public DbSet<QueueEntry> QueueEntries => Set<QueueEntry>();
        public DbSet<Consultation> Consultations => Set<Consultation>();
        public DbSet<Prescription> Prescriptions => Set<Prescription>();
        public DbSet<PrescriptionItem> PrescriptionItems => Set<PrescriptionItem>();
        public DbSet<Medicine> Medicines => Set<Medicine>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.ApplyConfigurationsFromAssembly(
             typeof(ClinicDbContext).Assembly);
        }
    }
}
