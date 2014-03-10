using Microsoft.Win32;
using RunProcessAsTask;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace RXL.Core
{
    public class Launcher
    {
        public Task<ProcessResults> Launch(String installLocation, String address, String name, String password = "")
        {
            if (installLocation == null)
                throw new ArgumentNullException("path");

            StringBuilder argumentsBuilder = new StringBuilder();
            argumentsBuilder.Append(address);
            argumentsBuilder.Append("?name=");
            argumentsBuilder.Append(name);
            if (!password.Equals(String.Empty))
            {
                argumentsBuilder.Append("?Password=");
                argumentsBuilder.Append(password);
            }

            ProcessStartInfo start = new ProcessStartInfo();
            start.Arguments = argumentsBuilder.ToString();
            start.FileName = CreatePathToExecutable(installLocation);

            return ProcessEx.RunAsync(start);
        }

        public String InstallLocationFromRegistry()
        {
            return Registry.GetValue(@"HKEY_CURRENT_USER\Software\Totem Arts\Renegade X", "RenXPath", null) as String;
        }

        public String DefaultInstallLocation()
        {
            return @"C:\Program Files (x86)\Renegade X";
        }

        public bool IsValidInstallLocation(String installLocation)
        {
            String loc = CreatePathToExecutable(installLocation);
            bool exists = File.Exists(loc);
            return exists;
        }

        private String CreatePathToExecutable(String installLocation)
        {
            return Path.Combine(installLocation, @"Binaries\Win32\UDK.exe");
        }
    }
}
