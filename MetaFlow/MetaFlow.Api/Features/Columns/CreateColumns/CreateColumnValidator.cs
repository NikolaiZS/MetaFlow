using FluentValidation;

namespace MetaFlow.Api.Features.Columns.CreateColumns
{
    public class CreateColumnValidator : AbstractValidator<CreateColumnCommand>
    {
        public CreateColumnValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Column name is required")
                .MaximumLength(100).WithMessage("Column name must not exceed 100 characters");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description is too long")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.ColumnType)
                .NotEmpty().WithMessage("Column type is required")
                .Must(type => new[] { "standard", "backlog", "done", "archive" }.Contains(type))
                .WithMessage("Invalid column type");

            RuleFor(x => x.WipLimit)
                .GreaterThan(0).WithMessage("WIP limit must be greater than 0")
                .When(x => x.WipLimit.HasValue);

            RuleFor(x => x.BoardId)
                .NotEmpty().WithMessage("Board ID is required");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required");
        }
    }
}