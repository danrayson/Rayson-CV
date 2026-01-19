using System.Security.Claims;
using Application.Auth;
using Application.Core;
using AutoMapper;
using Database.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Auth;

public class AuthService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ITokenService tokenService, IEmailService emailService, IMapper mapper) : IAuthService
{
    private readonly UserManager<ApplicationUser> userManager = userManager;
    private readonly RoleManager<ApplicationRole> roleManager = roleManager;
    private readonly ITokenService tokenService = tokenService;
    private readonly IEmailService emailService = emailService;
    private readonly IMapper mapper = mapper;

    public async Task<ServiceResponse> SignUp(string displayName, string email, string password)
    {
        ApplicationUser user = new()
        {
            DisplayName = displayName,
            Email = email,
            UserName = email//We don't use this in Domain
        };
        var result = await userManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
            return ServiceResponse.Succeed();
        }
        return ServiceResponse.Invalid(result.Errors.Select(e => $"{e.Description}").ToArray());
    }

    public async Task<ServiceResponse<SignInResponse>> SignIn(string email, string password)
    {
        var identityUser = await userManager.Users.FirstOrDefaultAsync(u => u.Email!.ToLower() == email.ToLower());
        if (identityUser is null)
        {
            //Same response for any reason - more secure.
            return ServiceResponse.Invalid("Email or Password was not found.");
        }

        var isCorrectPassword = await userManager.CheckPasswordAsync(identityUser, password);
        if (!isCorrectPassword)
        {
            //Same response for any reason - more secure.
            return ServiceResponse.Invalid("Email or Password was not found.");
        }

        var token = await tokenService.CreateTokenAsync(identityUser);
        return ServiceResponse.Succeed(new SignInResponse() { Token = token });
    }

    public async Task<ServiceResponse> SendEmailPasswordResetConfirmationEmail(string email, string redirectUrl)
    {
        try
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return ServiceResponse.Invalid("Email not found.");
            }

            // Generate a password reset token
            string resetToken = await userManager.GeneratePasswordResetTokenAsync(user);

            // Encode the token for use in the URL
            string encodedToken = Uri.EscapeDataString(resetToken);

            // Create the email message
            string subject = "Password Reset";
            string body = $"Please reset your password by clicking on this link: <a href='{redirectUrl}?token={encodedToken}'>Reset Password</a>";

            // Send the email
            await emailService.SendEmailAsync(user.Email, subject, body);

            return ServiceResponse.Succeed();
        }
        catch
        {
            return ServiceResponse.Fail("Error sending email");
        }
    }

    public async Task<ServiceResponse<string>> ChangePassword(string email, string token, string password, string passwordCheck)
    {
        // Find the user by ID
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return ServiceResponse.Invalid("User not found.");
        }

        // Check that the new password and confirmation match
        if (!password.Equals(passwordCheck))
        {
            return ServiceResponse.Invalid("New password and confirmation do not match.");
        }

        // Set the user's password
        var result = await userManager.ResetPasswordAsync(user, token, password);
        if (result.Succeeded)
        {
            return ServiceResponse.Succeed("Password changed successfully.");
        }

        // Return any errors from the password reset operation
        return ServiceResponse.Invalid(result.Errors.Select(e => $"{e.Description}").ToArray());
    }
}
