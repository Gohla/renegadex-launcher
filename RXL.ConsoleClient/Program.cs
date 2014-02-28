using RXL.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RXL.ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerList serverList = new ServerList();
            serverList.Update();

            Console.ReadKey();
        }
    }
}
