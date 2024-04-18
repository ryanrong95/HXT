using Needs.Ccs.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wl.IcgooBatch
{
    class Program
    {
        static void Main(string[] args)
        {          
            bool InsideProduct = Convert.ToBoolean(Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["InsideProduct"]));
            bool IcgooProduct = Convert.ToBoolean(Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["IcgooProduct"]));
            bool InsideOrder = Convert.ToBoolean(Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["InsideOrder"]));
            bool DutiablePricePost = Convert.ToBoolean(Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["DutiablePricePost"]));


            if (InsideOrder)
            {
                InsideOrderNoRequest();
            }

            if (IcgooProduct)
            {
                IcgooRequest();
            }

            if (InsideProduct)
            {
                InsidePreProducts();
            }

            if (DutiablePricePost)
            {
                Console.WriteLine("开始提交完税价格");
                PostDutiablePrice();
                Console.WriteLine("完税价格提交结束");
            }
            

            Environment.Exit(0);
        }

        /// <summary>
        /// 请求Icgoo待归类产品
        /// </summary>
        private static void IcgooRequest()
        {
            try
            {
                IcgooRequest request = new IcgooRequest();
                request.Process();
            }
            catch(Exception ex)
            {

            }
           
        }

        private static void InsideOrderNoRequest()
        {
            try
            {
                InsideOrderNoRequest request = new InsideOrderNoRequest();
                request.Process();
            }
            catch (Exception ex)
            {

            }
        }

        private static void InsidePreProducts()
        {
            try
            {
                getInsidePres pre = new getInsidePres();
                pre.GetProduct();
            }
            catch(Exception ex)
            {

            }
        }

        private static void PostDutiablePrice()
        {
            try
            {
                DutiablePricePost post = new DutiablePricePost();
                post.Run();
            }
            catch(Exception ex)
            {

            }
        }
    }
}
