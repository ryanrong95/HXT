using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public static class HttpCoding
    {
        static public string UrlEncoding(this string obj)
        {
           return UrlEncode(obj);
        }

        public static string UrlEncode(string str)
        {
            StringBuilder sb = new StringBuilder();
            byte[] byStr = System.Text.Encoding.UTF8.GetBytes(str); //默认是System.Text.Encoding.Default.GetBytes(str)
            for (int i = 0; i < byStr.Length; i++)
            {
                sb.Append(@"%" + Convert.ToString(byStr[i], 16));
            }

            return (sb.ToString());
        }
        
        static public string MFCUrlEncoding(this string obj)
        {
            Regex r = new Regex(@"^[a-zA-Z0-9]+$");
            if (r.Match(obj).Success)
            {
                return obj;
            }
            else
            {
                return UrlEncode(obj);
            }
        }
    }
}
