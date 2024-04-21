using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CnsleApp
{


    public class Product
    {
        public File Files { get; set; }
    }

    public class File
    {
        public string ID { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {

            //var date = dtDateTime.AddSeconds(1587704507747).ToLocalTime();
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = 1587706170235 * 10000;
            TimeSpan toNow = new TimeSpan(lTime);
            DateTime dtResult = dtStart.Add(toNow); //得到转换后的时间
            //System.DateTime date = new DateTime(new DateTime(1970, 1, 1, 0, 0, 0, 0).Ticks + 1587706145063 * 10000);
            //621355968000000000
            Console.WriteLine(dtResult.ToString("yyyy年MM月dd日 HH:mm:ss"));


            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                string productID = "7386198";
                string sql = @"SELECT * FROM [PSL].[dbo].[ProductFiles] where ProductID={0}";
                var ienums = repository.Query<File>(sql, productID);

                var ienums_product = repository.Query<Product>(sql, productID);

            }

        }
    }
}
