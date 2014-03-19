using System;
using System.Linq;
using System.Text;
using ReactiveIRC.Interface;

namespace ReactiveIRC.Protocol
{
    // http://tools.ietf.org/html/rfc2812
    public class MessageSender : IMessageSender
    {
        public IClient Client { get; private set; }
        public IClientConnection Connection { get; private set; }

        public MessageSender(IClientConnection connection)
        {
            Connection = connection;
            Client = connection.Client;
        }

        public ISendMessage Pass(String password)
        {
            return Client.CreateSendMessage(Connection, "PASS ", password, String.Empty, SendType.Pass);
        }

        public ISendMessage Nick(String nickname)
        {
            return Client.CreateSendMessage(Connection, "NICK ", nickname, String.Empty, SendType.Nick);
        }

        public ISendMessage User(String username, int usermode, String realname)
        {
            return Client.CreateSendMessage(Connection, "USER ", String.Concat(username, " ", usermode.ToString(), 
                " * :", realname), String.Empty, SendType.User);
        }

        public ISendMessage Oper(String name, String password)
        {
            return Client.CreateSendMessage(Connection, "OPER ", String.Concat(name, " ", password), String.Empty,
                SendType.Oper);
        }

        public ISendMessage Message(IMessageTarget receiver, String message)
        {
            return Client.CreateSendMessage(Connection, String.Concat("PRIVMSG ", receiver.Name, " :"), message,
                String.Empty, SendType.Privmsg, receiver);
        }

        public ISendMessage Action(IMessageTarget receiver, String action)
        {
            return Client.CreateSendMessage(Connection, String.Concat("PRIVMSG ", receiver.Name, " :"),
                String.Concat('\x001', "ACTION ", action, '\x001'), String.Empty, SendType.Privmsg, receiver);
        }

        public ISendMessage Notice(IMessageTarget receiver, String message)
        {
            return Client.CreateSendMessage(Connection, String.Concat("NOTICE ", receiver.Name, " :"), message,
                String.Empty, SendType.Notice, receiver);
        }

        public ISendMessage Join(IChannel channel)
        {
            return Client.CreateSendMessage(Connection, "JOIN ", channel.Name, String.Empty, SendType.Join, channel);
        }

        public ISendMessage Join(params IChannel[] channels)
        {
            String channellist = String.Join(",", channels.Select(c => c.Name));
            return Client.CreateSendMessage(Connection, "JOIN ", channellist, String.Empty, SendType.Join, channels);
        }

        public ISendMessage Join(IChannel channel, String key)
        {
            return Client.CreateSendMessage(Connection, "JOIN ", String.Concat(channel.Name, " ", key), String.Empty, 
                SendType.Join, channel);
        }

        public ISendMessage Join(IChannel[] channels, String[] keys)
        {
            String channellist = String.Join(",", channels.Select(c => c.Name));
            String keylist = String.Join(",", keys);
            return Client.CreateSendMessage(Connection, "JOIN ", String.Concat(channellist, " ", keylist), String.Empty,
                SendType.Join, channels);
        }

        public ISendMessage Part(IChannel channel)
        {
            return Client.CreateSendMessage(Connection, "PART ", channel.Name, String.Empty, SendType.Part, channel);
        }

        public ISendMessage Part(params IChannel[] channels)
        {
            String channellist = String.Join(",", channels.Select(c => c.Name));
            return Client.CreateSendMessage(Connection, "PART ", channellist, String.Empty, SendType.Part, channels);
        }

        public ISendMessage Part(IChannel channel, String partmessage)
        {
            return Client.CreateSendMessage(Connection, "PART ", String.Concat(channel.Name, " :", partmessage), 
                String.Empty, SendType.Part, channel);
        }

        public ISendMessage Part(String partmessage, params IChannel[] channels)
        {
            String channellist = String.Join(",", channels.Select(c => c.Name));
            return Client.CreateSendMessage(Connection, "PART ", String.Concat(channellist, " :", partmessage), 
                String.Empty, SendType.Part, channels);
        }

        public ISendMessage Kick(IChannelUser channelUser)
        {
            return Client.CreateSendMessage(Connection, "KICK ", String.Concat(channelUser.Channel.Name, " " +
                channelUser.User.Name), String.Empty, SendType.Kick, channelUser);
        }

        public ISendMessage Kick(IChannelUser channelUser, String comment)
        {
            return Client.CreateSendMessage(Connection, "KICK ", String.Concat(channelUser.Channel.Name, " "
               , channelUser.User.Name, " :", comment), String.Empty, SendType.Kick, channelUser);
        }

        /*public ISendMessage Kick(String[] channels, String nickname)
        {
            String channellist = String.Join(",", channels);
            return Client.CreateSendMessage(Connection, String.Concat("KICK ", channellist, " ", nickname), SendMessageType.Kick);
        }

        public ISendMessage Kick(String[] channels, String nickname, String comment)
        {
            String channellist = String.Join(",", channels);
            return Client.CreateSendMessage(Connection, String.Concat("KICK ", channellist, " ", nickname, " :", comment), 
                SendMessageType.Kick);
        }

        public ISendMessage Kick(String channel, String[] nicknames)
        {
            String nicknamelist = String.Join(",", nicknames);
            return Client.CreateSendMessage(Connection, String.Concat("KICK ", channel, " ", nicknamelist), SendMessageType.Kick);
        }

        public ISendMessage Kick(String channel, String[] nicknames, String comment)
        {
            String nicknamelist = String.Join(",", nicknames);
            return Client.CreateSendMessage(Connection, String.Concat("KICK ", channel, " ", nicknamelist, " :", comment), 
                SendMessageType.Kick);
        }*/

        public ISendMessage Kick(params IChannelUser[] channelUsers)
        {
            String channellist = String.Join(",", channelUsers.Select(c => c.Channel.Name));
            String nicknamelist = String.Join(",", channelUsers.Select(c => c.User.Name));
            return Client.CreateSendMessage(Connection, "KICK ", String.Concat(channellist, " ", nicknamelist), 
                String.Empty, SendType.Kick, channelUsers);
        }

        public ISendMessage Kick(String comment, params IChannelUser[] channelUsers)
        {
            String channellist = String.Join(",", channelUsers.Select(c => c.Channel.Name));
            String nicknamelist = String.Join(",", channelUsers.Select(c => c.User.Name));
            return Client.CreateSendMessage(Connection, "KICK ", String.Concat(channellist, " ", nicknamelist, " :", 
                comment), String.Empty, SendType.Kick, channelUsers);
        }

        public ISendMessage Motd()
        {
            return Client.CreateSendMessage(Connection, "MOTD", String.Empty, String.Empty, SendType.Motd);
        }

        public ISendMessage Motd(String target)
        {
            return Client.CreateSendMessage(Connection, "MOTD ", target, String.Empty, SendType.Motd);
        }

        public ISendMessage Lusers()
        {
            return Client.CreateSendMessage(Connection, "LUSERS", String.Empty, String.Empty, SendType.Lusers);
        }

        public ISendMessage Lusers(String mask)
        {
            return Client.CreateSendMessage(Connection, "LUSER ", mask, String.Empty, SendType.Lusers);
        }

        public ISendMessage Lusers(String mask, String target)
        {
            return Client.CreateSendMessage(Connection, "LUSER ", String.Concat(mask, " ", target), String.Empty, 
                SendType.Lusers);
        }

        public ISendMessage Version()
        {
            return Client.CreateSendMessage(Connection, "VERSION", String.Empty, String.Empty, SendType.Version);
        }

        public ISendMessage Version(String target)
        {
            return Client.CreateSendMessage(Connection, "VERSION ", target, String.Empty, SendType.Version);
        }

        public ISendMessage Stats()
        {
            return Client.CreateSendMessage(Connection, "STATS", String.Empty, String.Empty, SendType.Stats);
        }

        public ISendMessage Stats(String query)
        {
            return Client.CreateSendMessage(Connection, "STATS ", query, String.Empty, SendType.Stats);
        }

        public ISendMessage Stats(String query, String target)
        {
            return Client.CreateSendMessage(Connection, "STATS ", String.Concat(query, " ", target), String.Empty, 
                SendType.Stats);
        }

        public ISendMessage Links()
        {
            return Client.CreateSendMessage(Connection, "LINKS", String.Empty, String.Empty, SendType.Links);
        }

        public ISendMessage Links(String servermask)
        {
            return Client.CreateSendMessage(Connection, "LINKS ", servermask, String.Empty, SendType.Links);
        }

        public ISendMessage Links(String remoteserver, String servermask)
        {
            return Client.CreateSendMessage(Connection, "LINKS ", String.Concat(remoteserver, " ", servermask), 
                String.Empty, SendType.Links);
        }

        public ISendMessage Time()
        {
            return Client.CreateSendMessage(Connection, "TIME", String.Empty, String.Empty, SendType.Time);
        }

        public ISendMessage Time(String target)
        {
            return Client.CreateSendMessage(Connection, "TIME ", target, String.Empty, SendType.Time);
        }

        public ISendMessage Connect(String targetserver, String port)
        {
            return Client.CreateSendMessage(Connection, "CONNECT ", String.Concat(targetserver, " ", port), 
                String.Empty, SendType.Connect);
        }

        public ISendMessage Connect(String targetserver, String port, String remoteserver)
        {
            return Client.CreateSendMessage(Connection, "CONNECT ", String.Concat(targetserver, " ", port, " ", 
                remoteserver), String.Empty, SendType.Connect);
        }

        public ISendMessage Trace()
        {
            return Client.CreateSendMessage(Connection, "TRACE", String.Empty, String.Empty, SendType.Trace);
        }

        public ISendMessage Trace(String target)
        {
            return Client.CreateSendMessage(Connection, "TRACE ", target, String.Empty, SendType.Trace);
        }

        public ISendMessage Admin()
        {
            return Client.CreateSendMessage(Connection, "ADMIN", String.Empty, String.Empty, SendType.Admin);
        }

        public ISendMessage Admin(String target)
        {
            return Client.CreateSendMessage(Connection, "ADMIN ", target, String.Empty, SendType.Admin);
        }

        public ISendMessage Info()
        {
            return Client.CreateSendMessage(Connection, "INFO", String.Empty, String.Empty, SendType.Info);
        }

        public ISendMessage Info(String target)
        {
            return Client.CreateSendMessage(Connection, "INFO ", target, String.Empty, SendType.Info);
        }

        public ISendMessage Servlist()
        {
            return Client.CreateSendMessage(Connection, "SERVLIST", String.Empty, String.Empty, SendType.Servlist);
        }

        public ISendMessage Servlist(String mask)
        {
            return Client.CreateSendMessage(Connection, "SERVLIST ", mask, String.Empty, SendType.Servlist);
        }

        public ISendMessage Servlist(String mask, String type)
        {
            return Client.CreateSendMessage(Connection, "SERVLIST ", String.Concat(mask, " ", type), String.Empty, 
                SendType.Servlist);
        }

        public ISendMessage Squery(String servicename, String servicetext)
        {
            return Client.CreateSendMessage(Connection, "SQUERY ", String.Concat(servicename, " :", servicetext), 
                String.Empty, SendType.Squery);
        }

        public ISendMessage List()
        {
            return Client.CreateSendMessage(Connection, "LIST", String.Empty, String.Empty, SendType.List);
        }

        public ISendMessage List(IChannel channel)
        {
            return Client.CreateSendMessage(Connection, "LIST ", channel.Name, String.Empty, SendType.List, channel);
        }

        public ISendMessage List(IChannel[] channels)
        {
            String channellist = String.Join(",", channels.Select(c => c.Name));
            return Client.CreateSendMessage(Connection, "LIST ", channellist, String.Empty, SendType.List, channels);
        }

        public ISendMessage List(IChannel channel, String target)
        {
            return Client.CreateSendMessage(Connection, "LIST ", String.Concat(channel.Name, " ", target), String.Empty,
                SendType.List, channel);
        }

        public ISendMessage List(IChannel[] channels, String target)
        {
            String channellist = String.Join(",", channels.Select(c => c.Name));
            return Client.CreateSendMessage(Connection, "LIST ", String.Concat(channellist, " ", target), String.Empty,
                SendType.List, channels);
        }

        public ISendMessage Names()
        {
            return Client.CreateSendMessage(Connection, "NAMES", String.Empty, String.Empty, SendType.Names);
        }

        public ISendMessage Names(IChannel channel)
        {
            return Client.CreateSendMessage(Connection, "NAMES ", channel.Name, String.Empty, SendType.Names, channel);
        }

        public ISendMessage Names(IChannel[] channels)
        {
            String channellist = String.Join(",", channels.Select(c => c.Name));
            return Client.CreateSendMessage(Connection, "NAMES ", channellist, String.Empty, SendType.Names, channels);
        }

        public ISendMessage Names(IChannel channel, String target)
        {
            return Client.CreateSendMessage(Connection, "NAMES ", String.Concat(channel.Name, " ", target), String.Empty,
                SendType.Names, channel);
        }

        public ISendMessage Names(IChannel[] channels, String target)
        {
            String channellist = String.Join(",", channels.Select(c => c.Name));
            return Client.CreateSendMessage(Connection, "NAMES ", String.Concat(channellist, " ", target), String.Empty,
                SendType.Names, channels);
        }

        public ISendMessage Topic(IChannel channel)
        {
            return Client.CreateSendMessage(Connection, "TOPIC ", channel.Name, String.Empty, SendType.Topic, channel);
        }

        public ISendMessage Topic(IChannel channel, String newtopic)
        {
            return Client.CreateSendMessage(Connection, "TOPIC ", String.Concat(channel.Name, " :", newtopic), 
                String.Empty, SendType.Topic, channel);
        }

        public ISendMessage Mode(IMessageTarget target)
        {
            return Client.CreateSendMessage(Connection, "MODE ", target.Name, String.Empty, SendType.Mode, target);
        }

        public ISendMessage Mode(IMessageTarget target, String newmode)
        {
            return Client.CreateSendMessage(Connection, "MODE ", String.Concat(target.Name, " ", newmode), String.Empty,
                SendType.Mode, target);
        }

        public ISendMessage Mode(IMessageTarget target, String newmode, String newModeParameter)
        {
            return Client.CreateSendMessage(Connection, "MODE ", String.Concat(target.Name, " ", newmode, " ", 
                newModeParameter), String.Empty, SendType.Mode, target);
        }

        public ISendMessage Mode(IMessageTarget target, String[] newModes, String[] newModeParameters)
        {
            if(newModes == null)
            {
                throw new ArgumentNullException("newModes");
            }
            if(newModeParameters == null)
            {
                throw new ArgumentNullException("newModeParameters");
            }
            if(newModes.Length != newModeParameters.Length)
            {
                throw new ArgumentException("newModes and newModeParameters must have the same size.");
            }

            StringBuilder newMode = new StringBuilder(newModes.Length);
            StringBuilder newModeParameter = new StringBuilder();
            // as per RFC 3.2.3, maximum is 3 modes changes at once
            int maxModeChanges = 3;
            if(newModes.Length > maxModeChanges)
            {
                throw new ArgumentOutOfRangeException(
                    "newModes.Length",
                    newModes.Length,
                    String.Format("Mode change list is too large (> {0}).", maxModeChanges)
                );
            }

            for(int i = 0; i <= newModes.Length; i += maxModeChanges)
            {
                for(int j = 0; j < maxModeChanges; j++)
                {
                    if(i + j >= newModes.Length)
                    {
                        break;
                    }
                    newMode.Append(newModes[i + j]);
                }

                for(int j = 0; j < maxModeChanges; j++)
                {
                    if(i + j >= newModeParameters.Length)
                    {
                        break;
                    }
                    newModeParameter.Append(newModeParameters[i + j]);
                    newModeParameter.Append(" ");
                }
            }
            if(newModeParameter.Length > 0)
            {
                // remove trailing space
                newModeParameter.Length--;
                newMode.Append(" ");
                newMode.Append(newModeParameter.ToString());
            }

            return Mode(target, newMode.ToString());
        }

        public ISendMessage Service(String nickname, String distribution, String info)
        {
            return Client.CreateSendMessage(Connection, "SERVICE ", String.Concat(nickname, " * ", distribution, 
                " * * :", info), String.Empty, SendType.Service);
        }

        public ISendMessage Invite(IUser user, IChannel channel)
        {
            return Client.CreateSendMessage(Connection, "INVITE ", String.Concat(user.Name, " ", channel.Name), 
                String.Empty, SendType.Invite, user);
        }

        public ISendMessage Who()
        {
            return Client.CreateSendMessage(Connection, "WHO", String.Empty, String.Empty, SendType.Who);
        }

        public ISendMessage Who(String mask)
        {
            return Client.CreateSendMessage(Connection, "WHO ", mask, String.Empty, SendType.Who);
        }

        public ISendMessage Who(String mask, bool ircop)
        {
            if(ircop)
                return Client.CreateSendMessage(Connection, "WHO ", String.Concat(mask, " o"), String.Empty, 
                    SendType.Who);
            else
                return Client.CreateSendMessage(Connection, "WHO ", mask, String.Empty, SendType.Who);
        }

        public ISendMessage Whois(String mask)
        {
            return Client.CreateSendMessage(Connection, "WHOIS ", mask, String.Empty, SendType.Whois);
        }

        public ISendMessage Whois(String[] masks)
        {
            String masklist = String.Join(",", masks);
            return Client.CreateSendMessage(Connection, "WHOIS ", masklist, String.Empty, SendType.Whois);
        }

        public ISendMessage Whois(String target, String mask)
        {
            return Client.CreateSendMessage(Connection, "WHOIS ", String.Concat(target, " ", mask), String.Empty, 
                SendType.Whois);
        }

        public ISendMessage Whois(String target, String[] masks)
        {
            String masklist = String.Join(",", masks);
            return Client.CreateSendMessage(Connection, "WHOIS ", String.Concat(target, " ", masklist), String.Empty,
                SendType.Whois);
        }

        public ISendMessage Whowas(String nickname)
        {
            return Client.CreateSendMessage(Connection, "WHOWAS ", nickname, String.Empty, SendType.Whowas);
        }

        public ISendMessage Whowas(String[] nicknames)
        {
            String nicknamelist = String.Join(",", nicknames);
            return Client.CreateSendMessage(Connection, "WHOWAS ", nicknamelist, String.Empty, SendType.Whowas);
        }

        public ISendMessage Whowas(String nickname, String count)
        {
            return Client.CreateSendMessage(Connection, "WHOWAS ", String.Concat(nickname, " ", count, " "), 
                String.Empty, SendType.Whowas);
        }

        public ISendMessage Whowas(String[] nicknames, String count)
        {
            String nicknamelist = String.Join(",", nicknames);
            return Client.CreateSendMessage(Connection, "WHOWAS ", String.Concat(nicknamelist, " ", count, " "), 
                String.Empty, SendType.Whowas);
        }

        public ISendMessage Whowas(String nickname, String count, String target)
        {
            return Client.CreateSendMessage(Connection, "WHOWAS ", String.Concat(nickname, " ", count, " ", target), 
                String.Empty, SendType.Whowas);
        }

        public ISendMessage Whowas(String[] nicknames, String count, String target)
        {
            String nicknamelist = String.Join(",", nicknames);
            return Client.CreateSendMessage(Connection, "WHOWAS ", String.Concat(nicknamelist, " ", count, " ", target),
                String.Empty, SendType.Whowas);
        }

        public ISendMessage Kill(String nickname, String comment)
        {
            return Client.CreateSendMessage(Connection, "KILL ", String.Concat(nickname, " :", comment), String.Empty,
                SendType.Kill);
        }

        public ISendMessage Ping(String server)
        {
            return Client.CreateSendMessage(Connection, "PING ", server, String.Empty, SendType.Ping);
        }

        public ISendMessage Ping(String server, String server2)
        {
            return Client.CreateSendMessage(Connection, "PING ", String.Concat(server, " ", server2), String.Empty, 
                SendType.Ping);
        }

        public ISendMessage Pong(String server)
        {
            return Client.CreateSendMessage(Connection, "PONG ", server, String.Empty, SendType.Pong);
        }

        public ISendMessage Pong(String server, String server2)
        {
            return Client.CreateSendMessage(Connection, "PONG ", String.Concat(server, " ", server2), String.Empty, 
                SendType.Pong);
        }

        public ISendMessage Error(String errormessage)
        {
            return Client.CreateSendMessage(Connection, "ERROR :", errormessage, String.Empty, SendType.Error);
        }

        public ISendMessage Away()
        {
            return Client.CreateSendMessage(Connection, "AWAY", String.Empty, String.Empty, SendType.Away);
        }

        public ISendMessage Away(String awaytext)
        {
            return Client.CreateSendMessage(Connection, "AWAY :", awaytext, String.Empty, SendType.Away);
        }

        public ISendMessage Rehash()
        {
            return Client.CreateSendMessage(Connection, "REHASH", String.Empty, String.Empty, SendType.Rehash);
        }

        public ISendMessage Die()
        {
            return Client.CreateSendMessage(Connection, "DIE", String.Empty, String.Empty, SendType.Die);
        }

        public ISendMessage Restart()
        {
            return Client.CreateSendMessage(Connection, "RESTART", String.Empty, String.Empty, SendType.Restart);
        }

        public ISendMessage Summon(String user)
        {
            return Client.CreateSendMessage(Connection, "SUMMON ", user, String.Empty, SendType.Summon);
        }

        public ISendMessage Summon(String user, String target)
        {
            return Client.CreateSendMessage(Connection, "SUMMON ", String.Concat(user, " ", target), String.Empty, 
                SendType.Summon);
        }

        public ISendMessage Summon(String user, String target, String channel)
        {
            return Client.CreateSendMessage(Connection, "SUMMON ", String.Concat(user, " ", target, " ", channel), 
                String.Empty, SendType.Summon);
        }

        public ISendMessage Users()
        {
            return Client.CreateSendMessage(Connection, "USERS", String.Empty, String.Empty, SendType.Users);
        }

        public ISendMessage Users(String target)
        {
            return Client.CreateSendMessage(Connection, "USERS ", target, String.Empty, SendType.Users);
        }

        public ISendMessage Wallops(String wallopstext)
        {
            return Client.CreateSendMessage(Connection, "WALLOPS :", wallopstext, String.Empty, SendType.Wallops);
        }

        public ISendMessage Userhost(String nickname)
        {
            return Client.CreateSendMessage(Connection, "USERHOST ", nickname, String.Empty, SendType.Userhost);
        }

        public ISendMessage Userhost(String[] nicknames)
        {
            String nicknamelist = String.Join(" ", nicknames);
            return Client.CreateSendMessage(Connection, "USERHOST ", nicknamelist, String.Empty, SendType.Userhost);
        }

        public ISendMessage Ison(String nickname)
        {
            return Client.CreateSendMessage(Connection, "ISON ", nickname, String.Empty, SendType.Ison);
        }

        public ISendMessage Ison(String[] nicknames)
        {
            String nicknamelist = String.Join(" ", nicknames);
            return Client.CreateSendMessage(Connection, "ISON ", nicknamelist, String.Empty, SendType.Ison);
        }

        public ISendMessage Quit()
        {
            return Client.CreateSendMessage(Connection, "QUIT", String.Empty, String.Empty, SendType.Quit);
        }

        public ISendMessage Quit(String quitmessage)
        {
            return Client.CreateSendMessage(Connection, "QUIT :", quitmessage, String.Empty, SendType.Quit);
        }

        public ISendMessage Squit(String server, String comment)
        {
            return Client.CreateSendMessage(Connection, "SQUIT ", String.Concat(server, " :", comment), String.Empty,
                SendType.Squit);
        }
    }
}
