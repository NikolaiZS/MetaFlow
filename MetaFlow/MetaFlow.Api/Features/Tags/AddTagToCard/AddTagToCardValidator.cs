using FluentValidation;

namespace MetaFlow.Api.Features.Tags.AddTagToCard
{
    public class AddTagToCardValidator : AbstractValidator<AddTagToCardCommand>
    {
        public AddTagToCardValidator()
        {
            RuleFor(x => x.CardId)
                .NotEmpty().WithMessage("Card ID is required");

            RuleFor(x => x.TagId)
                .NotEmpty().WithMessage("Tag ID is required");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required");
        }
    }
}
