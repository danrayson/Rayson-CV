using Application.Contact;
using Application.Core;
using Microsoft.AspNetCore.Mvc;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;
using Presentation.Extensions;

namespace Presentation.Endpoints.Contact;

public static class ContactEndpoints
{
    public static void MapContactEndpoints(this WebApplication webApplication)
    {
        var group = webApplication.MapGroup("contact")
            .AddFluentValidationAutoValidation();
        group.MapPost("", SendContactEmail).AllowAnonymous();
    }

    private static async Task<IResult> SendContactEmail(
        [FromServices] IContactService contactService,
        [FromBody] ContactRequest request)
    {
        var result = await contactService.SendContactEmailAsync(request);
        return result.ToHttpResult();
    }
}
