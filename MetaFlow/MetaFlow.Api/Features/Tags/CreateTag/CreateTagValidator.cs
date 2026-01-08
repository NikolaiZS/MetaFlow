using FluentValidation;

namespace MetaFlow.Api.Features.Tags.CreateTag
{
    public class CreateTagValidator : AbstractValidator<CreateTagCommand>
    {
        public CreateTagValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Tag name is required")
                .MaximumLength(50).WithMessage("Tag name must not exceed 50 characters");

            RuleFor(x => x.Color)
                .NotEmpty().WithMessage("Tag color is required")
                .Matches("^#[0-9A-Fa-f]{6}$").WithMessage("Color must be a valid hex color (e.g., #FF5733)");

            RuleFor(x => x.BoardId)
                .NotEmpty().WithMessage("Board ID is required");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required");
        }
    }
}
