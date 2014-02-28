using RXL.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace RXL.Core
{
    public class ServerList
    {
        private const String SERVERLIST_ADDRESS = "http://renegadexgs.appspot.com/browser_2.jsp?view=false";

        private readonly ServerListParser _parser = new ServerListParser();
        private readonly KeyedCollection<String, Server> _servers = new KeyedCollection<String, Server>();

        private readonly BackgroundWorker _refreshWorker = new BackgroundWorker();
        private readonly BackgroundWorker _pingWorker = new BackgroundWorker();

        public IObservableCollection<Server> Servers { get { return _servers; } }

        public event Action<IEnumerable<Server>> Refreshed;
        public event Action<IEnumerable<PingResult>> Pinged;

        public ServerList()
        {
            _refreshWorker.WorkerSupportsCancellation = true;
            _refreshWorker.DoWork += delegate(object obj, DoWorkEventArgs args)
            {
                args.Result = Request()
                    .Select(_parser.ParseServer)
                    .Where(v => v != null)
                    ; 
            };
            _refreshWorker.RunWorkerCompleted += delegate(object obj, RunWorkerCompletedEventArgs args)
            {
                ISet<Server> removedServers = new HashSet<Server>(_servers.Values);
                foreach(Server server in args.Result as IEnumerable<Server>)
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

                Ping();

                if(Refreshed != null)
                    Refreshed.Invoke(_servers);
            };

            _pingWorker.WorkerSupportsCancellation = true;
            _pingWorker.DoWork += delegate(object obj, DoWorkEventArgs args)
            {
                args.Result = _servers
                    .AsParallel()
                    .WithDegreeOfParallelism(32)
                    .Select(s => new PingResult(s, new Ping().Send(String.Concat(s.Address.TakeWhile(c => c != ':')), 500)))
                    .ToArray()
                    ;
            };
            _pingWorker.RunWorkerCompleted += delegate(object obj, RunWorkerCompletedEventArgs args)
            {
                IEnumerable<PingResult> results = args.Result as IEnumerable<PingResult>;
                foreach(PingResult result in results)
                {
                    if(result.Reply.Status == IPStatus.Success)
                        result.Server.Latency = result.Reply.RoundtripTime;
                    else
                        result.Server.Latency = -1;
                }

                if(Pinged != null)
                    Pinged.Invoke(results);
            };
        }

        public void Refresh()
        {
            // TODO: Properly handle multiple requests.
            if(!_refreshWorker.IsBusy)
                _refreshWorker.RunWorkerAsync();
        }

        public void Ping()
        {
            // TODO: Properly handle multiple requests.
            if(!_pingWorker.IsBusy)
                _pingWorker.RunWorkerAsync();
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
