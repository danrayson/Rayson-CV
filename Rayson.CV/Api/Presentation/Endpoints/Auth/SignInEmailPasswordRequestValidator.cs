using FluentValidation;

namespace Presentation.Endpoints.Auth;

public class SignInEmailPasswordRequestValidator : AbstractValidator<SignInEmailPasswordRequest>
{

    public SignInEmailPasswordRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .Length(8, 100).WithMessage("Password must be at least 8 characters long and less than 100 chars long.")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).+$")
            .WithMessage("Password must contain at least one lowercase letter, one uppercase letter, one number, and one special character."); ;
    }
}
