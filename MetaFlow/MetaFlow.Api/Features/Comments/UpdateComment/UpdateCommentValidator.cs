using FluentValidation;

namespace MetaFlow.Api.Features.Comments.UpdateComment
{
    public class UpdateCommentValidator : AbstractValidator<UpdateCommentCommand>
    {
        public UpdateCommentValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Comment content is required")
                .MaximumLength(2000).WithMessage("Comment must not exceed 2000 characters");

            RuleFor(x => x.CommentId)
                .NotEmpty().WithMessage("Comment ID is required");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required");
        }
    }
}
