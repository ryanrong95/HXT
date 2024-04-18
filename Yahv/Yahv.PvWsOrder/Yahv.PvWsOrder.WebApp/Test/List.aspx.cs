using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Linq.Extends;
using Yahv.PvWsOrder.Services;
using Yahv.PvWsOrder.Services.Models;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PvWsOrder.WebApp.Test
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var id = string.Concat("", "DBAEAB43B47EB4299DD1D62F764E6B6A", "D6B972663DAE6B255A2D5A6EF43F85D3").MD5();
        }

        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <returns></returns>
        protected object data()
        {
            // SubmitChanged();
            SubmitSorted();

            ////查询参数
            //var Name = Request.QueryString["Name"];
            //var query = new Services.Views.AdminsAll().Where(item => true);
            ////查询表达式
            //Expression<Func<Yahv.Services.Models.Admin, bool>> predicate = null;
            //if (!string.IsNullOrWhiteSpace(Name))
            //{
            //    Name = Name.Trim();
            //    predicate = predicate.And(item => item.UserName.Contains(Name) || item.RealName.Contains(Name));
            //}

            //if (predicate != null)
            //{
            //    query = query.Where(predicate);
            //}
            //return this.Paging(query.OrderBy(t => t.ID), t => new
            //{
            //    ID = t.ID,
            //    UserName = t.UserName,
            //    RealName = t.RealName,
            //    RoleName = t.RoleName,
            //    LastLoginDate = t.LastLoginDate,
            //});
            return null;
        }

        private void SubmitSorted()
        {
            //调用代仓储 库房分拣更新数据接口
            var apisetting = new PvWsOrderApiSetting2();
            var apiurl = ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.SubmitSorted;

            var result = Yahv.Utils.Http.ApiHelper.Current.JPost(apiurl, new StoreChange
            {
                List = {
                        new ChangeItem() { orderid = "XL0000320200329001" },
                }
            });
        }

        private void SubmitChanged(string OrderId = "Order201912230007")
        {

            //var items = Erp.Current.WsOrder.OrderItems.Where(item => item.OrderID == OrderId && !item.ID.Contains("OOT"));

            //OrderChanges changes = new OrderChanges();
            //changes.OrderID = OrderId;
            //changes.Confirmed = false;
            //changes.Currency = ((int)Currency.USD).ToString();

            //changes.items = items.Select(en => new OrderItemChanges
            //{
            //    OrderItemID = en.ID,
            //    InputID = en.InputID,
            //    Product = en.Product,
            //    Origin = en.Origin,
            //    DateCode = en.DateCode,
            //    Quantity = en.Quantity,
            //    UnitPrice = en.UnitPrice,
            //}).ToList();
            ////新增的情况
            //changes.items.Add(new OrderItemChanges
            //{
            //    OrderItemID = Layers.Data.PKeySigner.Pick(PKeyType.OrderItem),
            //    InputID = Layers.Data.PKeySigner.Pick(PKeyType.Input),
            //    Product = new CenterProduct() { Manufacturer = "AA", PartNumber = "AA" },
            //    Origin = ((int)Origin.USA).ToString(),
            //    DateCode = "批次号111",
            //    Quantity = 10,
            //    UnitPrice = 100.00m,
            //});
            ////修改的情况
            //changes.items[0].DateCode = "批次号修改";
            ////删除的情况
            //changes.items.RemoveAt(3);

            //string requestData = changes.Json();

            OrderChanges changes = new OrderChanges();
            changes.OrderID = "Order202001020013";
            changes.Currency = "USD";
            changes.Confirmed = false;
            changes.items = new List<OrderItemChanges>();
            changes.items.Add(new OrderItemChanges {
                OrderItemID = "OrderItem20200102000022",
                InputID = "Ipt2020010200000075",
                Product = new CenterProduct {
                    PartNumber= "#B966AS-160M=P3",
                    Manufacturer= "MURATA",
                },
                Origin = "IOT",
                DateCode = "",
                Quantity = 50m,
                UnitPrice = 20.00000m,
                Unit ="007", 
            });

            string requestData = changes.Json();

            //调用代仓储 库房分拣更新数据接口
            var apisetting = new PvWsOrderApiSetting2();
            var apiurl = ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.SubmitChanged;
            var result = Yahv.Utils.Http.ApiHelper.Current.JPost(apiurl, changes);
        }
    }
}