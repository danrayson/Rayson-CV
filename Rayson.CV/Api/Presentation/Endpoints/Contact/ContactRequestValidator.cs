using Application.Contact;
using FluentValidation;

namespace Presentation.Endpoints.Contact;

public class ContactRequestValidator : AbstractValidator<ContactRequest>
{
    public ContactRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MinimumLength(2).WithMessage("Name must be at least 2 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .Matches(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")
            .WithMessage("Invalid email format");

        RuleFor(x => x.Subject)
            .NotEmpty().WithMessage("Subject is required")
            .MinimumLength(5).WithMessage("Subject must be at least 5 characters");

        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Message is required")
            .MinimumLength(10).WithMessage("Message must be at least 10 characters");
    }
}
