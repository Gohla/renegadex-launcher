using System;
using System.Diagnostics;
using System.IO;

namespace Gohla.Shared
{
    public static class Executable
    {
        public static String Name { get { return Process.GetCurrentProcess().MainModule.ModuleName; } }
        public static String FullFilename { get { return Process.GetCurrentProcess().MainModule.FileName; } }
        public static String FullPath { get { return Path.GetDirectoryName(FullFilename); } }

        public static String Combine(params String[] paths)
        {
            String[] allPaths = new String[paths.Length + 1];
            allPaths[0] = FullPath;
            paths.CopyTo(allPaths, 1);
            return Path.Combine(allPaths);
        }
    }
}
