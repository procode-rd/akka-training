using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Akka.Actor;

namespace _03_supervised_chat
{
    class AdminActor : ReceiveActor
    {
        public enum Command
        {
            Open,
            Close,
            NewChannel,
            ClockTick,
        }

        private static readonly string[] Names = new[]
        {
            "James", "Linda", "Borys", "Tanja", "Adam", "Agnes", "Robert", "Maria", "Jeff", "Mindy"
        };

        private readonly Props chatActorProps;
        private ICancelable clockCancelable;
        private uint openChannels;
        private uint totalChannels;

        public static Props GetProps(Props chatActorProps)
            => Props.Create(typeof(AdminActor), chatActorProps);

        public AdminActor(Props chatActorProps)
        {
            this.chatActorProps = chatActorProps;

            this.Become(() =>
            {
                this.Receive<Command>(this.WhenCommand);
                this.Receive<Terminated>(this.WhenChannelTerminated);
            });
        }

        private void Narrate(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{Context.Self.Path.Name}$ {message}");
        }

        private void WhenCommand(Command command)
        {
            switch (command)
            {
                case Command.Open:
                    this.OpenChat();
                    break;

                case Command.Close:
                    this.CloseChat();
                    break;

                case Command.NewChannel:
                    this.OpenChannel();
                    break;

                case Command.ClockTick:
                    this.Narrate($"channels open: {openChannels} ");
                    break;
            }
        }

        private void OpenChat()
        {
            this.clockCancelable = Context
                .System
                .Scheduler
                .ScheduleTellRepeatedlyCancelable(
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(5),
                    Context.Self,
                    Command.ClockTick,
                    Context.Self);

            this.Narrate($"Chat open");
        }

        private void CloseChat()
        {
            this.clockCancelable.Cancel();

            this.Narrate($"Chat closed");
        }

        private void OpenChannel()
        {
            Random rnd = new Random();

            var chosenNames = new HashSet<string>();

            do
            {
                chosenNames.Add(Names[rnd.Next(Names.Length)]);
            } while (chosenNames.Count < 2);

            string userName1 = chosenNames.First();
            string userName2 = chosenNames.Skip(1).First();

            IActorRef channel = Context.ActorOf(
                ChannelActor.GetProps(this.chatActorProps, userName1, userName2),
                $"channel{this.totalChannels++}-{userName1}:{userName2}");

            this.openChannels++;

            Context.Watch(channel);
        }

        private void WhenChannelTerminated(Terminated msg)
        {
            openChannels--;
            this.Narrate($"Channel {msg.ActorRef.Path.Name} has ended");
        }

        protected override void PostStop()
        {
            this.CloseChat();

            base.PostStop();
        }

        protected override SupervisorStrategy SupervisorStrategy()
            => new OneForOneStrategy(ex => Directive.Stop);
    }
}
