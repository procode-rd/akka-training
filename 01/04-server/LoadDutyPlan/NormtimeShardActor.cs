using _04_server.LocalDomain;
using _04_server.RepoMocks;

using _04_shared_domain;
using _04_shared_domain.Data;
using _04_shared_domain.Request;

using Akka.Actor;

using System;
using System.Collections.Generic;
using System.Text;

using ValueType = _04_shared_domain.Data.ValueType;

namespace _04_server.LoadDutyPlan
{
    internal class NormtimeShardActor : AbstractShardActor<ValueShard>
    {
        public const string Name = "normtime";

        public static Props GetProps()
            => Props.Create(typeof(NormtimeShardActor));

        public NormtimeShardActor()
            : base(10, 50, NewDtoDecorator)
        {
            this.Receive<ApiRequestContext<LoadDutyplanRequest, long[]>>(this.HandleLoadDutyplanRequest);
        }

        private void HandleLoadDutyplanRequest(ApiRequestContext<LoadDutyplanRequest, long[]> requestContext)
        {
            ApiResponse<LoadDutyplanRequest, ValueShard[]> response = this.ExecuteMeasuredApiRequest(
                requestContext.Request,
                request =>
                {
                    // randomize + time waste
                    var values = new List<ValueShard>();

                    foreach (long employmentId in requestContext.Context)
                    {
                        values.Add(this.Repo.Get(new { employmentId }));
                    }

                    return values.ToArray();
                }, 
                this.CallerName(requestContext.Request.Id));

            // send back
            requestContext.Origin.Tell(response);

            // send info to sender
            this.Sender.Tell(response);
        }

        /// <param name="context">bad in normal code, but well, this is an example so random data preparation simplification</param>
        private static void NewDtoDecorator(IRepo<ValueShard> repo, ValueShard x, dynamic context)
        {
            x.ValueType = ValueType.Normtime;
            x.Value = repo.Rng.Next(300);
            x.EmploymentId = context.employmentId;
        }
    }
}
