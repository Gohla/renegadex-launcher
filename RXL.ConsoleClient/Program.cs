using RXL.Core;
using System;
using System.Linq;

namespace RXL.ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerList serverList = new ServerList();

            var servers = serverList.Refresh();
            servers.Wait();
            foreach(Server server in servers.Result)
            {
                Console.WriteLine(server.Name);
            }

            var pings = serverList.Ping(servers.Result.Select(s => s.Address));
            pings.Wait();
            foreach(PingResult result in pings.Result)
                Console.WriteLine("{0}: {1}", result.Reply.Status, result.Address);

            Console.ReadKey();
        }
    }
}
