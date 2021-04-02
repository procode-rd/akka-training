using System;

using Topshelf;

namespace _04_server
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory
                .New(
                    hc =>
                    {
                        const string name = "04-akka-server";

                        hc.SetInstanceName(name);
                        hc.SetServiceName(name);
                        hc.EnableShutdown();
                        hc.UnhandledExceptionPolicy = Topshelf.Runtime.UnhandledExceptionPolicyCode.LogErrorOnly;

                        hc.Service(
                            hs => new ApiSystem(name));
                    })
                .Run();
        }
    }
}
