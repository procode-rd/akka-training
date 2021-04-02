using _04_server.LocalDomain;
using _04_server.RepoMocks;

using _04_shared_domain;
using _04_shared_domain.Data;
using _04_shared_domain.Request;

using Akka.Actor;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _04_server.LoadDutyPlan
{
    internal class ValidationShardActor : AbstractShardActor<ValidationFrame>
    {
        public const string Name = "validation";

        public static Props GetProps()
            => Props.Create(typeof(ValidationShardActor));

        public ValidationShardActor()
            : base(10, 15, NewDtoDecorator)
        {
            this.Receive<ApiRequestContext<LoadDutyplanRequest, long[]>>(this.HandleLoadDutyplanRequest);
        }

        private void HandleLoadDutyplanRequest(ApiRequestContext<LoadDutyplanRequest, long[]> requestContext)
        {
            var response = this.ExecuteMeasuredApiRequest(
                requestContext.Request,
                request =>
                {
                    // randomize + time waste
                    var values = new List<ValidationFrame>();

                    foreach (long employmentId in requestContext.Context)
                    {
                        for (int i = 0; i <= (requestContext.Request.To - requestContext.Request.From).Days; i++)
                        {
                            var stamp = requestContext.Request.From.AddDays(i);

                            values.Add(this.Repo.Get(new { employmentId, stamp }));
                        }
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
        private static void NewDtoDecorator(IRepo<ValidationFrame> repo, ValidationFrame frame, dynamic context)
        {
            var dice = repo.Rng.Next(100);
            frame.ValidationType =
                dice > 98 ? ValidationType.Error :
                    dice > 90 ? ValidationType.Warning :
                        dice > 88 ? ValidationType.Notice
                            : ValidationType.Ok;

            frame.EmploymentId = context.employmentId;
            frame.When = context.stamp;
            frame.Message = frame.ValidationType.ToString();
        }
    }
}
