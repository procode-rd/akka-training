using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Windows;

using _04_shared_domain;

using Akka.Actor;
using Akka.Configuration;

namespace _04_gui_client_wpf.Transport
{
    /// <summary>
    /// Wraps local actor system instance into class that can be blend with application itself.
    /// Works like adapter of akka world with wpf one.
    /// </summary>
    public class ActorSystemAdapter : IDisposable
    {
        private ActorSystem actorSystem;

        public ActorSystemAdapter()
        {
            const string AkkaConfig = "_04_gui_client_wpf.akka.conf";
            var config = ConfigurationFactory.FromResource(AkkaConfig, this);

            this.actorSystem = ActorSystem.Create(Application.Current.MainWindow.Name, config);
        }

        public IActorRef BindToRemote(string host, int port, Action<ApiResponse> responseReceiver, out string remotePath)
        {
            remotePath = string.Empty;
            try
            {
                IActorRef remoteApi = this.actorSystem
                    .ActorSelection($"akka.tcp://04-akka-server@{host}:{port}/user/root/api-v1")
                    .ResolveOne(TimeSpan.FromSeconds(5))
                    .Result;

                remotePath = remoteApi.Path.ToString();

                return this.actorSystem.ActorOf(
                    RemoteApiBridgeActor.GetProps(remoteApi, responseReceiver),
                    "remoteBridge");
            }
            catch (AggregateException aex)
            {
                var anf = aex.InnerExceptions.FirstOrDefault(x => x is ActorNotFoundException);

                ExceptionDispatchInfo.Capture(anf ?? aex.InnerException ?? aex).Throw();
                return null;
            }
        }

        public void Dispose()
        {
            if (this.actorSystem != null)
            {
                this.actorSystem.Terminate().Wait();
                this.actorSystem = null;
            }
        }
    }
}
