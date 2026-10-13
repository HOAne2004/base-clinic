namespace BaseClinic.DataAccess.Seeding
{
    /// <summary>
    /// Chạy toàn bộ seeder theo thứ tự dependency.
    /// 
    /// THỨ TỰ QUAN TRỌNG:
    /// 1. Permission trước (vì RolePermission cần).
    /// 2. Role sau (vì RolePermission cần cả 2).
    /// 3. RolePermission cuối cùng.
    /// </summary>
    public static class DatabaseSeeder
    {
        public static async Task SeedAllAsync(
            ClinicDbContext context,
            CancellationToken cancellationToken = default)
        {
            await DepartmentSeeder.SeedAsync(context, cancellationToken);
            await PermissionSeeder.SeedAsync(context, cancellationToken);
            await RoleSeeder.SeedAsync(context, cancellationToken);
            await RolePermissionSeeder.SeedAsync(context, cancellationToken);
        }
    }
}