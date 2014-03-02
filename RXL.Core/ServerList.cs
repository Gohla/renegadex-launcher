using RXL.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace RXL.Core
{
    public class ServerList
    {
        private const String SERVERLIST_ADDRESS = "http://renegadexgs.appspot.com/browser_2.jsp?view=false";
        private readonly ServerListParser _parser = new ServerListParser();

        public async Task<IEnumerable<Server>> Refresh()
        {
            IEnumerable<String> lines = await Request();

            return lines
                .Select(_parser.ParseServer)
                .Where(v => v != null)
                ;
        }

        public async Task<PingResult[]> Ping(IEnumerable<String> addresses)
        {
            return await Task.WhenAll(addresses.Select(a => PingOne(a)));
        }

        public async Task<PingResult> PingOne(String address)
        {
            String hostname = String.Concat(address.TakeWhile(c => c != ':'));
            Ping pinger = new Ping();
            PingReply reply = await pinger.SendPingAsync(hostname, 500);
            return new PingResult(address, reply);
        }

        private async Task<IEnumerable<String>> Request()
        {
            HttpWebRequest request = WebRequest.Create(SERVERLIST_ADDRESS) as HttpWebRequest;
            HttpWebResponse response = await request.GetResponseAsync() as HttpWebResponse;
            Encoding responseEncoding = Encoding.GetEncoding(response.CharacterSet);
            using(StreamReader reader = new StreamReader(response.GetResponseStream(), responseEncoding))
            {
                return reader.ReadToEnd().Split(new String[] { "\r\n", "\n" }, StringSplitOptions.None);
            }
        }
    }
}
