using BaseClinic.Business.Interfaces;
using BaseClinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.DataAccess.Repositories
{
    public class EncounterRepository : IEncounterRepository
    {
        private readonly ClinicDbContext _context;
        public EncounterRepository(ClinicDbContext context)
        {
            _context = context;
        }

        public void Add (Encounter encounter)
        {
            _context.Encounters.Add(encounter);
        }

        public async Task<string> GenerateEncounterCodeAsync(CancellationToken cancellationToken = default)
        {
            var today = DateTime.UtcNow.Date;
            var prefix = $"ENC-{today:yyyyMMdd}";

            var count = await _context.Encounters
                .Where(e => e.EncounterCode.StartsWith(prefix))
                .CountAsync(cancellationToken);

            var sequence = (count + 1).ToString("D4");
            return $"{prefix}-{sequence}";
        }
    }
}
