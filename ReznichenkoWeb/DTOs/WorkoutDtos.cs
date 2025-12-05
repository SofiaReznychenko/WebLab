public class WorkoutDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public string Instructor { get; set; } = string.Empty;
    public int MaxParticipants { get; set; }
    public DateTime ScheduledDateTime { get; set; }
    public string WorkoutType { get; set; } = string.Empty;
}

public class CreateWorkoutDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public string Instructor { get; set; } = string.Empty;
    public int MaxParticipants { get; set; }
    public DateTime ScheduledDateTime { get; set; }
    public string WorkoutType { get; set; } = string.Empty;
}

public class UpdateWorkoutDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public string Instructor { get; set; } = string.Empty;
    public int MaxParticipants { get; set; }
    public DateTime ScheduledDateTime { get; set; }
    public string WorkoutType { get; set; } = string.Empty;
}
