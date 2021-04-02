using System;
using System.Collections.Generic;
using System.Text;

using _04_shared_domain;
using _04_shared_domain.Data;
using _04_shared_domain.Request;

using Akka.Actor;

namespace _04_gui_client_wpf.Transport
{
    /// <summary>
    /// Encapsulates remote actor ref and handles all messages (in and out) from local actor system.
    /// </summary>
    public class RemoteApiBridgeActor : ReceiveActor
    {
        private readonly IActorRef remoteApi;
        private readonly Action<ApiResponse> responseReceiver;

        public static Props GetProps(IActorRef remoteApi, Action<ApiResponse> responseReceiver)
            => Props.Create(typeof(RemoteApiBridgeActor), remoteApi, responseReceiver);

        public RemoteApiBridgeActor(IActorRef remoteApi, Action<ApiResponse> responseReceiver)
        {
            this.remoteApi = Context.Watch(remoteApi);
            this.responseReceiver = responseReceiver;
        }

        protected override void PreStart()
        {
            this.Become(() =>
            {
                // send
                this.Receive<ApiRequest>(this.WhenApiRequestReceived);

                // receive
                this.Receive<ApiResponse>(this.WhenApiResponseReceived);

                // monitor
                this.Receive<Terminated>(this.WhenConnectionLost);
            });
        }

        protected virtual void OnCallResponseReceiver(ApiResponse response)
        {
            this.responseReceiver?.Invoke(response);
        }

        private void WhenApiRequestReceived(ApiRequest request)
        {
            this.remoteApi.Tell(request);
        }

        private void WhenApiResponseReceived(ApiResponse response)
        {
            this.OnCallResponseReceiver(response);
        }

        private void WhenConnectionLost(Terminated message)
        {
            if (message.ActorRef.Path == this.remoteApi.Path)
            {
                this.OnCallResponseReceiver(new ApiResponse<DisconnectRequest>(
                    new DisconnectRequest(),
                    success: true,
                    $"Connection to remote '{this.remoteApi}' has been lost"));
            }
        }
    }
}
