using System;
using System.IO;
using System.Net;
using System.Text;

namespace Gohla.Shared
{
    public static class HTTPUtility
    {
        // Source: http://blogs.msdn.com/b/feroze_daud/archive/2004/03/30/104440.aspx
        public static String Decode(WebResponse response, Stream stream)
        {
            String charset = null;
            String contentType = response.Headers["content-type"];
            if(contentType != null)
            {
                int index = contentType.IndexOf("charset=");
                if(index != -1)
                {
                    charset = contentType.Substring(index + 8);
                }
            }

            MemoryStream data = new MemoryStream();
            byte[] buffer = new byte[1024];
            int read = stream.Read(buffer, 0, buffer.Length);
            while(read > 0)
            {
                data.Write(buffer, 0, read);
                read = stream.Read(buffer, 0, buffer.Length);
            }
            stream.Close();

            Encoding encoding = Encoding.UTF8;
            try
            {
                if(charset != null)
                    encoding = Encoding.GetEncoding(charset);
            }
            catch { }

            data.Seek(0, SeekOrigin.Begin);
            StreamReader streamReader = new StreamReader(data, encoding);
            return streamReader.ReadToEnd();
        }
    }
}
