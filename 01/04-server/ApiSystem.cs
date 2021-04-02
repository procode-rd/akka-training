using _04_server.LocalDomain;

using _04_shared_domain;

using Akka.Actor;
using Akka.Configuration;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Topshelf;

namespace _04_server
{
    class ApiSystem : ServiceControl
    {
        private readonly ActorSystem actorSystem;
        private IActorRef bootstrapActor;

        public ApiSystem(string name)
        {
            const string AkkaConfig = "_04_server.akka.conf";
            var config = ConfigurationFactory.FromResource(AkkaConfig, this);

            this.actorSystem = ActorSystem.Create(name, config);
        }

        public bool Start(HostControl hostControl)
        {
            this.bootstrapActor = this.actorSystem.ActorOf(
                BootstrapActor.Props,
                BootstrapActor.Name);

            Task<ApiResponse> taskResponse = this.bootstrapActor.Ask<ApiResponse>(
                new StartRequest(),
                TimeSpan.FromSeconds(30));

            taskResponse.Wait();

            return taskResponse.Status == TaskStatus.RanToCompletion && taskResponse.Result.Success;
        }

        public bool Stop(HostControl hostControl)
        {
            Task<ApiResponse> taskResponse = this.bootstrapActor.Ask<ApiResponse>(
               new StopRequest(),
               TimeSpan.FromSeconds(30));

            taskResponse.Wait();

            this.actorSystem.Terminate().Wait();

            return taskResponse.Status == TaskStatus.RanToCompletion && taskResponse.Result.Success;
        }
    }
}
