using FluentValidation;

namespace MetaFlow.Api.Features.Cards.CreateCard
{
    public class CreateCardValidator : AbstractValidator<CreateCardCommand>
    {
        public CreateCardValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Card title is required")
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters");

            RuleFor(x => x.Description)
                .MaximumLength(5000).WithMessage("Description is too long")
                .When(x => !string.IsNullOrWhiteSpace(x.Description));

            RuleFor(x => x.Priority)
                .Must(p => new[] { "low", "medium", "high", "urgent" }.Contains(p))
                .WithMessage("Invalid priority. Must be: low, medium, high or urgent");

            RuleFor(x => x.ColumnId)
                .NotEmpty().WithMessage("Column ID is required");

            RuleFor(x => x.BoardId)
                .NotEmpty().WithMessage("Board ID is required");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required");

            RuleFor(x => x.DueDate)
                .GreaterThan(DateTime.UtcNow).WithMessage("Due date must be in the future")
                .When(x => x.DueDate.HasValue);
        }
    }
}