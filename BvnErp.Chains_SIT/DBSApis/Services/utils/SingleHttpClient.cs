using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace DBSApis.Services
{
    public  class SingleHttpClient
    {
        static object locker = new object();
        static SingleHttpClient current;

        public HttpClient httpClient;

        private SingleHttpClient()
        {
            this.httpClient = new HttpClient();
        }

        public static SingleHttpClient Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new SingleHttpClient();
                        }
                    }
                }
                return current;
            }
        }
    }
}