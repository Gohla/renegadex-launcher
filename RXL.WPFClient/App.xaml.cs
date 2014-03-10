using BugSense;
using BugSense.Model;
using System.Windows;

namespace RXL.WPFClient
{
    public partial class App : Application
    {
        public App()
        {
            BugSenseHandler.Instance.InitAndStartSession(new ExceptionManager(Current), "w8cd8c8f");
        }
    }
}
