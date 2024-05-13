using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms.Services.Models_chenhan.___FIUOLIUHOHPUHsdfasdf
{
    public class Waybill
    {
        public string ID { get; set; }

        public string Code { get; set; }
    }
    public class Notcie
    {
        public string ID { get; set; }
        public string WaybillID { get; set; }
        public string InputID { get; set; }
        public string OutputID { get; set; }
        public string ProductID { get; set; }
        public string Source { get; set; }
        public string Type { get; set; }
    }

    public class Sorting
    {
        public string ID { get; set; }
        public string InputID { get; set; }
        public string NoticeID { get; set; }

        public string WaybillID { get; set; }
    }

    public class Input
    {
        public string ID { get; set; }
        public string Code { get; set; }
        public string ProductID { get; set; }
    }

    public class Product
    {
        public string ID { get; set; }
        public string Name { get; set; }
    }

    public class Storage
    {
        public string ID { get; set; }

        public string Type { get; set; }

        public string InputID { get; set; }
    }

    class MyMains
    {
        Waybill[] waybillsView = new Waybill[0];
        Notcie[] noticesView = new Notcie[0];
        Sorting[] sortingsView = new Sorting[0];
        Input[] inputsView = new Input[0];
        IEnumerable<Product> productsView = new Product[0];
        Storage[] storagesView = new Storage[0];

        public MyMains()
        {

        }


        /// <summary>
        /// 入库里列表
        /// </summary>
        public void fun0()
        {
            //入库的视图
            var items = from notice in noticesView
                            //join input in inputsView on notice.InputID equals input.ID
                            //join product in productsView on notice.ProductID equals product.ID
                        where notice.Type == "入库"
                        select notice;


            var linqs = from waybill in waybillsView
                        join item in items on waybill.ID equals item.WaybillID into temps
                        select new
                        {
                            waybill,
                            items = temps
                        };
        }

        /// <summary>
        /// 入库普通视图
        /// </summary>
        public void fun1()
        {

            //入库的视图
            var items = from notice in noticesView
                        join input in inputsView on notice.InputID equals input.ID
                        join product in productsView on notice.ProductID equals product.ID
                        select new
                        {
                            notice,
                            input,
                            product
                        };


            var linqs = from waybill in waybillsView
                        join item in items on waybill.ID equals item.notice.WaybillID into temps
                        select new
                        {
                            waybill,
                            items = temps
                        };

        }

        /// <summary>
        /// 入库分拣
        /// </summary>
        public void fun2()
        {
            //入库的视图
            var items = from input in inputsView
                        join sorting in sortingsView on input.ID equals sorting.InputID
                        join storage in storagesView on input.ID equals storage.InputID
                        join product in productsView on input.ProductID equals product.ID
                        join notice in noticesView on input.ID equals notice.InputID
                        select new
                        {
                            notice,
                            input,
                            product,
                            sorting,
                            kkk = new
                            {

                            }
                        };

            var linqs = from waybill in waybillsView
                        join item in items on waybill.ID equals item.sorting.WaybillID into temps
                        select new
                        {
                            waybill,
                            items = temps
                        };
        }


        /// <summary>
        /// 入库分拣
        /// </summary>
        public void fun2(Func<Product, bool> predicate)
        {
            var products = predicate == null ? productsView : productsView.Where(predicate);

            //入库的视图
            var items = from input in inputsView
                        join sorting in sortingsView on input.ID equals sorting.InputID
                        join storage in storagesView on input.ID equals storage.InputID
                        join product in products on input.ProductID equals product.ID
                        join notice in noticesView on input.ID equals notice.InputID
                        select new
                        {
                            notice,
                            input,
                            product,
                            sorting,
                            kkk = new
                            {

                            }
                        };




            var linqs = from waybill in waybillsView
                        join item in items on waybill.ID equals item.sorting.WaybillID into temps
                        select new
                        {
                            waybill,
                            items = temps
                        };
        }

        /// <summary>
        /// 入库分拣
        /// </summary>
        public void fun2(params object[] predicate)
        {
            var products = productsView;

            if (predicate.Any(item => item is Func<Product, bool>))
            {
                var kk = predicate.OfType<Func<Product, bool>>().First();
                products = productsView.Where(kk);
            }



            //入库的视图
            var items = from input in inputsView
                        join sorting in sortingsView on input.ID equals sorting.InputID
                        join storage in storagesView on input.ID equals storage.InputID
                        join product in products on input.ProductID equals product.ID
                        join notice in noticesView on input.ID equals notice.InputID
                        select new
                        {
                            notice,
                            input,
                            product,
                            sorting,
                            kkk = new
                            {

                            }
                        };

            var linqs = from waybill in waybillsView
                        join item in items on waybill.ID equals item.sorting.WaybillID into temps
                        select new
                        {
                            waybill,
                            items = temps
                        };
        }


        /// <summary>
        /// 入库分拣
        /// </summary>
        public void fun3(params object[] predicate)
        {
            var products = productsView;

            if (predicate.Any(item => item is Func<Product, bool>))
            {
                var kk = predicate.OfType<Func<Product, bool>>().First();
                products = productsView.Where(kk);
            }

            //入库的视图
            var items = from input in inputsView
                        join sorting in sortingsView on input.ID equals sorting.InputID
                        join storage in storagesView on input.ID equals storage.InputID
                        join product in products on input.ProductID equals product.ID
                        join notice in noticesView on input.ID equals notice.InputID
                        select new
                        {
                            notice,
                            input,
                            product,
                            sorting,
                            kkk = new
                            {

                            }
                        };

            var linq_ids = from waybill in waybillsView
                           join item in items on waybill.ID equals item.sorting.WaybillID
                           where item.product.Name.Contains(" ")
                           orderby waybill.ID descending
                           select waybill.ID;

            var ids = linq_ids.Take(500).ToArray();


            var linq = from waybill in waybillsView
                       where ids.Contains(waybill.ID)
                       select waybill;


            //face

            var aryy = items;


            json(new
            {

                client = new { },
                company = new { },

                left1 = items.Select(item => new
                {
                    product = new object(),
                    //ying = item.price ,
                    //shi=  item .notice
                    //.ID.Sum() 


                })


            });


        }


        void json(object o)
        {

        }

    }
}
