using System.Windows;
using BugSense;
using BugSense.Model;

namespace RXL.WPFClient
{
    public partial class App : Application
    {
        public App()
        {
            // Initialize BugSense
            BugSenseHandler.Instance.InitAndStartSession(new ExceptionManager(Current), "w8cd8c8f");
            // Other Windows Store specific operations
        }
    }
}
