using FluentValidation;

namespace MetaFlow.Api.Features.Cards.UpdateCard
{
    public class UpdateCardValidator : AbstractValidator<UpdateCardCommand>
    {
        public UpdateCardValidator()
        {
            RuleFor(x => x.Title)
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters")
                .When(x => !string.IsNullOrEmpty(x.Title));

            RuleFor(x => x.Description)
                .MaximumLength(5000).WithMessage("Description is too long")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.Priority)
                .Must(p => new[] { "low", "medium", "high", "urgent" }.Contains(p))
                .WithMessage("Invalid priority")
                .When(x => !string.IsNullOrEmpty(x.Priority));

            RuleFor(x => x.Status)
                .Must(s => new[] { "active", "blocked", "completed" }.Contains(s))
                .WithMessage("Invalid status")
                .When(x => !string.IsNullOrEmpty(x.Status));

            RuleFor(x => x.CardId)
                .NotEmpty().WithMessage("Card ID is required");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required");
        }
    }
}
