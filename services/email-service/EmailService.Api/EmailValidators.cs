using FluentValidation;

namespace EmailService.Api;

public class EmailRequestValidator : AbstractValidator<EmailRequest>
{
    public EmailRequestValidator()
    {
        RuleFor(x => x.To)
            .NotEmpty().WithMessage("Recipient email is required")
            .EmailAddress().WithMessage("Invalid email address format");

        RuleFor(x => x.Subject)
            .NotEmpty().WithMessage("Email subject is required")
            .MaximumLength(200).WithMessage("Subject must not exceed 200 characters");

        RuleFor(x => x.Template)
            .NotEmpty().When(x => string.IsNullOrEmpty(x.Body))
            .WithMessage("Either Template or Body must be provided");

        RuleFor(x => x.Body)
            .NotEmpty().When(x => string.IsNullOrEmpty(x.Template))
            .WithMessage("Either Template or Body must be provided");

        RuleFor(x => x.Attachments)
            .Must(list => list == null || list.All(a => a.Data.Length <= 10 * 1024 * 1024))
            .When(x => x.Attachments != null)
            .WithMessage("Each attachment must not exceed 10MB");

        RuleFor(x => x.Attachments)
            .Must(list => list == null || list.Count <= 5)
            .When(x => x.Attachments != null)
            .WithMessage("Maximum 5 attachments allowed");
    }
}

public class BatchEmailRequestValidator : AbstractValidator<IEnumerable<EmailRequest>>
{
    public BatchEmailRequestValidator()
    {
        RuleForEach(x => x).SetValidator(new EmailRequestValidator());

        RuleFor(x => x)
            .Must(list => list.Count() <= 100)
            .WithMessage("Maximum 100 emails per batch");
    }
}
