namespace Application.Core;


public class ServiceResponse
    {
        public ServiceResponseCodes ResultCode { get; set; }
        public IEnumerable<string> ErrorMessages { get; set; } = [];
        public IEnumerable<string> ValidationProblems { get; set; } = [];

        public static ServiceResponse Succeed()
        {
            return new ServiceResponse
            {
                ResultCode = ServiceResponseCodes.Ok
            };
        }

        public static ServiceResponse<T> Succeed<T>(T result)
        {
            return ServiceResponse<T>.Succeed(result);
        }

        public static ServiceResponse Fail()
        {
            return new ServiceResponse { ResultCode = ServiceResponseCodes.UnexpectedError };
        }

        public static ServiceResponse Fail(params string[] errors)
        {
            return new ServiceResponse
            {
                ErrorMessages = errors,
                ResultCode = ServiceResponseCodes.UnexpectedError
            };
        }

        public static ServiceResponse Fail(IEnumerable<string> errors)
        {
            return Fail(errors.ToArray());
        }

        public static ServiceResponse Invalid(params string[] validationProblems)
        {
            return new ServiceResponse
            {
                ErrorMessages = ["Invalid"],
                ResultCode = ServiceResponseCodes.ValidationProblem,
                ValidationProblems = validationProblems
            };
        }

        public static ServiceResponse Invalid()
        {
            return Invalid([]);
        }

        /// <summary>
        /// Convert from a generic result to a non-result response
        /// </summary>
        /// <param name="other"></param>
        public static implicit operator ServiceResponse(ServiceResponse<object> other)
        {
            return new ServiceResponse { ResultCode = other.ResultCode, ErrorMessages = other.ErrorMessages, ValidationProblems = other.ValidationProblems };
        }

        /// <summary>
        /// This is a hacky way to resolve the autocasting for the responses from repositories
        /// </summary>
        /// <param name="other"></param>
        public static implicit operator ServiceResponse(ServiceResponse<int> other)
        {
            return new ServiceResponse { ResultCode = other.ResultCode, ErrorMessages = other.ErrorMessages, ValidationProblems = other.ValidationProblems };
        }

        /// <summary>
        /// This is a hacky way to resolve the autocasting
        /// </summary>
        /// <param name="other"></param>
        public static implicit operator ServiceResponse(ServiceResponse<string> other)
        {
            return new ServiceResponse { ResultCode = other.ResultCode, ErrorMessages = other.ErrorMessages, ValidationProblems = other.ValidationProblems };
        }
    }

    public class ServiceResponse<T>
    {
        public ServiceResponseCodes ResultCode { get; set; }
        public IEnumerable<string> ErrorMessages { get; set; } = [];
        public IEnumerable<string> ValidationProblems { get; set; } = [];
        public T? Payload { get; private init; }

        public static ServiceResponse<T> Succeed(T result)
        {
            return new ServiceResponse<T>
            {
                ResultCode = ServiceResponseCodes.Ok,
                Payload = result
            };
        }

        public static ServiceResponse<T> NoResults(T result)
        {
            return new ServiceResponse<T>
            {
                ResultCode = ServiceResponseCodes.NoContent,
                Payload = result
            };
        }

        public static ServiceResponse<T> Invalid(IEnumerable<string> validationProblems)
        {
            return new ServiceResponse<T>
            {
                ResultCode = ServiceResponseCodes.ValidationProblem,
                Payload = default,
                ValidationProblems = validationProblems
            };
        }

        public static ServiceResponse<T> Invalid(string validationProblem)
        {
            return Invalid([validationProblem]);
        }

        public static ServiceResponse<T> Fail(T result)
        {
            return new ServiceResponse<T>
            {
                ResultCode = ServiceResponseCodes.UnexpectedError,
                Payload = result
            };
        }

        public static ServiceResponse<T> Fail(params string[] errors)
        {
            return Fail(default, errors);
        }

        public static ServiceResponse<T> Fail(IEnumerable<string> errors)
        {
            return Fail(errors.ToArray());
        }

        private static ServiceResponse<T> Fail(T? result, params string[] errors)
        {
            return new ServiceResponse<T>
            {
                ResultCode = ServiceResponseCodes.UnexpectedError,
                Payload = result,
                ErrorMessages = errors
            };
        }

        /// <summary>
        /// Enables autoconversion for when a normal ServiceResponse is received, but a ServiceResponse<T> is required
        /// </summary>
        /// <param name="other"></param>
        public static implicit operator ServiceResponse<T>(ServiceResponse other)
        {
            return new ServiceResponse<T> { Payload = default, ResultCode = other.ResultCode, ErrorMessages = other.ErrorMessages, ValidationProblems = other.ValidationProblems };
        }

        /// <summary>
        /// This is a hacky way to resolve the autocasting for the responses from repositories
        /// </summary>
        /// <param name="other"></param>
        public static implicit operator ServiceResponse<T>(ServiceResponse<int> other)
        {
            return new ServiceResponse<T> { Payload = default, ResultCode = other.ResultCode, ErrorMessages = other.ErrorMessages, ValidationProblems = other.ValidationProblems };
        }
    }