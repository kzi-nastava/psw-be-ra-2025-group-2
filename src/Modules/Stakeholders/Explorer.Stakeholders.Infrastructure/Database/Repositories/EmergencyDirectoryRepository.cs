using Explorer.Stakeholders.Core.Domain.Emergency;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;


public class EmergencyDirectoryRepository : IEmergencyDirectoryRepository
{
    private readonly StakeholdersContext _context;
    public EmergencyDirectoryRepository(StakeholdersContext context) { _context = context; }

    public EmergencyDirectory? GetByCountry(CountryCode code)
    {
       return _context.EmergencyDirectories
        .Include(d => d.Places)
        .Include(d => d.Embassies)
        .Include(d => d.Phrases)
        .SingleOrDefault(d => d.Country.Value == code.Value);

    }
}
