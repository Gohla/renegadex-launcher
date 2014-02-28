using RXL.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;

namespace RXL.Core
{
    public class ServerList
    {
        private const String SERVERLIST_ADDRESS = "http://renegadexgs.appspot.com/browser_2.jsp?view=false";

        private ServerListParser _parser = new ServerListParser();
        private KeyedCollection<String, Server> _servers = new KeyedCollection<String, Server>();

        public IObservableCollection<Server> Servers { get { return _servers; } }

        public void Refresh()
        {
            IEnumerable<Server> servers = Request()
                .Select(_parser.ParseServer)
                .Where(v => v != null)
                ;
            ISet<Server> removedServers = new HashSet<Server>(_servers.Values);
            foreach(Server server in servers)
            {
                if(!Update(server))
                {
                    Add(server);
                }
                removedServers.Remove(server);
            }

            foreach(Server server in removedServers)
            {
                Remove(server.Address);
            }
        }

        public void Ping()
        {
            Ping pinger = new Ping();
            foreach(Server server in _servers)
            {
                Console.WriteLine("Ping!");
                PingReply reply = pinger.Send(String.Concat(server.Address.TakeWhile(c => c != ':')), 1000);
                if(reply.Status == IPStatus.Success)
                {
                    server.Latency = reply.RoundtripTime;
                    Console.WriteLine("Pong! " + reply.RoundtripTime);
                }
                else
                {
                    Console.WriteLine("Timeout!");
                }
            }
        }

        private IEnumerable<String> Request()
        {
            HttpWebRequest request = WebRequest.Create(SERVERLIST_ADDRESS) as HttpWebRequest;
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            Encoding responseEncoding = Encoding.GetEncoding(response.CharacterSet);
            using(StreamReader reader = new StreamReader(response.GetResponseStream(), responseEncoding))
            {
                while(!reader.EndOfStream)
                    yield return reader.ReadLine();
            }
        }

        private void Add(Server server)
        {
            _servers.Add(server);
        }

        private bool Update(Server server)
        {
            if(_servers.Contains(server.Address))
            {
                Server existingServer = _servers[server.Address];
                existingServer.Update(server);
                return true;
            }
            return false;
        }

        private void Remove(String address)
        {
            _servers.Remove(address);
        }
    }
}
