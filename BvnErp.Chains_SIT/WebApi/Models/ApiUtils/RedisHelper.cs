using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models.ApiUtils
{
    /// <summary>
    /// 
    /// </summary>
    public class RedisHelper
    {
        static object locker = new object();
        static RedisHelper current;

        /// <summary>
        /// 
        /// </summary>
        public ConnectionMultiplexer connectionMultiplexer { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public static RedisHelper Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new RedisHelper();
                        }
                    }
                }

                return current;
            }
        }

        private RedisHelper() 
        {
            string RedisUrl = System.Configuration.ConfigurationManager.AppSettings["RedisUrl"];
            this.connectionMultiplexer = ConnectionMultiplexer.Connect(RedisUrl);
        }
    }
}