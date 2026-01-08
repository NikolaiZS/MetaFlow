using FluentValidation;

namespace MetaFlow.Api.Features.BoardMembers.InviteMember
{
    public class InviteMemberValidator : AbstractValidator<InviteMemberCommand>
    {
        public InviteMemberValidator()
        {
            RuleFor(x => x.BoardId)
                .NotEmpty().WithMessage("Board ID is required");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required");

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Role is required")
                .Must(role => new[] { "owner", "admin", "member", "viewer" }.Contains(role))
                .WithMessage("Invalid role. Must be: owner, admin, member, or viewer");

            RuleFor(x => x.InvitedById)
                .NotEmpty().WithMessage("Inviter user ID is required");
        }
    }
}
