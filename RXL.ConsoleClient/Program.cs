using ReactiveIRC.Client;
using ReactiveIRC.Interface;
using RXL.Core;
using System;
using System.Linq;
using System.Reactive.Linq;

namespace RXL.ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerList serverList = new ServerList();

            var servers = serverList.Refresh();
            servers.Wait();
            foreach (Server server in servers.Result)
            {
                Console.WriteLine(server.Name);
            }

            var pings = serverList.Ping(servers.Result.Select(s => s.Address));
            pings.Wait();
            foreach (PingResult result in pings.Result)
            {
                if (result == null)
                    continue;
                Console.WriteLine("{0}: {1}", result.Reply.Status, result.Address);
            }

            IClient ircClient = new Client();
            IClientConnection connection = ircClient.CreateClientConnection("irc.freenode.net", 6666, null);
            connection.Connect().Wait();
            connection.Login("Harvester", "Harvester", "Harvester").Wait();
            connection.Join("#tz");

            Console.ReadKey();
        }
    }
}
