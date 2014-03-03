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
        public Task<ProcessResults> Launch(String address, String name = "Harvester", String password = "")
        {
            return Launch(InstalledPath(), address, name, password);
        }

        public Task<ProcessResults> Launch(String path, String address, String name = "Harvester", String password = "")
        {
            if(path == null)
                throw new ArgumentNullException("path");

            StringBuilder argumentsBuilder = new StringBuilder();
            argumentsBuilder.Append(address);
            argumentsBuilder.Append("?name=");
            argumentsBuilder.Append(name);
            if(!password.Equals(String.Empty))
            {
                argumentsBuilder.Append("?Password=");
                argumentsBuilder.Append(password);
            }

            ProcessStartInfo start = new ProcessStartInfo();
            start.Arguments = argumentsBuilder.ToString();
            start.FileName = Path.Combine(path, @"Binaries\Win32\UDK.exe"); ;

            return ProcessEx.RunAsync(start);
        }

        public String InstalledPath()
        {
            return Registry.GetValue(@"HKEY_CURRENT_USER\Software\Totem Arts\Renegade X", "RenXPath", null) as String;
        }
    }
}
