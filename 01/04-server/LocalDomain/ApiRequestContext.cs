using _04_shared_domain;

using Akka.Actor;

using System;
using System.Collections.Generic;
using System.Text;

namespace _04_server.LocalDomain
{
    internal class NoContext
    {
        public static readonly NoContext Instance = new NoContext();
    }

    internal class ApiRequestContext
    {
        public ApiRequestContext(IActorRef origin, ApiRequest request)
        {
            this.Origin = origin;
            this.Request = request;
        }

        public IActorRef Origin { get; }
        public ApiRequest Request { get; }
    }

    internal class ApiRequestContext<TRequest, TContext> : ApiRequestContext
        where TRequest : ApiRequest
        where TContext : class
    {
        public ApiRequestContext(IActorRef origin, TRequest request, TContext context = default)
            : base(origin, request)
        {
            this.Context = context;
        }

        public new TRequest Request => base.Request as TRequest;

        public TContext Context { get; }

        public ApiRequestContext<TRequest, TNewContext> Change<TNewContext>(TNewContext newContext)
            where TNewContext : class
        {
            return new ApiRequestContext<TRequest, TNewContext>(
                this.Origin,
                this.Request,
                newContext);
        }
    }

    internal class ApiRequestContext<TRequest> : ApiRequestContext<TRequest, NoContext>
        where TRequest : ApiRequest
    {
        public ApiRequestContext(IActorRef origin, TRequest request)
            : base(origin, request, NoContext.Instance)
        {
        }
    }

}
