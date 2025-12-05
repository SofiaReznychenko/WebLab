using FluentValidation;

public class CreateMemberDtoValidator : AbstractValidator<CreateMemberDto>
{
    public CreateMemberDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Ім'я є обов'язковим");
        RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Введіть коректний Email");
        RuleFor(x => x.Phone).NotEmpty().Matches(@"^\+380\d{9}$").WithMessage("Телефон повинен бути у форматі +380XXXXXXXXX");
        RuleFor(x => x.MembershipType).NotEmpty();
    }
}

public class UpdateMemberDtoValidator : AbstractValidator<UpdateMemberDto>
{
    public UpdateMemberDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Phone).NotEmpty();
        RuleFor(x => x.MembershipType).NotEmpty();
    }
}
