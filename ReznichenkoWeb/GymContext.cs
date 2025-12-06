using Microsoft.EntityFrameworkCore;
using ReznichenkoWeb.Models;

namespace ReznichenkoWeb;

public class GymContext : DbContext
{
    public GymContext(DbContextOptions<GymContext> options) : base(options) { }

    public DbSet<Member> Members { get; set; }
    public DbSet<Workout> Workouts { get; set; }
    public DbSet<Trainer> Trainers { get; set; }
}
