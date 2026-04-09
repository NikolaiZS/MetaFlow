using FluentValidation;

namespace MetaFlow.Api.Features.Columns.ReorderColumns
{
    public class ReorderColumnsValidator : AbstractValidator<ReorderColumnsCommand>
    {
        public ReorderColumnsValidator()
        {
            RuleFor(x => x.BoardId)
                .NotEmpty().WithMessage("Board ID is required");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required");

            RuleFor(x => x.Columns)
                .NotEmpty().WithMessage("Columns list is required")
                .Must(columns => columns.Select(c => c.ColumnId).Distinct().Count() == columns.Count)
                .WithMessage("Duplicate column IDs found");
        }
    }
}