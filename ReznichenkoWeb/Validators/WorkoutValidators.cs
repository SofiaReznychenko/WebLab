using FluentValidation;
using ReznichenkoWeb.DTOs;

namespace ReznichenkoWeb.Validators;

public class CreateWorkoutDtoValidator : AbstractValidator<CreateWorkoutDto>
{
    public CreateWorkoutDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Назва тренування є обов'язковою");
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.DurationMinutes).InclusiveBetween(1, 300).WithMessage("Тривалість має бути від 1 до 300 хвилин");
        RuleFor(x => x.Instructor).NotEmpty();
        RuleFor(x => x.MaxParticipants).InclusiveBetween(1, 50);
        RuleFor(x => x.ScheduledDateTime).GreaterThan(DateTime.UtcNow).WithMessage("Дата тренування повинна бути в майбутньому");
        RuleFor(x => x.WorkoutType).NotEmpty();
    }
}

public class UpdateWorkoutDtoValidator : AbstractValidator<UpdateWorkoutDto>
{
    public UpdateWorkoutDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.DurationMinutes).InclusiveBetween(1, 300);
        RuleFor(x => x.Instructor).NotEmpty();
        RuleFor(x => x.MaxParticipants).InclusiveBetween(1, 50);
        RuleFor(x => x.WorkoutType).NotEmpty();
    }
}
