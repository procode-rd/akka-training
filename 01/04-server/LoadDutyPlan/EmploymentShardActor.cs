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
    internal class EmploymentShardActor : AbstractShardActor<EmploymentShard>
    {
        public const string Name = "employment-shard";

        private static readonly string[] Names = new[] 
            { 
                "John", "Linda", "Jeff", "Alice", "Kriss", "Panam", "Arnold", "Susan", "Clark", "Ragnar", "Beth", "Maria", "Jefferey", "Jenice", "Tom", "Isabel", "Lance", "Mindy", "Tony",
            };
        private static readonly string[] Surnames = new[] 
            { 
                "Smith", "Kent", "Trump", "Racoon", "Mace", "Biden", "Rusloff", "Aleca", "Lohan", "Kennedy", "Reje", "Coben", "Johannson", "Stark", "Garcia", "Knight", "Hoermann", "Lewman", "Razzano",
            };

        public static Props GetProps()
            => Props.Create(typeof(EmploymentShardActor));

        public EmploymentShardActor()
            : base(1, 5, NewDtoDecorator)
        {
            this.Receive<ApiRequestContext<LoadDutyplanRequest>>(this.HandleGetEmploymentShardsRequest);
        }

        private void HandleGetEmploymentShardsRequest(ApiRequestContext<LoadDutyplanRequest> requestContext)
        {
            ApiResponse<LoadDutyplanRequest, EmploymentShard[]> response = this.ExecuteMeasuredApiRequest(
                requestContext.Request,
                request =>
                {
                    EmploymentShard[] values = this.Repo
                        .GetMany(
                            10 + this.Rng.Next(90),
                            default)
                        .ToArray();

                    return values;
                }, 
                this.CallerName(requestContext.Request.Id));

            // now we have some into remote can already process, sent it back and process more
            requestContext.Origin.Tell(response);

            // send info to sender
            this.Sender.Tell(response);
        }

        /// <param name="context">bad in normal code, but well, this is an example so random data preparation simplification</param>
        private static void NewDtoDecorator(IRepo<EmploymentShard> repo, EmploymentShard x, dynamic context)
        {
            x.Name = Names[repo.Rng.Next(Names.Length)];
            x.Surname = Surnames[repo.Rng.Next(Surnames.Length)];
        }
    }
}
