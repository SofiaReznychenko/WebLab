using FluentValidation;

public class CreateTrainerDtoValidator : AbstractValidator<CreateTrainerDto>
{
    public CreateTrainerDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Age).InclusiveBetween(18, 100);
        RuleFor(x => x.Gender).NotEmpty();
        RuleFor(x => x.Experience).InclusiveBetween(0, 50);
        RuleFor(x => x.Phone).NotEmpty().Matches(@"^\+?380\d{9}$").WithMessage("Телефон має бути у форматі +380...");
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}

public class UpdateTrainerDtoValidator : AbstractValidator<UpdateTrainerDto>
{
    public UpdateTrainerDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Age).InclusiveBetween(18, 100);
        RuleFor(x => x.Gender).NotEmpty();
        RuleFor(x => x.Experience).InclusiveBetween(0, 50);
        RuleFor(x => x.Phone).NotEmpty().Matches(@"^\+?380\d{9}$").WithMessage("Телефон має бути у форматі +380...");
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}
