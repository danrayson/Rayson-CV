using Microsoft.AspNetCore.Mvc;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;
using Application.Auth;
using Presentation.Extensions;
using System.Web;

namespace Presentation.Endpoints.Auth;

public static class AuthenticationEndpoints
{
    public static void MapAuthEndpoints(this WebApplication webApplication)
    {
        var group = webApplication.MapGroup("auth")
            .AddFluentValidationAutoValidation();
        group.MapPost("signup", SignUpEmailPassword);
        group.MapPost("signin", SignInEmailPassword);
        group.MapPost("request-password-reset", SendEmailPasswordResetConfirmationEmail);
        group.MapPut("reset-password", ChangePassword);
    }

    private static async Task<IResult> SignUpEmailPassword(
        [FromServices] IAuthService authService,
        [FromBody] SignUpEmailPasswordRequest request)
    {
        var result = await authService.SignUp(request.DisplayName, request.Email, request.Password);
        if (result.ResultCode == Application.Core.ServiceResponseCodes.Ok)
        {
            return Results.Created();
        }
        return result.ToHttpResult();
    }

    private static async Task<IResult> SignInEmailPassword(
        HttpContext httpContext,
        [FromServices] IAuthService authService,
        [FromBody] SignInEmailPasswordRequest request)
    {
        var tokenResponse = await authService.SignIn(request.Email, request.Password);
        if (tokenResponse.ResultCode == Application.Core.ServiceResponseCodes.Ok && tokenResponse.Payload is not null)
        {
            httpContext.Response.Headers.Remove("X-Auth-Token");
            httpContext.Response.Headers.Append("X-Auth-Token", tokenResponse.Payload.Token);
            return Results.Ok("Login Success - See header 'X-Auth-Token'");
        }
        else
        {
            return tokenResponse.ToHttpResult();
        }
    }

    private static async Task<IResult> SendEmailPasswordResetConfirmationEmail(
        HttpContext httpContext,
        [FromServices] IAuthService authService,
        [FromBody] RequestPasswordResetRequest request)
    {
        var response = await authService.SendEmailPasswordResetConfirmationEmail(request.Email, request.WebsiteUrl);
        if (response.ResultCode == Application.Core.ServiceResponseCodes.Ok)
        {
            return Results.Ok();
        }
        return response.ToHttpResult();
    }

    private static async Task<IResult> ChangePassword(
        HttpContext httpContext,
        [FromServices] IAuthService authService,
        [AsParameters] ChangePasswordRequest request)
    {
        var token = HttpUtility.UrlDecode(request.Token);
        var response = await authService.ChangePassword(request.Email, token, request.Password, request.PasswordCheck);
        if (response.ResultCode == Application.Core.ServiceResponseCodes.Ok)
        {
            return Results.Ok();
        }
        return response.ToHttpResult();
    }
}
