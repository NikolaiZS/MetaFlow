using FluentValidation;

namespace MetaFlow.Api.Features.Cards.MoveCard
{
    public class MoveCardValidator : AbstractValidator<MoveCardCommand>
    {
        public MoveCardValidator()
        {
            RuleFor(x => x.CardId)
                .NotEmpty().WithMessage("Card ID is required");

            RuleFor(x => x.TargetColumnId)
                .NotEmpty().WithMessage("Target column ID is required");

            RuleFor(x => x.Position)
                .GreaterThanOrEqualTo(0).WithMessage("Position must be non-negative");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required");
        }
    }
}
