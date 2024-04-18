using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Wl.IcgooMQ
{
    class Program
    {
        public static string UserName;
        public static string Password;
        public static string HostName;
        public static string Port;
        public static string VirtualHost;

        static void Main(string[] args)
        {

            UserName = System.Configuration.ConfigurationManager.AppSettings["UserName"];
            Password = System.Configuration.ConfigurationManager.AppSettings["Password"];
            HostName = System.Configuration.ConfigurationManager.AppSettings["HostName"];
            Port = System.Configuration.ConfigurationManager.AppSettings["Port"];
            VirtualHost = System.Configuration.ConfigurationManager.AppSettings["VirtualHost"];

            bool InsideOrder = Convert.ToBoolean(Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["InsideOrder"]));
            bool IcgooOrder = Convert.ToBoolean(Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["IcgooOrder"]));

            //InsideOld();
            //UpdateOld();
            UpdateEditionOne();

            //if (InsideOrder)
            //{
            //    ThreadStart threadStart = new ThreadStart(Inside);
            //    Thread thread = new Thread(threadStart);
            //    thread.Start();
            //}

            //if (IcgooOrder)
            //{
            //    ThreadStart threadicgoo = new ThreadStart(Icgoo);
            //    Thread threadIcgoo = new Thread(threadicgoo);
            //    threadIcgoo.Start();
            //}


            //try
            //{
            //    InsideCreateOrder order = new InsideCreateOrder();
            //    order.Create("6BFAE3A20A4546E2B8DF50B112804269");
            //}
            //catch (Exception ex)
            //{
            //    string test = ex.ToString();
            //}

            //try
            //{
            //    IcgooCreateOrderSpeed order = new IcgooCreateOrderSpeed();
            //    order.Create("DD9ADDB6348B45809DEE116233CD56F8");
            //}
            //catch (Exception ex)
            //{

            //}
        }

        private static void Icgoo()
        {
            while (true)
            {              
                Receive receive = new Receive(UserName, Password, HostName, Convert.ToInt16(Port), VirtualHost);
                receive.IcgooConsume();
            }           
        }

        private static void Inside()
        {
            while (true)
            {               
                Receive receive = new Receive(UserName, Password, HostName, Convert.ToInt16(Port), VirtualHost);
                receive.InsideConsume();
            }               
        }

        private static void InsideOld()
        {
            InsideCreateOrder order = new InsideCreateOrder();
            order.CreateOld();
        }

        public static void UpdateOld()
        {
            InsideCreateOrder order = new InsideCreateOrder();
            order.UpdateOld();
        }

        public static void UpdateEditionOne()
        {
            InsideCreateOrder order = new InsideCreateOrder();
            order.UpdateEditionOne();
        }

    }
}
