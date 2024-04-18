using Needs.Configuration;
using Needs.Configuration.Model;
using Needs.Interpreter.Domain;
using Needs.Interpreter.Model;
using Needs.Interpreter.Views;
using Needs.Settings;
using Needs.Settings.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Underly;
using Needs.Overall;
using System.Reflection;
using System.Collections.Concurrent;
using System.Threading;

namespace CnslApp
{
    class Program
    {
        static void Main(string[] args)
        {
            for (int index = 0; index < 1000000; index++)
            {
                var product = new
                {
                    name = "ad620",

                    count = 111,
                };

                dynamic odbco = new System.Dynamic.ExpandoObject();

                //odbco = new
                //{
                //    name = "ad620",
                //    count = 111,
                //};

                //odbco["manufacturer"] = "it";

                ////分步骤走

                //odbco["supplier"] = odbco["manufacturer"];
                odbco.supplier = "it";
                odbco.supplier1 = "it";
                odbco.supplier2 = "it";
                odbco.supplier3 = "it";
                odbco.supplier4 = "it";
            }

            //Console.WriteLine(product.ToString());


            new Class1().test();


            Console.WriteLine(12);

            new Thread(delegate ()
            {
                foreach (var item in Needs.Overall.Devlopers.Currents)
                {
                    Console.WriteLine(item.ID);
                }
            }).Start();

            using (new Needs.Linq.LinqContext())
            {
                using (Needs.Linq.LinqContext.Current)
                {
                    foreach (var item in Needs.Overall.Devlopers.Currents)
                    {
                        Console.WriteLine(item.ID);
                    }
                }
            }

            using (Needs.Linq.LinqContext.Current)
            {
                foreach (var item in Needs.Overall.Devlopers.Currents)
                {
                    Console.WriteLine(item.ID);
                }
            }

            using (new Needs.Linq.LinqContext())
            {
                foreach (var item in Needs.Overall.Devlopers.Currents)
                {
                    Console.WriteLine(item.ID);
                }
            }

            {
                foreach (var item in Needs.Overall.Devlopers.Currents)
                {
                    Console.WriteLine(item.ID);
                }

                Needs.Linq.LinqContext.Current.Dispose();
            }

            Console.WriteLine(111);

            //var kkk = Needs.Erp.ErpPlot.Current.ClientSolutions;

            ////var k123 = kkk.MyClient;


            //Assembly assembly = Assembly.LoadFile("");
            ////assembly.get


            ////var ikc = Needs.Erp.ErpPlot.Current.MyClients["0"];

            //for (int index = 0; index < 10; index++)
            //{
            //    var id = PKeySigner.Pick(PKeyType.Admin);
            //    Console.WriteLine(id);
            //}

            //foreach (var item in Enum.GetValues(typeof(Currency)).Cast<Currency>())
            //{
            //    Console.WriteLine(Legally.Current[item].ShortName);
            //    Console.WriteLine(Legally.Current[item].ShortSymbol);
            //    Console.WriteLine(Legally.Current[item].Symbol);
            //    Console.WriteLine();
            //}

            //foreach (var item in Enum.GetValues(typeof(District)).Cast<District>())
            //{
            //    Console.WriteLine(Legally.Current[item].ShortName);
            //    Console.WriteLine(Legally.Current[item].Name);
            //    Console.WriteLine(Legally.Current[item].Domain);
            //    Console.WriteLine(Legally.Current[item].ShowName);
            //    Console.WriteLine();
            //}
            //Console.WriteLine(SettingsManager<IErpSettings>.Current.Llaot);
            //Console.WriteLine(SettingsManager<IErpSettings>.Current.Tnsc);




            //Console.WriteLine(ConfigurationManager<ITesterConfig>.Current.Supremum);
            //Console.WriteLine(ConfigurationManager<ISystemConfig>.Current.SettingDbName);

            //#region Adapter

            //using (var current = Needs.Linq.Adapter<ITranslate, TranslateView>.Current)
            //{

            //    //可以真实利用上 iunique

            //    var k = current["asdf"];


            //    foreach (ITranslate item in current)
            //    {
            //        Console.WriteLine($"{item.ID}:{item.Name}");
            //    }
            //}

            //#endregion

            //#region Factory

            //using (var current = Needs.Linq.Factory<ITranslate, TranslateView>.Current)
            //{
            //    foreach (ITranslate item in current)
            //    {
            //        Console.WriteLine($"{item.ID}:{item.Name}");
            //    }
            //}

            //using (var current = Needs.Linq.Factory<IMapTester, MapsTesterView>.Current)
            //{
            //    foreach (ITranslate item in current)
            //    {
            //        Console.WriteLine($"{item.ID}:{item.Name}");
            //    }
            //}

            //#endregion

            //#region other

            //using (var orders = new OrdersView())
            //{
            //    foreach (var item in orders)
            //    {
            //        Console.WriteLine($"{item.ID}:{item.Name}");
            //    }
            //}

            //using (var orders = new OrdersView())
            //{
            //    var linq = orders.Where(item => item.ID != "a");

            //    foreach (var item in linq)
            //    {
            //        Console.WriteLine($"{item.ID}:{item.Name}");
            //    }
            //}

            //#endregion

            Console.Read();
        }
    }
}
