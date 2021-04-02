using System;
using System.Collections.Generic;
using System.Text;

using _04_server.LocalDomain;
using _04_server.RepoMocks;

using _04_shared_domain.Data;

using Akka.Actor;

namespace _04_server.LoadDutyPlan
{
    internal abstract class AbstractShardActor<T> : AbstractRequestHandlerActor
        where T : class, IId, new()
    {
        protected AbstractShardActor(uint minResponse, uint maxResponse, Action<IRepo<T>, T, dynamic> repoDecorator)
        {
            this.Rng = new Random();
            this.Repo = RepoFactory.Create(minResponse, maxResponse, repoDecorator);
        }

        protected Random Rng { get; }
        protected IRepo<T> Repo { get; }
    }
}
