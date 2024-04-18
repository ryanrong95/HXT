using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Needs.FrontEnd
{

    class FedItem
    {
        public string RelativeUri { get; private set; }

        public string Html { get; private set; }

        public DateTime LastWriteTime { get; private set; }

        public FedItem(string relativeUri, string html, DateTime lastWriteTime)
        {
            this.RelativeUri = relativeUri;
            this.Html = html;
            this.LastWriteTime = lastWriteTime;
        }
    }

    class FedItems
    {
        public Encoding Encoding { get; set; }

        ConcurrentDictionary<string, FedItem> concurrent;

        FedItems()
        {
            this.Encoding = Encoding.UTF8;
            this.concurrent = new ConcurrentDictionary<string, FedItem>();
        }

        public FedItem this[string index]
        {
            get
            {
                var item = concurrent.GetOrAdd(index, this.CreateItem(index));

                string fileName = HttpContext.Current.Server.MapPath(index);
                FileInfo info = new FileInfo(fileName);

                if (item.LastWriteTime != info.LastWriteTime)
                {
                    lock (this)
                    {
                        info.Refresh();
                        if (item.LastWriteTime != info.LastWriteTime)
                        {
                            item = concurrent[index] = this.CreateItem(index);
                        }
                    }
                }

                return item;
            }
        }


        FedItem CreateItem(string relativeUri)
        {
            string html;
            string fileName = HttpContext.Current.Server.MapPath(relativeUri);
            FileInfo info = new FileInfo(fileName);

            if (info.Extension.EndsWith("shtml", StringComparison.OrdinalIgnoreCase))
            {
                Uri uri = new Uri(HttpContext.Current.Request.Url, relativeUri);
                using (WebClient client = new WebClient())
                {
                    client.Encoding = this.Encoding;
                    html = client.DownloadString(uri);
                }
            }
            else
            {
                using (StreamReader reader = new StreamReader(info.Open(FileMode.Open)))
                {
                    html = reader.ReadToEnd();
                }
            }

            return new FedItem(relativeUri, html, info.LastWriteTime);
        }


        static FedItems current;
        static object locker = new object();
        static public FedItems Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new FedItems();
                        }
                    }
                }

                return current;
            }
        }
    }
}
