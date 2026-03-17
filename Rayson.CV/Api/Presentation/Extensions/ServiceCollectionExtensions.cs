using FluentValidation;
using Application.Contact;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;
using Presentation.Endpoints.Contact;

namespace Presentation.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddPresentationServices(this IServiceCollection services)
    {
        services.AddScoped<IValidator<ContactRequest>, ContactRequestValidator>();
        services.AddAutoMapper(typeof(Program));
        services.AddFluentValidationAutoValidation();
    }
}
