using _04_server.LocalDomain;

using _04_shared_domain;

using Akka.Actor;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace _04_server
{
    public class BootstrapActor : ReceiveActor
    {
        public const string Name = "root";

        private IActorRef api;

        internal static Props Props 
            => Props.Create(typeof(BootstrapActor));

        public BootstrapActor()
        {
            this.Receive<StartRequest>(this.HandleStartRequest);
            this.Receive<StopRequest>(this.HandleStopRequest);
        }

        private void HandleStopRequest(StopRequest request)
        {
            this.StopServingApi();

            Context.Sender.Tell(new ApiResponse(request, success: true));
        }

        private void HandleStartRequest(StartRequest request)
        {
            this.StopServingApi();

            this.api = Context
                .ActorOf(ApiActor.Props, ApiActor.Name);

            Context.Sender.Tell(new ApiResponse(request, success: true));
        }

        private void StopServingApi()
        {
            if (this.api != null)
            {
                this.api.Tell(PoisonPill.Instance);
                this.api = null;
            }
        }
    }
}
