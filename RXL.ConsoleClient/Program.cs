using RXL.Core;
using System;

namespace RXL.ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerList serverList = new ServerList();

            serverList.Refreshed += servers =>
            {
                foreach(Server server in servers)
                {
                    Console.WriteLine(server);
                }
            };

            serverList.Pinged += pingResults =>
            {
                foreach(PingResult result in pingResults)
                    Console.WriteLine("{0}: {1}", result.Reply.Status, result.Server.Address);
            };

            serverList.Refresh();

            Console.ReadKey();
        }
    }
}
