using FluentValidation;

namespace MetaFlow.Api.Features.Boards.CreateBoard
{
    public class CreateBoardValidator : AbstractValidator<CreateBoardCommand>
    {
        public CreateBoardValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Board name is required")
                .MaximumLength(200).WithMessage("Board name must not exceed 200 characters");

            RuleFor(x => x.Description)
                .MaximumLength(2000).WithMessage("Description is too long")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.MethodologyPresetId)
                .NotEmpty().WithMessage("Methodology preset is required");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required");
        }
    }

}
