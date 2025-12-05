public interface IWorkoutRepository : IRepository<Workout>
{
    // Add specific methods here if needed
    Task<IEnumerable<Workout>> GetScheduledWorkoutsAsync();
}
