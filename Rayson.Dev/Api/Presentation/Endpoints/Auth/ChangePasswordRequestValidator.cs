using System;
using FluentValidation;

namespace Presentation.Endpoints.Auth;

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(request => request.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email address");

        RuleFor(request => request.Token)
            .NotEmpty().WithMessage("Token is required");

        RuleFor(request => request.Password)
            .NotEmpty().WithMessage("Password is required")
            .Length(8, 100).WithMessage("Password must be at least 8 characters long and less than 100 chars long")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).+$")
            .WithMessage("Must contain at least one lowercase letter, one uppercase letter, one number, and one special character");

        RuleFor(request => request.PasswordCheck)
            .NotEmpty().WithMessage("Confirm password is required")
            .Equal(x => x.Password).WithMessage("Passwords do not match");
    }
}