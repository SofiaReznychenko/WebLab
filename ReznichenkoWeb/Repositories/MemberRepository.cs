using Microsoft.EntityFrameworkCore;

public class MemberRepository : Repository<Member>, IMemberRepository
{
    public MemberRepository(GymContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Member>> GetActiveMembersAsync()
    {
        return await _dbSet.Where(m => m.IsActive).ToListAsync();
    }
}
