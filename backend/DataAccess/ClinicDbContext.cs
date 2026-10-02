using BaseClinic.Business.Interfaces;
using BaseClinic.Domain.Common;
using BaseClinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace BaseClinic.DataAccess
{
    public class ClinicDbContext : DbContext
    {
        private readonly ICurrentUserService _currentUserService;
        public ClinicDbContext(DbContextOptions<ClinicDbContext> options, ICurrentUserService currentUserService) : base(options)
        {
            _currentUserService = currentUserService;
        }
        public DbSet<Account> Accounts => Set<Account>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Permission> Permissions => Set<Permission>();
        public DbSet<AccountRole> AccountRoles => Set<AccountRole>();
        public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
        public DbSet<Patient> Patients => Set<Patient>();
        public DbSet<PatientDelegation> PatientDelegations => Set<PatientDelegation>();
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

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var currentUserId = _currentUserService.AccountId;
            var currentTime = DateTimeOffset.UtcNow;

            // Quét các entity kế thừa từ AuditableEntity đang được thêm hoặc sửa
            foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        // Dùng .Property().CurrentValue vì các thuộc tính này có protected setter
                        entry.Property(x => x.CreatedAt).CurrentValue = currentTime;
                        entry.Property(x => x.CreatedBy).CurrentValue = currentUserId;
                        break;

                    case EntityState.Modified:
                        entry.Property(x => x.UpdatedAt).CurrentValue = currentTime;
                        entry.Property(x => x.UpdatedBy).CurrentValue = currentUserId;

                        // Đảm bảo không vô tình ghi đè CreatedAt/CreatedBy khi update
                        entry.Property(x => x.CreatedAt).IsModified = false;
                        entry.Property(x => x.CreatedBy).IsModified = false;
                        break;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
