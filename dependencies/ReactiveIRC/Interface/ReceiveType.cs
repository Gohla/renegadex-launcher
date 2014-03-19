namespace ReactiveIRC.Interface
{
    /// <summary>
    /// Values that represent the type of a received message.
    /// </summary>
    public enum ReceiveType
    {
        Unknown
      , Reply               // {INetwork}                       -> {Me}
      , Error               // {INetwork}                       -> {Me}
      , Ping                // {INetwork}                       -> {Me}
      , Echo                // {Me}                             -> {IMessageTarget}
      , Message	            // {IChannelUser, IUser, INetwork}  -> {IUser, IChannel}. 
      , Action              // {IChannelUser, IUser, INetwork}  -> {IUser, IChannel}. 
      , Notice              // {IChannelUser, IUser, INetwork}  -> {IUser, IChannel}. 
      , Invite              // {IChannelUser, IUser, INetwork}  -> {IChannel}.
      , Join                // {IChannelUser}                   -> {IChannel}. 
      , Part                // {IChannelUser}                   -> {IChannel}. 
      , Kick                // {IChannelUser, INetwork}         -> {IChannelUser}. 
      , Quit                // {IUser}                          -> {IUser, IChannel, INetwork}.
      , TopicChange         // {IChannelUser, IUser, INetwork}  -> {IChannel}. 
      , NickChange          // {IUser}                          -> {IUser, IChannel, INetwork}. 
      , UserModeChange      // {IUser, INetwork}                -> {Me?}
      , ChannelModeChange   // {IChannelUser, IUser, INetwork}  -> {IChannel}
    }
}
