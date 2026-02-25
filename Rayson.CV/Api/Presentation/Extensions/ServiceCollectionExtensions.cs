using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

namespace Presentation.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddPresentationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(Program));
        services.AddFluentValidationAutoValidation();
    }
}
