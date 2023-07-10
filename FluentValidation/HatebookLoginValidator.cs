using FluentValidation;

namespace Hatebook.Models.Validators
{
    public class HatebookLoginValidator : AbstractValidator<HatebookLogin>
    {
        public HatebookLoginValidator()
        {
            RuleFor(model => model.Email)
                .NotEmpty().NotNull().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(model => model.Password)
                .NotEmpty().NotNull().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("The password must be at least 6 characters. хахаhaha");
            // Add more validation rules as needed
        }
    }
}
