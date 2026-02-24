using FluentValidation;

namespace MetaFlow.Api.Features.Comments.CreateComment
{
    public class CreateCommentValidator : AbstractValidator<CreateCommentCommand>
    {
        public CreateCommentValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Comment content is required")
                .MaximumLength(2000).WithMessage("Comment must nop exceed 2000 characters");

            RuleFor(x => x.CardId)
                .NotEmpty().WithMessage("Card ID is required");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required");
        }
    }
}