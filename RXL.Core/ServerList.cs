using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RXL.Core
{
    public class ServerList
    {
        private const String SERVERLIST_ADDRESS = "http://renegadexgs.appspot.com/browser_2.jsp?view=false";

        private ServerListParser _parser = new ServerListParser();
        private ObservableDictionary<String, Server> _servers = new ObservableDictionary<String, Server>();

        public ObservableDictionary<String, Server> Servers { get { return _servers; } }

        public void Refresh()
        {
            IEnumerable<Server> servers = Request()
                .Select(_parser.Parse)
                .Where(v => v != null)
                ;
            ISet<Server> removedServers = new HashSet<Server>(_servers.Values);
            foreach(Server server in servers)
            {
                if(!Update(server))
                {
                    Add(server);
                }
            }

            foreach(Server server in removedServers)
            {
                Remove(server.Address);
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
            _servers.Add(server.Address, server);
        }

        private bool Update(Server server)
        {
            Server existingServer;
            if(_servers.TryGetValue(server.Address, out existingServer))
            {
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
