using System;

namespace Gohla.Shared
{
    public class FileType
    {
        public String Extension { get; private set; }
        public String ExtensionName { get; private set; }
        public String Name { get; private set; }

        public FileType(String extension, String name)
        {
            if(!extension.Contains("."))
                extension = "." + extension;

            Extension = extension;
            ExtensionName = extension.Remove(0, 1);
            Name = name;
        }

        public override int GetHashCode()
        {
            return Extension.GetHashCode();
        }

        public override String ToString()
        {
            return Extension;
        }
    }
}
