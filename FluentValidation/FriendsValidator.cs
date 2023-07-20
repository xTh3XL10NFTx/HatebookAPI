using FluentValidation;

namespace Hatebook.Models.Validators
{
    public class FriendsListValidator : AbstractValidator<FriendsList>
    {
        public FriendsListValidator()
        {
            RuleFor(model => model.Reciver)
                .NotEmpty().NotNull().WithMessage("You have to enter an the email of the user you want to send a friend request to.")
                .EmailAddress().WithMessage("Invalid email format.");
        }
    }
}