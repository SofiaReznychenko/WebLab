using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class TrainerRepository : Repository<Trainer>, ITrainerRepository
{
    public TrainerRepository(GymContext context, ILogger<TrainerRepository> logger) : base(context, logger)
    {
    }
}
