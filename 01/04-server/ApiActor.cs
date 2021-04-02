using _04_server.LoadDutyPlan;
using _04_server.LocalDomain;

using _04_shared_domain;
using _04_shared_domain.Data;
using _04_shared_domain.Request;

using Akka.Actor;
using Akka.Routing;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace _04_server
{
    public class ApiActor : AbstractRequestHandlerActor
    {
        public const string Name = "api-v1";

        internal static Props Props 
            => Props.Create<ApiActor>();

        private readonly IActorRef dutyplanLoaderRef;

        public ApiActor()
        {
            const int ConcurrentLoaders = 10; 

            this.dutyplanLoaderRef =
                Context
                    .ActorOf(
                        LoadDutyplanCoordinatorActor.Props.WithRouter(
                            new ConsistentHashingPool(
                                ConcurrentLoaders,
                                this.GetRoutingHashForLoadDutyplanCoordinatorActor)),
                        LoadDutyplanCoordinatorActor.Name);

            this.Receive<ApiRequest>(this.HandleApiRequest, this.AuthorizeApiRequest);
        }

        protected override void PreStart()
        {
            base.PreStart();

            Console.WriteLine($"Ready at: {Context.Self.Path}, running hosted @ PID: {Process.GetCurrentProcess().Id}");
        }

        private bool AuthorizeApiRequest(ApiRequest request)
        {
            // because security is most important...
            return true;
        }

        private void HandleApiRequest(ApiRequest request)
        {
            IActorRef sender = Context.Sender;

            this.Log($"Executing: '{request}' from {sender}", Akka.Event.LogLevel.InfoLevel);
            
            switch (request)
            {
                case ConnectRequest connectRequest:
                    sender.Tell(
                        new ConnectResponse(
                            connectRequest,
                            System.Reflection.Assembly.GetExecutingAssembly().GetName().Version,
                            $"PID: {Process.GetCurrentProcess().Id} by {Context.Self.Path}"));
                    break;

                case DisconnectRequest disconnectRequest:
                    this.Log($"Remote disconnects by {disconnectRequest.Id}. Hasta la vista!");
                    break;

                case LoadDutyplanRequest loadDutyplanRequest:
                    this.dutyplanLoaderRef.Tell(
                        new ApiRequestContext<LoadDutyplanRequest>(sender, loadDutyplanRequest));
                    break;

                default:
                    sender.Tell(new ApiResponse(request, false, $"{Context.Self.Path}: unable to handle request type `{request.GetType().FullName}`"));
                    return;
            }
        }

        private object GetRoutingHashForLoadDutyplanCoordinatorActor(object message)
        {
            switch (message)
            {
                case ApiRequestContext requestContext:
                    return requestContext.Origin.GetHashCode();

                default:
                    return message.GetHashCode();
            }
        }

    }
}
