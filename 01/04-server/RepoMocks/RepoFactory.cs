using _04_shared_domain.Data;

using System;
using System.Collections.Generic;
using System.Text;

namespace _04_server.RepoMocks
{
    public static class RepoFactory
    {
        public static IRepo<T> Create<T>(uint minResponse, uint maxResponse, Action<IRepo<T>, T, dynamic> decorator) 
            where T : class, IId, new()
        {
            return new Repo<T>(minResponse, maxResponse, decorator);
        }
    }
}
