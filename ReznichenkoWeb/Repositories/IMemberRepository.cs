public interface IMemberRepository : IRepository<Member>
{
    // Add specific methods here if needed, e.g. GetActiveMembers()
    Task<IEnumerable<Member>> GetActiveMembersAsync();
}
