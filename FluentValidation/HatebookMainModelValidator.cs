using FluentValidation;
using System;

namespace Hatebook.Models.Validators
{
    public class HatebookMainModelValidator : AbstractValidator<HatebookMainModel>
    {
        public HatebookMainModelValidator()
        {
            RuleFor(model => model.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(model => model.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("The password must be at least 6 characters.");

            RuleFor(model => model.FirstName)
                .NotEmpty().WithMessage("First name is required.");

            RuleFor(model => model.Birthday)
                .NotEmpty().WithMessage("Birthday is required.");

            RuleFor(model => model.Roles)
                .Must(BeValidRole).WithMessage("Invalid role specified. Allowed roles are 'user' and 'administrator'.");


            // Add more validation rules as needed
        }
        private bool BeValidRole(ICollection<Role> roles)
        {
            foreach (Role? role in roles)
            {
                if (role.Name == null || role.Name.ToLower() != "user" || role.Name.ToLower() != "administrator")
                {
                    return false;
                }
            }
            return true;
        }
    }
}
