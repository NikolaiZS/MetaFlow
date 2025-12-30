using FluentValidation;

namespace MetaFlow.Api.Features.Attachments.UploadAttachment
{
    public class UploadAttachmentValidator : AbstractValidator<UploadAttachmentCommand>
    {
        public UploadAttachmentValidator()
        {
            RuleFor(x => x.FileName)
                .NotEmpty().WithMessage("File name is required")
                .MaximumLength(255).WithMessage("File name must not exceed 255 characters");

            RuleFor(x => x.FileUrl)
                .NotEmpty().WithMessage("File URL is required")
                .MaximumLength(1000).WithMessage("File URL is too long");

            RuleFor(x => x.FileSize)
                .GreaterThan(0).WithMessage("File size must be greater than 0")
                .LessThanOrEqualTo(50 * 1024 * 1024).WithMessage("File size must not exceed 50MB");

            RuleFor(x => x.MimeType)
                .NotEmpty().WithMessage("MIME type is required")
                .MaximumLength(100).WithMessage("MIME type is too long");

            RuleFor(x => x.ThumbnailUrl)
                .MaximumLength(1000).WithMessage("Thumbnail URL is too long")
                .When(x => !string.IsNullOrEmpty(x.ThumbnailUrl));

            RuleFor(x => x.CardId)
                .NotEmpty().WithMessage("Card ID is required");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required");
        }
    }
}
