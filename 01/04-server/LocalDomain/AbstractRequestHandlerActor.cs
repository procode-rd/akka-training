using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using _04_shared_domain;

using Akka.Actor;
using Akka.Event;

namespace _04_server.LocalDomain
{
    public abstract class AbstractRequestHandlerActor : ReceiveActor
    {
        protected string CallerName(object id, [CallerMemberName] string caller = "")
            => $"{this.GetType().Name}.{caller}({id})";

        protected void Log(string message, LogLevel level = LogLevel.DebugLevel)
        {
            Context.System.Log.Log(level, message);
        }

        protected void Log(Exception exception, LogLevel level = LogLevel.ErrorLevel)
        {
            Context.System.Log.Log(level, exception, exception.GetBaseException().Message);
        }

        protected async Task ExecuteMeasuredApiRequestAsync<TRequest>(
            TRequest request,
            Func<TRequest, Task> func,
            [CallerMemberName] string caller = "")
            where TRequest : ApiRequest
        {
            _ = await this.ExecuteMeasuredApiRequestAsync(
                request,
                async x => { await func(x); return true; },
                caller);
        }

        protected async Task<ApiResponse<TRequest, TResult>> ExecuteMeasuredApiRequestAsync<TRequest, TResult>(
            TRequest request,
            Func<TRequest, Task<TResult>> func,
            [CallerMemberName] string caller = "")
            where TRequest : ApiRequest
        {
            var sw = Stopwatch.StartNew();
            try
            {
                TResult result = await func(request).ConfigureAwait(continueOnCapturedContext: true);

                ApiResponse<TRequest, TResult> response = new ApiResponse<TRequest, TResult>(request, success: true, result);

                return response;
            }
            catch (Exception ex)
            {
                this.Log(ex);

                return new ApiResponse<TRequest, TResult>(request, success: false, default);
            }
            finally
            {
                this.Log($"Execution of ´{caller}´ took {sw.ElapsedMilliseconds}ms");
            }
        }


        protected ApiResponse<TRequest, TResult> ExecuteMeasuredApiRequest<TRequest, TResult>(
            TRequest request, 
            Func<TRequest, TResult> func, 
            [CallerMemberName] string caller = "")
            where TRequest : ApiRequest
        {
            var sw = Stopwatch.StartNew();
            try
            {
                TResult result = func(request);

                ApiResponse<TRequest, TResult> response = new ApiResponse<TRequest, TResult>(request, success: true, result);

                return response;
            }
            catch (Exception ex)
            {
                this.Log(ex);

                return new ApiResponse<TRequest, TResult>(request, success: false, default);
            }
            finally
            {
                this.Log($"Execution of ´{caller}´ took {sw.ElapsedMilliseconds}ms");
            }
            /*
            return this.ExecuteMeasuredApiRequestAsync(
                    request,
                    x => Task.Run(() => func(x)).ConfigureAwait(continueOnCapturedContext: true).GetAwaiter().GetResult(),
                    respondToSender,
                    caller)
                .GetAwaiter()
                .GetResult();*/
        }

        protected ApiResponse<TRequest> ExecuteMeasuredApiRequest<TRequest>(
            TRequest request, 
            Action<TRequest> action, 
            [CallerMemberName] string caller = "")
            where TRequest : ApiRequest
        {
            return this.ExecuteMeasuredApiRequest(
                request, 
                x => 
                { 
                    action(x); 
                    return true; 
                }, 
                caller);
        }

    }
}
