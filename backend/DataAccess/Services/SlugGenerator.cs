using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Business.Interfaces.Services;
using BaseClinic.Domain.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.DataAccess.Services
{
    public class SlugGenerator : ISlugGenerator
    {
        /// <summary>
        /// Các slug KHÔNG được dùng — vì trùng với route của Controller.
        /// VD: /api/departments/active → route "active" không được là slug.
        /// </summary>
        private static readonly HashSet<string> ReservedSlugs = new(StringComparer.OrdinalIgnoreCase)
        {
            "active", "public", "admin",
            "new", "edit", "delete",
            "list", "detail", "create", "update", "restore",
            "deactivate", "activate",
            "me", "my", "self"
        };

        private const int MaxSlugAttempts = 1000;

        private readonly IDepartmentRepository _departmentRepo;
        private readonly IDoctorRepository _doctorRepo;

        public SlugGenerator(
            IDepartmentRepository departmentRepo,
            IDoctorRepository doctorRepo)
        {
            _departmentRepo = departmentRepo;
            _doctorRepo = doctorRepo;
        }

        public Task<string> GenerateUniqueDepartmentSlugAsync(
            string input,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default)
        {
            return GenerateUniqueSlugAsync(
                input,
                fallback: "khoa",
                existsCheck: slug => _departmentRepo.GetQueryable()
                    .IgnoreQueryFilters()
                    .AnyAsync(d => d.Slug == slug
                                && (!excludeId.HasValue || d.Id != excludeId.Value),
                              cancellationToken));
        }

        public Task<string> GenerateUniqueDoctorSlugAsync(
            string input,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default)
        {
            return GenerateUniqueSlugAsync(
                input,
                fallback: "bac-si",
                existsCheck: slug => _doctorRepo.GetQueryable()
                    .IgnoreQueryFilters()
                    .AnyAsync(d => d.Slug == slug
                                && (!excludeId.HasValue || d.Id != excludeId.Value),
                              cancellationToken));
        }

        // ==================== HELPERS ====================

        /// <summary>
        /// Sinh slug unique:
        /// 1. Generate từ input qua SlugHelper.
        /// 2. Nếu rỗng → dùng fallback.
        /// 3. Nếu là reserved → append -1.
        /// 4. Nếu trùng DB → append -2, -3, ...
        /// </summary>
        private static async Task<string> GenerateUniqueSlugAsync(
            string input,
            string fallback,
            Func<string, Task<bool>> existsCheck)
        {
            var baseSlug = SlugHelper.Generate(input);

            if (string.IsNullOrEmpty(baseSlug))
                baseSlug = fallback;

            // Check reserved
            if (ReservedSlugs.Contains(baseSlug))
                baseSlug = $"{baseSlug}-1";

            // Check unique
            var candidate = baseSlug;
            int suffix = 2;

            while (await existsCheck(candidate))
            {
                candidate = $"{baseSlug}-{suffix}";
                suffix++;

                if (suffix > MaxSlugAttempts)
                    throw new InvalidOperationException(
                        $"Không thể sinh slug unique cho '{input}' sau {MaxSlugAttempts} lần thử.");
            }

            return candidate;
        }
    }
}