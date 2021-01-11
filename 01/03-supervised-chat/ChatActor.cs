using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using Akka.Actor;

namespace _03_supervised_chat
{
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
