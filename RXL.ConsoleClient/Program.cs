using RXL.Core;
using System;

namespace RXL.ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerList serverList = new ServerList(null);
            serverList.Refresh();
            serverList.Refresh();

            foreach(Server server in serverList.Servers) {
                Console.WriteLine(server);
            }

            Console.ReadKey();
        }
    }
}
