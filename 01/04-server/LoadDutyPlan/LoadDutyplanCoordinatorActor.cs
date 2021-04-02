using _04_server.LocalDomain;

using _04_shared_domain;
using _04_shared_domain.Data;
using _04_shared_domain.Request;

using Akka.Actor;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _04_server.LoadDutyPlan
{
    internal class LoadDutyplanCoordinatorActor : AbstractRequestHandlerActor
    {
        public const string Name = "dutyplan-loader";

        private readonly IActorRef employmentShardRef;
        private readonly IActorRef dutiesDeviationsShardRef;
        private readonly IActorRef normtimeShardRef;
        private readonly IActorRef absenceShardRef;
        private readonly IActorRef validationShardRef;

        internal static Props Props
            => Props.Create(typeof(LoadDutyplanCoordinatorActor));

        public LoadDutyplanCoordinatorActor()
        {
            var selfName = Context.Self.Path.Name;

            // spawn dependent actors
            this.employmentShardRef =
                Context.ActorOf(EmploymentShardActor.GetProps(), $"{EmploymentShardActor.Name}{selfName}");

            this.dutiesDeviationsShardRef =
                Context.ActorOf(DutyDeviationShardActor.GetProps(), $"{DutyDeviationShardActor.Name}{selfName}");

            this.normtimeShardRef =
                Context.ActorOf(NormtimeShardActor.GetProps(), $"{NormtimeShardActor.Name}{selfName}");

            this.absenceShardRef =
                Context.ActorOf(AbsenceShardActor.GetProps(), $"{AbsenceShardActor.Name}{selfName}");

            this.validationShardRef =
                Context.ActorOf(ValidationShardActor.GetProps(), $"{ValidationShardActor.Name}{selfName}");

            // build handlers
            this.ReceiveAsync<ApiRequestContext<LoadDutyplanRequest>>(this.HandleLoadDutyplanRequestAsync);
        }

        private async Task HandleLoadDutyplanRequestAsync(ApiRequestContext<LoadDutyplanRequest> requestContext)
        {
            await this.ExecuteMeasuredApiRequestAsync(
                requestContext.Request,
                x => this.ProcessLoadDutyPlanRequestAsync(requestContext),
                this.CallerName(requestContext.Request.Id))
                .ConfigureAwait(continueOnCapturedContext: true);
        }

        private async Task ProcessLoadDutyPlanRequestAsync(ApiRequestContext<LoadDutyplanRequest> requestContext)
        {
            LoadDutyplanRequest request = requestContext.Request;

            // fetch and wait for employment data first. it is prerequisite for next parallelized steps
            ApiResponse<LoadDutyplanRequest, EmploymentShard[]> employmentShardsResponse =
                await this.employmentShardRef
                    .Ask<ApiResponse<LoadDutyplanRequest, EmploymentShard[]>>(requestContext)
                    .ConfigureAwait(continueOnCapturedContext: true);

            if (employmentShardsResponse.Error)
            {
                this.Log($"Cannot process employments for '{request}'. Skipping rest of pipeline");
                return;
            }

            // wrapped original request with additional context data for actors to work properly
            ApiRequestContext<LoadDutyplanRequest, long[]> requestContextWithEmployments = 
                requestContext.Change(
                    employmentShardsResponse
                        .Result
                        .Select(x => x.Id)
                        .ToArray());

            // spin further actions to further actors as parallel.
            // they will respond when ready to.
            Task<ApiResponse>[] steps = new Task<ApiResponse>[]
            {
                // duty and deviation data
                this.dutiesDeviationsShardRef.Ask<ApiResponse>(requestContextWithEmployments),

                // calculations data
                this.normtimeShardRef.Ask<ApiResponse>(requestContextWithEmployments),

                // absence data
                this.absenceShardRef.Ask<ApiResponse>(requestContextWithEmployments),

                // validations data
                this.validationShardRef.Ask<ApiResponse>(requestContextWithEmployments),
            };

            // await for all operations to finish until next message will be processed from queue
            await Task.WhenAll(steps.ToArray());

            int stepsOk = steps.Count(x => x.IsCompletedSuccessfully && x.Result.Success);

            this.Log($"All parts of {nameof(LoadDutyplanRequest)}.ID = {request.Id} finished. {stepsOk} out of {steps.Length} succeeded");
        }
    }
}
