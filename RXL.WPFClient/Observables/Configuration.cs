using RXL.Util;
using System;

namespace RXL.WPFClient.Observables
{
    public class Configuration : NotifyPropertyChangedBase
    {
        private String _name;
        private String _installLocation;

        public String Name
        {
            get { return _name; }
            set { SetField(ref _name, value, () => Name); }
        }

        public String InstallLocation
        {
            get { return _installLocation; }
            set { SetField(ref _installLocation, value, () => InstallLocation); }
        }

        public Configuration()
        {
            Name = String.Empty;
            InstallLocation = String.Empty;
        }
    }
}
