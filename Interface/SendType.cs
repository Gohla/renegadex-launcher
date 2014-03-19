namespace ReactiveIRC.Interface
{
    /// <summary>
    /// Values that represent the type of a sent message.
    /// </summary>
    public enum SendType
    {
        Pass
      , Nick
      , User
      , Oper
      , Privmsg
      , Notice
      , Join
      , Part
      , Kick
      , Motd
      , Lusers
      , Version
      , Stats
      , Links
      , Time
      , Connect
      , Trace
      , Admin
      , Info
      , Servlist
      , Squery
      , List
      , Names
      , Topic
      , Mode
      , Service
      , Invite
      , Who
      , Whois
      , Whowas
      , Kill
      , Ping
      , Pong
      , Error
      , Away
      , Rehash
      , Die
      , Restart
      , Summon
      , Users
      , Wallops
      , Userhost
      , Ison
      , Quit
      , Squit
    }
}
