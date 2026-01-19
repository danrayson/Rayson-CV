using Application.Core;

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

    private static Dictionary<string, string[]> ToValidationDictionary(this IEnumerable<string> validationMessages)
    {
        return new Dictionary<string, string[]> { { "Validation", validationMessages.ToArray() } };
    }
}
