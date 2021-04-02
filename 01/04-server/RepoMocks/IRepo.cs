using _04_shared_domain.Data;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;

namespace _04_server.RepoMocks
{
    public interface IRepo<T>
        where T : IId
    {
        public Random Rng { get; }
        T Get(dynamic context);
        IEnumerable<T> GetMany(int howMany, dynamic context);
    }

    public class Repo<T> : IRepo<T>
        where T : class, IId, new()
    {
        private static long uberId = 1;

        private readonly int minResponse;
        private readonly int maxResponse;
        private readonly Action<IRepo<T>, T, dynamic> decorator;

        public Random Rng => this.Throttler;
        private Random Throttler { get; }

        public Repo(uint minResponse, uint maxResponse, Action<IRepo<T>, T, dynamic> decorator)
        {
            this.minResponse = (int)minResponse;
            this.maxResponse = (int)maxResponse;
            this.decorator = decorator;

            this.Throttler = new Random();
        }

        public T Get(dynamic context)
        {
            Thread.Sleep(this.minResponse + this.Throttler.Next(this.maxResponse - this.minResponse));

            var result = new T()
            {
                Id = Interlocked.Increment(ref uberId),
            };
            this.decorator?.Invoke(this, result, context);
            return result;
        }

        public IEnumerable<T> GetMany(int howMany, dynamic context)
        {
            for (int i = 0; i < howMany; i++)
            {
                yield return this.Get(context);
            }
        }
    }
}
