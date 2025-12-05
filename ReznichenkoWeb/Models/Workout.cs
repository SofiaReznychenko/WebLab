using System.ComponentModel.DataAnnotations;

public class Workout
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public string Description { get; set; } = string.Empty;
    [Range(1, 300)]
    public int DurationMinutes { get; set; }
    [Required]
    public string Instructor { get; set; } = string.Empty;
    [Range(1, 50)]
    public int MaxParticipants { get; set; }
    public DateTime ScheduledDateTime { get; set; }
    public string WorkoutType { get; set; } = string.Empty;
}
