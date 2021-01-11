using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

using Akka.Actor;

namespace _02_chatters
{
    enum Subject
    {
        Greeting = 0,
        Name,
        Age,
        Mood,
    }

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
            ActorSystem system = ActorSystem.Create("test-system");

            IActorRef samRef = system.ActorOf(ChatActor.GetProps(questions, answers), "lovely-sam");
            IActorRef betsieRef = system.ActorOf(ChatActor.GetProps(questions, answers), "shy-betsie");

            // run
            samRef.Tell("go", betsieRef);

            // wait for exit
            while (!Console.KeyAvailable && Console.ReadKey(true).Key != ConsoleKey.Escape)
            {
                Thread.Sleep(1);
            }

            // cleanup
            system.Terminate().Wait();
        }
    }

    class ChatMessage
    {
        public Subject Subject { get; }
        public string Message { get; }
        public bool IsAnswer { get; }

        public ChatMessage(Subject subject, string message, bool isAnswer)
        {
            this.Subject = subject;
            this.Message = message;
            this.IsAnswer = isAnswer;
        }
    }

    class ChatActor : ReceiveActor
    {
        private readonly Random random;
        private readonly IDictionary<Subject, string> questions;
        private readonly IDictionary<Subject, string[]> answers;

        public static Props GetProps(IDictionary<Subject, string> questions, IDictionary<Subject, string[]> answers)
            => Props.Create<ChatActor>(questions, answers);

        public ChatActor(IDictionary<Subject, string> questions, IDictionary<Subject, string[]> answers)
        {
            this.random = new Random();
            this.questions = questions;
            this.answers = answers;

            this.Become(() =>
            {
                this.Receive<string>(this.StartChat, x => x == "go");
                this.Receive<ChatMessage>(this.HandleQuestion);
            });
        }

        private void Narrate(string message)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"{Context.Self.Path.Name}> {message}");
        }

        /// <summary>
        /// Starts conversation to other with greeting
        /// </summary>
        private void StartChat(string message)
        {
            var greeting = $"Hello {Context.Sender.Path.Name}, nice to meet ya!. I'm {Context.Self.Path.Name}";

            this.Narrate(greeting);

            Context.Sender.Tell(new ChatMessage(Subject.Greeting, greeting, isAnswer: false));
        }

        private void HandleQuestion(ChatMessage message)
        {
            if (message.IsAnswer)
            {
                // this holds data from other chat party. not used in our sample, normally would be displayed on screen of receiver?
                return;
            }

            // imitate some writing time.
            Thread.Sleep(500 + this.random.Next(500));

            if (this.InMoodForLongerTalk())
            {
                ChatMessage answer = this.RollAnswer(message.Subject);
                this.Narrate(answer.Message);

                ChatMessage question = this.RollQuestion();
                this.Narrate($"{Context.Sender.Path.Name}, {question.Message}");
                Context.Sender.Tell(question);
            }
            else
            {
                ChatMessage byebye = new ChatMessage(Subject.Mood, $"It was nice talk, {Context.Sender.Path.Name} but I gonna run. Bye!", isAnswer: true);
                this.Narrate(byebye.Message);
                Context.Sender.Tell(byebye);

                Context.Self.Tell(PoisonPill.Instance);
                return;
            }
        }

        private bool InMoodForLongerTalk()
            => this.random.Next(10) >= 2;

        /// <summary>
        /// Produces some answer to given question
        /// </summary>
        private ChatMessage RollAnswer(Subject subject)
        {
            string answer;

            if (subject == Subject.Greeting)
            {
                answer = $"Hi {Context.Sender.Path.Name}. For me it's also a pleasure.";
            }
            else
            {
                var pool = this.answers[subject];
                answer = pool[this.random.Next(pool.Length)];
                if (string.IsNullOrEmpty(answer) && subject == Subject.Name)
                {
                    answer = Context.Self.Path.Name;
                }
            }

            return new ChatMessage(subject, answer, isAnswer: true);
        }

        /// <summary>
        /// Produces new random question message
        /// </summary>
        private ChatMessage RollQuestion()
        {
            Subject subject = (Subject)(1 + this.random.Next(Enum.GetValues(typeof(Subject)).Length - 1));
            string message = this.questions[subject];

            return new ChatMessage(subject, message, isAnswer: false);
        }
    }
}
