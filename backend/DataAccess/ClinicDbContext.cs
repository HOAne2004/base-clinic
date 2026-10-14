using BaseClinic.Business.Interfaces.Services;
using BaseClinic.Domain.Common;
using BaseClinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Linq.Expressions;
using System.Reflection;


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
        public DbSet<AccountRole> AccountRoles => Set<AccountRole>();
        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<Consultation> Consultations => Set<Consultation>();
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<Doctor> Doctors => Set<Doctor>();
        public DbSet<DoctorSchedule> DoctorSchedules => Set<DoctorSchedule>();
        public DbSet<DoctorScheduleItem> DoctorScheduleItems => Set<DoctorScheduleItem>();
        public DbSet<Encounter> Encounters => Set<Encounter>();
        public DbSet<LoginHistory> LoginHistory => Set<LoginHistory>();
        public DbSet<Medicine> Medicines => Set<Medicine>();
        public DbSet<MergeRollbackRequest> MergeRollbackRequests => Set<MergeRollbackRequest>();
        public DbSet<OtpCode> OtpCodes => Set<OtpCode>();
        public DbSet<Patient> Patients => Set<Patient>();
        public DbSet<PatientDelegation> PatientDelegations => Set<PatientDelegation>();
        public DbSet<PatientMergeLog> PatientMergeLogs => Set<PatientMergeLog>();
        public DbSet<Permission> Permissions => Set<Permission>();
        public DbSet<Prescription> Prescriptions => Set<Prescription>();
        public DbSet<PrescriptionItem> PrescriptionItems => Set<PrescriptionItem>();
        public DbSet<Queue> Queues => Set<Queue>();
        public DbSet<QueueEntry> QueueEntries => Set<QueueEntry>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<RolePermission> RolePermissions => Set<RolePermission>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Áp dụng các cấu hình entity riêng (EntityTypeConfiguration)
            modelBuilder.ApplyConfigurationsFromAssembly(
             typeof(ClinicDbContext).Assembly);

            // Ap dung Global query filter cho soft delete
            ApplySoftDeleteQueryFiler(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // 1, Lấy thông tin user hiện tại từ JWT
            var currentUserId = _currentUserService.AccountId;
            var currentUserName = _currentUserService.UserName;
            var currentTime = DateTimeOffset.UtcNow;

            // 2, Quét các entity kế thừa từ AuditableEntity đang được thêm hoặc sửa
            // LƯU Ý: Vòng lặp này chỉ xử lý entity kế thừa AuditableEntity.
            // Với thiết kế hiện tại (FullAuditedEntity : AuditableEntity),
            // mọi entity soft-delete đều được xử lý.
            // Nếu tương lai có entity chỉ implement ISoftDeleteEntity (không audit),
            // cần bổ sung xử lý riêng cho interface đó.
            foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
            {
                switch (entry.State)
                {
                    // Entry State = Added → gán Created*
                    case EntityState.Added:
                        // Dùng .Property().CurrentValue vì các thuộc tính này có protected setter
                        entry.Property(x => x.CreatedDate).CurrentValue = currentTime;
                        entry.Property(x => x.CreatedId).CurrentValue = currentUserId;
                        entry.Property(x => x.CreatedBy).CurrentValue = currentUserName;
                        break;

                    // Entry State = Modified → gán UpdatedAt/UpdatedBy, khóa CreatedAt
                    case EntityState.Modified:
                        entry.Property(x => x.UpdatedDate).CurrentValue = currentTime;
                        entry.Property(x => x.UpdatedId).CurrentValue = currentUserId;
                        entry.Property(x => x.UpdatedBy).CurrentValue = currentUserName;

                        // Đảm bảo không vô tình ghi đè Created* khi update
                        entry.Property(x => x.CreatedDate).IsModified = false;
                        entry.Property(x => x.CreatedBy).IsModified = false;
                        entry.Property(x => x.CreatedId).IsModified = false;
                        break;

                    // Entry State = Deleted → chuyển thành soft delete
                    case EntityState.Deleted:
                        // Nếu entity implement ISoftDeleteEntity → chuyển thành soft delete
                        if (entry.Entity is ISoftDeleteEntity softDeleteEntity)
                        {
                            // Đổi state từ Deleted → Modified để EF Core sinh UPDATE thay vì DELETE
                            entry.State = EntityState.Modified;
                            entry.Property(nameof(ISoftDeleteEntity.IsDeleted)).CurrentValue = true;
                            entry.Property(nameof(ISoftDeleteEntity.DeletedDate)).CurrentValue = currentTime;
                            entry.Property(nameof(ISoftDeleteEntity.DeletedId)).CurrentValue = currentUserId;

                            // Không ghi đè Created* khi soft delete
                            entry.Property(x => x.CreatedDate).IsModified = false;
                            entry.Property(x => x.CreatedId).IsModified = false;
                            entry.Property(x => x.CreatedBy).IsModified = false;
                        }
                        // Nếu không → giữ nguyên Deleted (xóa cứng)
                        break;
                }
            }

            // Gọi khi đảm bảo mọi thay đổi audit được áp dụng trước khi commit
            return await base.SaveChangesAsync(cancellationToken);
        }

        private static void ApplySoftDeleteQueryFiler(ModelBuilder modelBuilder)
        {
            // Duyet qua toan bo entity type da duoc dang ky voi EF Core
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // Chi ap dung cho entity co implement ISoftDeleteEntity
                if (!typeof(ISoftDeleteEntity).IsAssignableFrom(entityType.ClrType))
                    continue;

                // Bo qua ne entity da co filter rieng (tranh ghi de)
                if (entityType.GetQueryFilter() != null)
                    continue;


                // B1: Tao parameter "e" voi type = entityType.ClrType
                var parameter = Expression.Parameter(entityType.ClrType, "e");

                // B2: Truy cap property "e.IsDeleted"
                var isDeletedProperty = Expression.Property(
                    parameter,
                    nameof(ISoftDeleteEntity.IsDeleted));

                // B3: So sanh "e.IsDelelted == false"
                var compareExpression = Expression.Equal(
                    isDeletedProperty,
                    Expression.Constant(false));

                // B4: Dong goi thanh lambda "e => e.IsDeleted == false"
                var lamda = Expression.Lambda(compareExpression, parameter);

                // B5: Ap dung filter cho entity type
                modelBuilder.Entity(entityType.ClrType)
                    .HasQueryFilter(lamda); // gắn một điều kiện WHERE cố định vao entity (Where IsDelete = 0 )
            }
        }
    }
}
