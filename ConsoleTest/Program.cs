using System;
using System.Threading;
using NLog;
using ReactiveIRC.Interface;

namespace ReactiveIRC.ConsoleTest
{
    public class Program
    {
        protected static readonly Logger _logger = NLog.LogManager.GetLogger("Program");

        public static void Main(String[] args)
        {
            bool run = true;
            IClient client = new ReactiveIRC.Client.Client();
            IClientConnection connection = client.CreateClientConnection(args[0], Convert.ToUInt16(args[1]), null);
            connection.ReceivedMessages.Subscribe(PrintMessage);
            connection.Connect().Subscribe(
                _ => {},
                e => _logger.ErrorException("Error connecting.", e),
                () => connection.Login(args[2], args[3], args[4], args[5]).Subscribe()
            );

            Console.CancelKeyPress += delegate
            {
                connection.Dispose();
                run = false;
            };

            while(run)
            {
                Thread.Sleep(50);
            }
        }

        private static void PrintMessage(IReceiveMessage message)
        {
            _logger.Info(message.Type.ToString() + "," + message.ReplyType.ToString() + " - " + 
                message.Sender.ToString() + " :: " + message.Receiver + " :: " + message.Contents);
        }
    }
}
