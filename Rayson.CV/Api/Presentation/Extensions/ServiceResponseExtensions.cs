using Application.Core;
using Application.Health;

namespace Presentation.Extensions;

public static class ServiceResponseExtensions
{
    public static IResult ToHttpResult<TServiceResponse>(this ServiceResponse<TServiceResponse> serviceResponse)
    {
        if (serviceResponse.ResultCode == ServiceResponseCodes.ValidationProblem)
        {
            return TypedResults.ValidationProblem(serviceResponse.ValidationProblems.ToValidationDictionary());
        }
        else if (serviceResponse.ResultCode == ServiceResponseCodes.UnexpectedError)
        {
            return TypedResults.Problem(string.Join(Environment.NewLine, serviceResponse.ErrorMessages));
        }
        else if (serviceResponse.ResultCode == ServiceResponseCodes.NoContent)
        {
            return TypedResults.NoContent();
        }
        return TypedResults.Ok(serviceResponse.Payload);
    }

    public static IResult ToHttpResult(this ServiceResponse serviceResponse)
    {
        if (serviceResponse.ResultCode == ServiceResponseCodes.ValidationProblem)
        {
            return TypedResults.ValidationProblem(serviceResponse.ValidationProblems.ToValidationDictionary());
        }
        else if (serviceResponse.ResultCode == ServiceResponseCodes.UnexpectedError)
        {
            return TypedResults.Problem(string.Join(Environment.NewLine, serviceResponse.ErrorMessages));
        }
        else if (serviceResponse.ResultCode == ServiceResponseCodes.NoContent)
        {
            return TypedResults.NoContent();
        }

        return TypedResults.Ok();
    }

    public static IResult ToHealthHttpResult(this ServiceResponse<HealthResult> serviceResponse)
    {
        if (serviceResponse.ResultCode == ServiceResponseCodes.ServiceUnavailable)
        {
            return TypedResults.Json(serviceResponse.Payload, statusCode: 503);
        }
        return serviceResponse.ToHttpResult();
    }

    private static Dictionary<string, string[]> ToValidationDictionary(this IEnumerable<string> validationMessages)
    {
        return new Dictionary<string, string[]> { { "Validation", validationMessages.ToArray() } };
    }
}
