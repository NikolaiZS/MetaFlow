using FluentValidation;

namespace MetaFlow.Api.Features.BoardMembers.UpdateMemberRole
{
    public class UpdateMemberRoleValidator : AbstractValidator<UpdateMemberRoleCommand>
    {
        public UpdateMemberRoleValidator()
        {
            RuleFor(x => x.BoardId)
                .NotEmpty().WithMessage("Board ID is required");

            RuleFor(x => x.MemberId)
                .NotEmpty().WithMessage("Member ID is required");

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Role is required")
                .Must(role => new[] { "admin", "member", "viewer" }.Contains(role))
                .WithMessage("Invalid role. Must be: admin, member, or viewer");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required");
        }
    }
}
