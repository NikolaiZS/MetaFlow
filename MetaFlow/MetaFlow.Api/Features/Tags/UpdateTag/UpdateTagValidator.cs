using FluentValidation;

namespace MetaFlow.Api.Features.Tags.UpdateTag
{
    public class UpdateTagValidator : AbstractValidator<UpdateTagCommand>
    {
        public UpdateTagValidator()
        {
            RuleFor(x => x.Name)
                .MaximumLength(50).WithMessage("Tag name must not exceed 50 characters")
                .When(x => !string.IsNullOrEmpty(x.Name));

            RuleFor(x => x.Color)
                .Matches("^#[0-9A-Fa-f]{6}$").WithMessage("Color must be a valid hex color (e.g., #FF5733)")
                .When(x => !string.IsNullOrEmpty(x.Color));

            RuleFor(x => x.TagId)
                .NotEmpty().WithMessage("Tag ID is required");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required");
        }
    }
}
