using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;
using Yahv.Utils.Serializers;

namespace Yahv.PvOms.WebApp.Test
{
    public partial class PvDataApiTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            using (var r = new PvWsOrderReponsitory())
            {
                var orders = r.ReadTable<Layers.Data.Sqls.PvWsOrder.Orders>();
                var orderItems = r.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItems>();
                var linq = from entity in orders
                            join item in orderItems on entity.ID equals item.OrderID into items
                            select new
                            {
                                entity.ID,
                                entity.ClientID,
                                entity.InvoiceID,
                                Items = items.ToArray()
                            };
                var str = linq.ToString();
                var arr = linq.ToArray();
            }
            //TestGetAutoClassified();
            //TestAutoPreClassify();
            //TestQueryClassfiy();
        }

        void TestGetAutoClassified()
        {
            //var apiurl = "http://hv.erp.b1b.com/PvDataApi/Classify/GetAutoClassified";
            var apiurl = "http://erp80.foric.b1b.cn/PvDataApi/Classify/GetAutoClassified";
            var result = Yahv.Utils.Http.ApiHelper.Current.JPost<JSingle<dynamic>>(apiurl, new
            {
                PartNumber = "C8051F350-GQR",
                Manufacturer = "silicon",
                UnitPrice = 1.4,
                Origin = "USA",
            });
            var data = result.data;
            bool embargo = data.Embargo;
        }

        void TestAutoPreClassify()
        {
            //PreProductCategories表的ID
            var mainIDs = new List<string>() { "0008512FB52E48A3AD7F1F750C243FE5", "000A7566A3A74600B1813559B39FEC8E" };

            var apiurl = "http://apidev.for-ic.net/api/PreClassify/AutoClassify";
            var result = Yahv.Utils.Http.ApiHelper.Current.JPost<JMessage>(apiurl, new
            {
                MainIDs = mainIDs
            });
            var data = result.data;
        }

        void TestQueryClassfiy()
        {
            var apiurl = "http://hv.erp.b1b.com/PvDataApi/ClassifyInfo";
            var result = Yahv.Utils.Http.ApiHelper.Current.Get<JSingle<ClassifiedInfo>>(apiurl, new
            {
                cpnId = "48DEC037A51CE69537FD4EE26943A0E9"
            });
            var data = result.data;
            var dic = data.Elements;
        }
    }

    class ClassifiedInfo
    {
        public string PartNumber;

        public string Manufacturer;

        public string HSCode;

        public Dictionary<string, string> Elements;
    }
}