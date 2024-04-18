using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;

namespace Yahv.Web.Faces
{
    /// <summary>
    /// Yahv.Web.Faces 的 Globals
    /// </summary>
    public class Globals : HttpApplication
    {
        /// <summary>
        /// 默认构造器
        /// </summary>
        public Globals()
        {
            this.BeginRequest += Globals_BeginRequest_Request;
            this.PreSendRequestHeaders += Globals_PreSendRequestHeaders;
            //this.BeginRequest += Globals_BeginRequest_RewritePath;
        }

        //private void Globals_BeginRequest_RewritePath(object sender, EventArgs e)
        //{
        //    Match match = Regex.Match(Context.Request.Path, @"^/ViewPerson\-(\d+)\.aspx$");
        //    if (match.Success)
        //    {
        //        string id = match.Groups[1].Value;//拿到（\d+）就是id 的值 
        //        Context.RewritePath("/ViewPerson.aspx?id=" + id);
        //    }
        //}

        Regex sregex = new Regex(@"\s+", RegexOptions.Singleline);

        virtual protected void Globals_BeginRequest_Request(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            if (application == null
                || application.Request == null
                || application.Context == null
                || application.Context.Response == null)
            {
                return;
            }

            if (Request.HttpMethod == "GET" || Request.HttpMethod == "POST")
            {
                var dtf = Thread.CurrentThread.CurrentCulture.DateTimeFormat;
                FieldInfo field = typeof(DateTimeFormatInfo).GetField("m_isReadOnly", BindingFlags.NonPublic | BindingFlags.Instance);
                if (field == null)
                {
                    throw new ArgumentException("请重新找寻修改只读的参数", "m_isReadOnly");
                }
                field.SetValue(dtf, false);

                dtf.DateSeparator = "-";
                dtf.LongTimePattern = "HH:mm:ss";
                dtf.ShortDatePattern = "yyyy-MM-dd";


                //Thread.CurrentThread.CurrentCulture.NumberFormat
            }

            if (Request.HttpMethod == "POST" && Request.Form.Count > 0)
            {
                var form = Request.Form;
                FieldInfo field = typeof(NameObjectCollectionBase).GetField("_readOnly", BindingFlags.Instance | BindingFlags.NonPublic);
                if (field == null)
                {
                    throw new ArgumentException("请重新找寻修改只读的参数", "_readOnly");
                }
                field.SetValue(form, false);


                //string formvalue = "<script></script>";
                //HttpUtility.HtmlEncode(formvalue)

                for (int index = 0; index < form.AllKeys.Length; index++)
                {
                    string name = form.AllKeys[index];
                    if (name != null && (name.Contains("Html") || name.EndsWith("ForJson", StringComparison.OrdinalIgnoreCase)))
                    {
                        continue;
                    }

                    string value = form[index];
                    form[name] = sregex.Replace(value, " ").Replace("<", "&lt;").Replace(">", "&gt;");
                }
            }
        }

        private void Globals_PreSendRequestHeaders(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            if (application == null
                || application.Request == null
                || application.Context == null
                || application.Context.Response == null)
            {
                return;
            }

            NameValueCollection headers = application.Context.Response.Headers;

            if (headers == null)
            {
                return;
            }
#if !DEBUG
            headers.Remove("Server");
            headers.Remove("X-AspNet-Version");
            headers.Remove("X-Powered-By");
            /*
            HTTP/1.1 200 OK
            Cache-Control: private
            Content-Type: text/html; charset=utf-8
            Server: c07 testing
            X-Powered-By: ASP.NET
            Date: Thu, 20 Oct 2016 03:50:21 GMT
            Content-Length: 18310
            */

            //headers.Set("Project Leader", "cute chen");
            //headers.Set("Server", "ch gread");
#endif
        }
    }
}
