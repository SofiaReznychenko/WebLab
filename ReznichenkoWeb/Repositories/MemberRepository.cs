using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class MemberRepository : Repository<Member>, IMemberRepository
{
    public MemberRepository(GymContext context, ILogger<MemberRepository> logger) : base(context, logger)
    {
    }

    public async Task<IEnumerable<Member>> GetActiveMembersAsync()
    {
        return await _dbSet.Where(m => m.IsActive).ToListAsync();
    }
}
