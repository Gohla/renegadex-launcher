using System;
using System.Linq;
using System.Net.Sockets;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using NLog;

namespace ReactiveIRC.Client
{
    public class RawClientConnection : IDisposable
    {
        protected static readonly Logger _logger = NLog.LogManager.GetLogger("RawClientConnection");

        private TcpClient _socket = new TcpClient();
        private Subject<String> _rawMessages = new Subject<String>();
        private IDisposable _rawMessagesSubscription;

        public String Address { get; private set; }
        public ushort Port { get; private set; }
        public IObservable<String> RawMessages { get { return _rawMessages; } }

        public RawClientConnection(String address, ushort port)
        {
            if(String.IsNullOrEmpty(address))
                throw new ArgumentNullException("address");

            Address = address;
            Port = port;
        }

        public void Dispose()
        {
            if(_socket.Connected)
                Disconnected();
        }

        public IObservable<Unit> Connect()
        {
            if(_socket.Connected)
                return Observable.Throw<Unit>(new InvalidOperationException("Already connected or connecting."));

            return _socket.Client
                .ConnectObservable(Address, Port)
                .Do(_ => Connected(), ConnectedError)
                ;
        }

        public IObservable<Unit> Disconnect()
        {
            if(!_socket.Connected)
                return Observable.Throw<Unit>(new InvalidOperationException("No connection."));

            return _socket.Client
                .DisconnectObservable(true)
                .Do(_ => Disconnected(), DisconnectedError)
                ;
        }

        protected IObservable<Unit> WriteRaw(String message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message + Environment.NewLine);
            return _socket.Client
                .SendObservable(data, 0, data.Length, SocketFlags.None)
                .Select(_ => Unit.Default)
                ;
        }

        protected IObservable<Unit> WriteRaw(params String[] messages)
        {
            // TODO: Send all at once.
            return Observable.Merge(messages.Select(m => WriteRaw(m)));
        }

        private void Connected()
        {
            _rawMessagesSubscription = _socket.Client.ReceiveUntilCompleted(SocketFlags.None)
                .Do(_ => { }, e => _logger.ErrorException("Error receiving: ", e), 
                    () => _logger.Error("Receive observable completed."))
                .SelectMany(x => Encoding.UTF8.GetString(x).ToCharArray())
                .Scan(String.Empty, (a, b) => (a.EndsWith("\r\n") ? "" : a) + b)
                .Where(x => x.EndsWith("\r\n"))
                .Select(b => String.Join("", b).Replace("\r\n", ""))
                .Subscribe(_rawMessages)
                ;
        }

        private void ConnectedError(Exception e)
        {
            _socket.Close();
        }

        private void Disconnected()
        {
            _socket.Close();
            _rawMessagesSubscription.Dispose();
            _rawMessagesSubscription = null;
        }

        private void DisconnectedError(Exception e)
        {
            Disconnected();
        }
    }
}
