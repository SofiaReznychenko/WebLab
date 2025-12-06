using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ReznichenkoWeb.Models;

namespace ReznichenkoWeb.Repositories;

public class WorkoutRepository : Repository<Workout>, IWorkoutRepository
{
    public WorkoutRepository(GymContext context, ILogger<WorkoutRepository> logger) : base(context, logger)
    {
    }

    public async Task<IEnumerable<Workout>> GetScheduledWorkoutsAsync()
    {
        return await _dbSet.Where(w => w.ScheduledDateTime > DateTime.UtcNow).ToListAsync();
    }
}
