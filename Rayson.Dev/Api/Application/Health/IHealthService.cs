using Application.Core;

namespace Application.Health;

public interface IHealthService
{
    Task<ServiceResponse<HealthResult>> GetHealthAsync();
    Task<ServiceResponse<HealthResult>> GetLivenessAsync();
    Task<ServiceResponse<HealthResult>> GetReadinessAsync();
}
