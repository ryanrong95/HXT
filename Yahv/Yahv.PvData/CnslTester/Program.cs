using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CnslTester
{
    class Program
    {
        static void Main(string[] args)
        {

            string conntext = "Data Source=172.30.10.51,6522;Initial Catalog=PvData;Persist Security Info=True;User ID=u_v0;Password=V6e1W9MiA84t6FJ05TXCU9uRY9THg3EpT60v9X1qwMbLg895Oc";
            string sql = @"SELECT TOP 100 [ID]
      ,[PartNumber]
      ,[Manufacturer]
      ,[HSCode]
      ,[Name]
      ,[LegalUnit1]
      ,[LegalUnit2]
      ,[VATRate]
      ,[TariffRate]
      ,[ExciseTaxRate]
      ,[Elements]
      ,[SupervisionRequirements]
      ,[CIQC]
      ,[CIQCode]
      ,[TaxCode]
      ,[TaxName]
      ,[Ccc]
      ,[Embargo]
      ,[HkControl]
      ,[Coo]
      ,[CIQ]
      ,[CIQprice]
      ,[CreateDate]
      ,[OrderDate]
      ,[Summary]
      ,[Eccn]
      ,[AddedTariffRate]
  FROM [PvData].[dbo].[StandardPartnumbersForPlugin]";

            Thread[] threads = new Thread[50].Select((item, index) =>
            {
                return new Thread(() =>
                {
                    try
                    {
                        using (SqlConnection conn = new SqlConnection(conntext))
                        using (SqlCommand command = new SqlCommand(sql, conn))
                        using (SqlDataAdapter adpter = new SqlDataAdapter(command))
                        {
                            DataSet ds = new DataSet();
                            adpter.Fill(ds);

                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        //Console.WriteLine(Thread.CurrentThread.Name);
                    }
                })
                {
                    Name = "线程:" + index + "_时间" + DateTime.Now
                };
            }).ToArray();

            for (int index = 0; index < threads.Length; index++)
            {
                threads[index].Start();
            }

            //Stopwatch watch = new Stopwatch();
            //watch.Start();
            //for (int index = 0; index < threads.Length; index++)
            //{
            //    threads[index].Join();
            //}
            //watch.Stop();
            //Console.WriteLine(watch.ElapsedMilliseconds);

            Console.Read();
        }
    }
}
