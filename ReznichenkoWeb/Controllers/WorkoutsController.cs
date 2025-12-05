using Microsoft.AspNetCore.Mvc;
using FluentValidation;

[ApiController]
[Route("api/[controller]")]
public class WorkoutsController : ControllerBase
{
    private readonly IWorkoutRepository _workoutRepository;
    private readonly IValidator<CreateWorkoutDto> _createWorkoutValidator;
    private readonly IValidator<UpdateWorkoutDto> _updateWorkoutValidator;

    public WorkoutsController(
        IWorkoutRepository workoutRepository,
        IValidator<CreateWorkoutDto> createWorkoutValidator,
        IValidator<UpdateWorkoutDto> updateWorkoutValidator)
    {
        _workoutRepository = workoutRepository;
        _createWorkoutValidator = createWorkoutValidator;
        _updateWorkoutValidator = updateWorkoutValidator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WorkoutDto>>> GetAllWorkouts()
    {
        var workouts = await _workoutRepository.GetAllAsync();
        var workoutDtos = workouts.Select(w => new WorkoutDto
        {
            Id = w.Id,
            Name = w.Name,
            Description = w.Description,
            DurationMinutes = w.DurationMinutes,
            Instructor = w.Instructor,
            MaxParticipants = w.MaxParticipants,
            ScheduledDateTime = w.ScheduledDateTime,
            WorkoutType = w.WorkoutType
        });
        return Ok(workoutDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WorkoutDto>> GetWorkoutById(int id)
    {
        var workout = await _workoutRepository.GetByIdAsync(id);
        if (workout == null)
            return NotFound($"Тренування з ID {id} не знайдено");

        var workoutDto = new WorkoutDto
        {
            Id = workout.Id,
            Name = workout.Name,
            Description = workout.Description,
            DurationMinutes = workout.DurationMinutes,
            Instructor = workout.Instructor,
            MaxParticipants = workout.MaxParticipants,
            ScheduledDateTime = workout.ScheduledDateTime,
            WorkoutType = workout.WorkoutType
        };
        return Ok(workoutDto);
    }

    [HttpPost]
    public async Task<ActionResult<WorkoutDto>> CreateWorkout([FromBody] CreateWorkoutDto createWorkoutDto)
    {
        var validationResult = await _createWorkoutValidator.ValidateAsync(createWorkoutDto);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var workout = new Workout
        {
            Name = createWorkoutDto.Name,
            Description = createWorkoutDto.Description,
            DurationMinutes = createWorkoutDto.DurationMinutes,
            Instructor = createWorkoutDto.Instructor,
            MaxParticipants = createWorkoutDto.MaxParticipants,
            ScheduledDateTime = createWorkoutDto.ScheduledDateTime,
            WorkoutType = createWorkoutDto.WorkoutType
        };

        await _workoutRepository.AddAsync(workout);

        var workoutDto = new WorkoutDto
        {
            Id = workout.Id,
            Name = workout.Name,
            Description = workout.Description,
            DurationMinutes = workout.DurationMinutes,
            Instructor = workout.Instructor,
            MaxParticipants = workout.MaxParticipants,
            ScheduledDateTime = workout.ScheduledDateTime,
            WorkoutType = workout.WorkoutType
        };

        return CreatedAtAction(nameof(GetWorkoutById), new { id = workoutDto.Id }, workoutDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<WorkoutDto>> UpdateWorkout(int id, [FromBody] UpdateWorkoutDto updateWorkoutDto)
    {
        var validationResult = await _updateWorkoutValidator.ValidateAsync(updateWorkoutDto);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var workout = await _workoutRepository.GetByIdAsync(id);
        if (workout == null)
            return NotFound($"Тренування з ID {id} не знайдено");

        workout.Name = updateWorkoutDto.Name;
        workout.Description = updateWorkoutDto.Description;
        workout.DurationMinutes = updateWorkoutDto.DurationMinutes;
        workout.Instructor = updateWorkoutDto.Instructor;
        workout.MaxParticipants = updateWorkoutDto.MaxParticipants;
        workout.ScheduledDateTime = updateWorkoutDto.ScheduledDateTime;
        workout.WorkoutType = updateWorkoutDto.WorkoutType;

        await _workoutRepository.UpdateAsync(workout);

        var workoutDto = new WorkoutDto
        {
            Id = workout.Id,
            Name = workout.Name,
            Description = workout.Description,
            DurationMinutes = workout.DurationMinutes,
            Instructor = workout.Instructor,
            MaxParticipants = workout.MaxParticipants,
            ScheduledDateTime = workout.ScheduledDateTime,
            WorkoutType = workout.WorkoutType
        };

        return Ok(workoutDto);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteWorkout(int id)
    {
        var workout = await _workoutRepository.GetByIdAsync(id);
        if (workout == null)
            return NotFound($"Тренування з ID {id} не знайдено");

        await _workoutRepository.DeleteAsync(workout);
        return NoContent();
    }
}
