using System;
using System.Collections.Generic;
using System.Threading;

using Akka.Actor;
using Akka.Configuration;

namespace _03_supervised_chat
{
    static class Program
    {
        static Dictionary<Subject, string> questions = new Dictionary<Subject, string>
        {
            [Subject.Name] = "What's your name?",
            [Subject.Age] = "How old are you?",
            [Subject.Mood] = "How do you feel today?",
        };

        static Dictionary<Subject, string[]> answers = new Dictionary<Subject, string[]>
        {
            [Subject.Name] = new[] { "Won't tell ya", default, "Get lost!" },
            [Subject.Age] = new[] { "Haha, nice try", "18", "24", "41", "60", "80" },
            [Subject.Mood] = new[] { "fine", "terrific!", "so-so", "..." },
        };

        static void Main(string[] args)
        {
            // init / setup
            ActorSystem system = ActorSystem.Create("chat-system",
                ConfigurationFactory.ParseString(
                    @"akka {{
                        stdout-loglevel = Off # DEBUG .. ERROR
                        loglevel = Off # DEBUG .. ERROR
                        actor {{
                          debug {{
                              receive = on
                              autoreceive = on
                              lifecycle = on
                              event-stream = on
                              unhandled = on
                            }}
                        }}
                    }}"));

            IActorRef adminRef = system.ActorOf(
                AdminActor.GetProps(
                    Props.Create(typeof(ChatActor), questions, answers)), 
                "admin");

            // run
            adminRef.Tell(AdminActor.Command.Open);

            // wait for exit
            bool shallExit = false;
            while (!shallExit)
            {
                if (Console.KeyAvailable)
                {
                    switch (Console.ReadKey(true).Key)
                    {
                        case ConsoleKey.Escape:
                            adminRef.Tell(AdminActor.Command.Close);
                            shallExit = true;
                            break;

                        case ConsoleKey.N:
                            adminRef.Tell(AdminActor.Command.NewChannel);
                            break;
                    }
                }
                Thread.Sleep(1);
            }

            // cleanup
            system.Terminate().Wait();
        }
    }
}
