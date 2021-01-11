using System;
using System.Collections.Generic;
using System.Text;

namespace _03_supervised_chat
{
    enum Subject
    {
        Greeting = 0,
        Name,
        Age,
        Mood,
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
}
