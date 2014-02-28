using System;
using System.Collections.Generic;
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
        private Dictionary<String, Server> _servers = new Dictionary<String, Server>();

        public void Update()
        {
            IEnumerable<Server> servers = Request()
                .Select(_parser.Parse)
                .Where(v => v != null)
                ;
            ISet<Server> removedServers = new HashSet<Server>(_servers.Values);
            foreach(Server server in servers)
            {
                Server existingServer;
                if(_servers.TryGetValue(server.Address, out existingServer))
                {
                    existingServer.Update(server);
                    removedServers.Remove(server);
                }
                else
                {
                    _servers.Add(server.Address, server);
                    removedServers.Remove(server);
                }
            }

            foreach(Server server in removedServers)
            {
                _servers.Remove(server.Address);
            }
        }

        public IEnumerable<String> Request()
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
    }
}
