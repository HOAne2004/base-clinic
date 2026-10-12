using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.DataAccess.Repositories
{
    public class EncounterRepository : Repository<Encounter>, IEncounterRepository
    {
        public EncounterRepository(ClinicDbContext context) : base(context) { }

        public async Task<string> GenerateEncounterCodeAsync(
            CancellationToken cancellationToken = default)
        {
            var today = DateTime.UtcNow.Date;
            var prefix = $"ENC-{today:yyyyMMdd}";

            var count = await _dbSet
                .Where(e => e.EncounterCode.StartsWith(prefix))
                .CountAsync(cancellationToken);

            var sequence = (count + 1).ToString("D4");
            return $"{prefix}-{sequence}";
        }
    }
}