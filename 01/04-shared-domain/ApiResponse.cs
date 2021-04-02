using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _04_shared_domain
{
    public class ApiResponse
    {
        protected ApiResponse()
        { 
        }

        public ApiResponse(ApiRequest request, bool success)
            : this(request, success, default)
        { 
        }

        public ApiResponse(ApiRequest request, bool success, object result)
            : this(request, success, default, result)
        {

        }

        public ApiResponse(ApiRequest request, bool success, string message, object result)
        {
            this.Request = request;
            this.Success = success;
            this.Result = result;

            this.Message = string.IsNullOrEmpty(message) 
                ? success 
                    ? "OK" 
                    : "Error"
                : message;
        }

        public ApiRequest Request { get; set; }
        public bool Success { get; set; }
        public bool Error => !this.Success;
        public object Result { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append($"Response[{this.Message}] with ");

            if (this.Result != null)
            {
                sb.Append("Data=");
                if (this.Result is IList enumerable)
                {
                    int count = enumerable.Count;
                    sb.Append(count);
                    sb.Append(" of ");
                    sb.Append(count > 0 ? enumerable[0].GetType().Name : "?");
                }
                else
                {
                    sb.Append(this.Result.GetType().Name);
                }
            }
            else
            {
                sb.Append("no data");
            }

            sb.Append(" for ");
            sb.Append(this.Request);

            return sb.ToString();
        }
    }

    public class ApiResponse<TRequest> : ApiResponse
        where TRequest : ApiRequest
    {
        protected ApiResponse()
        {
        }

        public ApiResponse(ApiRequest request, bool success)
            : this(request, success, default, default)
        {
        }

        public ApiResponse(ApiRequest request, bool success, string message)
            : this(request, success, message, default)
        {
        }
        protected ApiResponse(ApiRequest request, bool success, string message, object result)
            : base(request, success, message, result)
        {
        }

        public new TRequest Request
        {
            get => base.Request as TRequest;
            set => base.Request = value;
        }
    }

    public class ApiResponse<TRequest, TResult> : ApiResponse<TRequest>
        where TRequest : ApiRequest
    {
        protected ApiResponse()
        {
        }

        public ApiResponse(ApiRequest request, bool success, TResult result)
            : this(request, success, default, result)
        {
        }

        public ApiResponse(ApiRequest request, bool success, string message, TResult result)
            : base(request, success, message, result)
        {
        }

        public new TResult Result
        {
            get => (TResult)base.Result;
            set => base.Result = value;
        }
    }
}
