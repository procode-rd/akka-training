using System;
using System.Threading;
using System.Threading.Tasks;

using Akka.Actor;

namespace _01_helloworld
{
    class Program
    {
        static void Main(string[] args)
        {
            // init / setup
            ActorSystem system = ActorSystem.Create("test-system");

            IActorRef actorRef = system.ActorOf<HelloWorldActor>("hello-world");

            // run
            actorRef.Tell("AreYouThere?");

            // wait for exit
            while (!Console.KeyAvailable && Console.ReadKey(true).Key != ConsoleKey.Escape)
            {
                Thread.Sleep(1);
            }

            // cleanup
            system.Terminate().Wait();
        }
    }

    class HelloWorldActor : ReceiveActor
    {
        public static Props Props
            => Props.Create<HelloWorldActor>();

        public HelloWorldActor()
        {
            this.Become(() =>
            {
                this.Receive<string>(this.HelloWorldReceiver, x => x == "AreYouThere?");
            });
        }

        private void HelloWorldReceiver(string message)
        {
            Console.WriteLine($"I'm here! Hello from `{this.Self}`");
        }
    }
}
