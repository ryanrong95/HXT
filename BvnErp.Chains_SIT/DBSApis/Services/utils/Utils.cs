using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBSApis.Services
{
    public class Utils
    {
        public static bool IsValidJson(string s)
        {
            try
            {
                var json = JContainer.Parse(s);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static Stream GenStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
        public static string GenStringFromStream(Stream s)
        {
            s.Seek(0, SeekOrigin.Begin);
            StreamReader reader = new StreamReader(s);
            return reader.ReadToEnd();
        }
        public static byte[] GenBytesFromStream(Stream s)
        {
            if (s is MemoryStream)
            {
                return ((MemoryStream)s).ToArray();
            }
            else
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    s.CopyTo(ms);
                    return ms.ToArray();
                }
            }
        }
    }
}