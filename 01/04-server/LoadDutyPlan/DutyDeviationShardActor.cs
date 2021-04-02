using _04_server.LocalDomain;
using _04_server.RepoMocks;

using _04_shared_domain;
using _04_shared_domain.Data;
using _04_shared_domain.Request;

using Akka.Actor;

using System;
using System.Collections.Generic;
using System.Text;

namespace _04_server.LoadDutyPlan
{
    internal class DutyDeviationShardActor : AbstractShardActor<DutyShard>
    {
        public const string Name = "duties-deviations";

        private static readonly string[] DutyTypes = new[]
        {
            "AUTO", "NT", "DAG",
        };

        private static readonly string[] DeviationTypes = new[]
        {
            "FE", "OF", "6F", "OS", "SO", "RB",
        };

        private readonly IRepo<DeviationShard> repoDeviation;

        public static Props GetProps()
            => Props.Create(typeof(DutyDeviationShardActor));


        public DutyDeviationShardActor()
            : base(2, 10, NewDutyDtoDecorator)
        {
            this.repoDeviation = RepoFactory.Create<DeviationShard>(2, 10, NewDeviationDtoDecorator);

            this.Receive<ApiRequestContext<LoadDutyplanRequest, long[]>>(this.HandleLoadDutyplanRequest);
        }

        private void HandleLoadDutyplanRequest(ApiRequestContext<LoadDutyplanRequest, long[]> requestContext)
        {
            ApiResponse<LoadDutyplanRequest, DutyShard[]> response = this.ExecuteMeasuredApiRequest(
                requestContext.Request,
                request =>
                {
                    // create random data and some dummy time waste
                    // each employment has at least one duty one day in range of requested dutyplan. 
                    // additionally, there can be up to 2 deviations (with zero). simplification for later steps like normtime, absence and validation to work anyhow.
                    var duties = new List<DutyShard>();

                    DateTime from = requestContext.Request.From.Date;
                    DateTime to = requestContext.Request.To.Date;

                    foreach (long employmentId in requestContext.Context)
                    {
                        var stamp = from;

                        while (stamp <= to)
                        {
                            DutyShard duty = this.Repo.Get(new { employmentId, stamp });
                            var deviations = new List<DeviationShard>();

                            for (int j = 0; j < this.Rng.Next(2); j++)
                            {
                                DeviationShard deviation = this.repoDeviation.Get(new { duty });

                                deviations.Add(deviation);
                            }

                            duty.Deviations = deviations.ToArray();
                            duties.Add(duty);

                            stamp = stamp.AddDays(1);
                        }
                    }

                    return duties.ToArray();
                }, 
                this.CallerName(requestContext.Request.Id));

            // send data to requester
            requestContext.Origin.Tell(response);

            // send info to sender
            this.Sender.Tell(response);
        }

        /// <param name="context">bad in normal code, but well, this is an example so random data preparation simplification</param>
        private static void NewDutyDtoDecorator(IRepo<DutyShard> repo, DutyShard x, dynamic context)
        {
            x.DutyType = DutyTypes[repo.Rng.Next(DutyTypes.Length)];
            x.EmploymentId = context.employmentId;
            x.Start = context.stamp + TimeSpan.FromHours(repo.Rng.Next(18));
            x.End = x.Start.AddHours(5.5);
        }

        /// <param name="context">bad in normal code, but well, this is an example so random data preparation simplification</param>
        private static void NewDeviationDtoDecorator(IRepo<DeviationShard> repo, DeviationShard x, dynamic context)
        {
            x.DeviationType = DeviationTypes[repo.Rng.Next(DeviationTypes.Length)];
            x.DutyId = context.duty.Id;
            x.Start = context.duty.Start;
            x.End = context.duty.End;
        }
    }
}
