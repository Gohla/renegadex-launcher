using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Text;

namespace RXL.WPFClient.Utils
{
    public class JSONStorage
    {
        public void Write<T>(T data, String filename)
        {
            String json = JsonConvert.SerializeObject(data);
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForAssembly())
            {
                using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(filename, FileMode.Create, FileAccess.Write, store))
                {
                    byte[] jsonData = GetBytes(json);
                    stream.Write(jsonData, 0, jsonData.Length);
                }
            }
        }

        public T Read<T>(String filename) where T : new()
        {
            try
            {
                using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForAssembly())
                {
                    using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(filename, FileMode.Open, FileAccess.Read, store))
                    {
                        StringBuilder sb = new StringBuilder();
                        byte[] buffer = new byte[0x1000];
                        int numRead;
                        while ((numRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            String text = GetString(buffer, numRead);
                            sb.Append(text);
                        }

                        return JsonConvert.DeserializeObject<T>(sb.ToString());
                    }
                }
            }
            catch (Exception)
            {
                // TODO: better error handling.
                return new T();
            }
        }

        public bool Exists(String filename)
        {
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForAssembly())
            {
                return store.FileExists(filename);
            }
        }

        private static byte[] GetBytes(String str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        private static String GetString(byte[] bytes, int count)
        {
            char[] chars = new char[count / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, count);
            return new String(chars);
        }
    }
}
