using Microsoft.EntityFrameworkCore;

public class WorkoutRepository : Repository<Workout>, IWorkoutRepository
{
    public WorkoutRepository(GymContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Workout>> GetScheduledWorkoutsAsync()
    {
        return await _dbSet.Where(w => w.ScheduledDateTime > DateTime.UtcNow).ToListAsync();
    }
}
