using FluentValidation;

namespace Hatebook.Models.Validators
{
    public class GroupsValidator : AbstractValidator<GroupsModel>
    {
        public GroupsValidator()
        {
            RuleFor(model => model.Name)
                .NotEmpty().NotNull().WithMessage("Please, enter a name.");
        }
    }
    public class GroupAdminsValidator : AbstractValidator<GroupAdmins>
    {
        public GroupAdminsValidator()
        {
            RuleFor(model => model.UserId)
                .NotEmpty().NotNull().WithMessage("Please, enter the name of the user.");

            RuleFor(model => model.GroupId)
                .NotEmpty().NotNull().WithMessage("Please, enter the name of the group.");
        }
    }
    public class UsersInGroupsValidator : AbstractValidator<UsersInGroups>
    {
        public UsersInGroupsValidator()
        {
            RuleFor(model => model.UserId)
                .NotEmpty().NotNull().WithMessage("Please, enter the name of the user.");

            RuleFor(model => model.GroupId)
                .NotEmpty().NotNull().WithMessage("Please, enter the name of the group.");
        }
    }
}