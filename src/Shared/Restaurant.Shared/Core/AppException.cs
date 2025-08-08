using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Shared.Core
{
    public class AppException : Exception
    {
        public int Code { get; }
        public string ErrorType { get; }

        public AppException(string message, int statusCode = (int)ErrorCode.BAD_REQUEST, string errorType = nameof(ErrorCode.BAD_REQUEST))
            : base(message)
        {
            Code = statusCode;
            ErrorType = errorType;
        }
    }

    public enum ErrorCode
    {
        VALIDATION_ERROR = 400,
        ENTITY_NOT_FOUND = 404,
        UNAUTHORIZED = 401,
        FORBIDDEN = 403,
        INTERNAL_ERROR = 500,
        BAD_REQUEST = 400,
        DUPLICATE_RESOURCE = 409,
        CONFLICT = 409,
        TIMEOUT = 408,
        SERVICE_UNAVAILABLE = 503
    }
}
