using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Akka.Actor;

namespace _03_supervised_chat
{
    class ChannelActor : ReceiveActor
    {
        private readonly IActorRef user1;
        private readonly IActorRef user2;

        public static Props GetProps(Props chatActorProps, string userName1, string userName2)
            => Props.Create(typeof(ChannelActor), chatActorProps, userName1, userName2);

        public ChannelActor(Props chatActorProps, string userName1, string userName2)
        {
            this.user1 = Context.ActorOf(chatActorProps, userName1);
            this.user2 = Context.ActorOf(chatActorProps, userName2);

            Context.Watch(this.user1);
            Context.Watch(this.user2);

            this.Become(() =>
            {
                this.Receive<Terminated>(this.WhenTerminated);
            });
        }

        protected override void PreStart()
        {
            base.PreStart();

            this.Narrate($"{this.user1.Path.Name} entered the channel");
            this.Narrate($"{this.user2.Path.Name} entered the channel");

            this.user1.Tell("go", this.user2);
        }

        private void WhenTerminated(Terminated msg)
        {
            this.Narrate($"{msg.ActorRef.Path.Name} has left the chat.");

            if (!Context.GetChildren().Any())
            {
                this.Narrate($"Noone left in the channel. It's time for the pill. Bye.");
                Context.Self.Tell(PoisonPill.Instance);
                return;
            }

            switch (msg.ActorRef)
            {
                case IActorRef chat when chat == this.user1:
                    this.user2.Tell(PoisonPill.Instance);
                    break;

                case IActorRef chat when chat == this.user2:
                    this.user1.Tell(PoisonPill.Instance);
                    break;
            }            
        }

        private void Narrate(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{Context.Self.Path.Name}| {message}");
        }

        protected override SupervisorStrategy SupervisorStrategy()
            => new AllForOneStrategy(ex => Directive.Restart);
    }
}
