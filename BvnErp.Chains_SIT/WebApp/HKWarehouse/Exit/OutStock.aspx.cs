using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;



namespace WebApp.HKWarehouse.Exit
{
    /// <summary>
    ///待出库通知展示界面
    ///香港库房
    /// </summary>
    public partial class OutStock : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        /// <summary>
        /// 出库信息
        /// </summary>
        protected void LoadData()
        {
            this.Model.ExitNotices = "".Json();
            string id = Request["ExitNoticeID"];
            var exitNotice = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.HKExitNotice[id];

            this.Model.ExitNotices = new
            {
                OrderId = exitNotice.Order.ID,
                NoticeID = exitNotice.ID,
                VehicleNo = exitNotice.OutputWayBill?.Voyage.HKLicense==null?"" : exitNotice.OutputWayBill?.Voyage.HKLicense,
                DriverCode = exitNotice.OutputWayBill?.Voyage.DriverCode==null?"": exitNotice.OutputWayBill?.Voyage.DriverCode,
                DriverName = exitNotice.OutputWayBill?.Voyage.DriverName==null?"": exitNotice.OutputWayBill?.Voyage.DriverName,
                VoyageNo = exitNotice.DecHead.VoyNo,
                exitNotice.DecHead.BillNo,
                exitNotice.DecHead.PackNo,
            }.Json();
        }

        /// <summary>
        /// 待出库商品信息
        /// </summary>
        protected void data()
        {
         
            //出库描述:1、根据销售（报关单）生成出库通知及运单
            //出库描述:2、根据出库通知，到库房的货架中拣货（找货，根据出库通知中的产品所在的库位、箱号进行拣货）
            //出库描述:3、理论上拣货完成后需要进行包装称重等,完成装箱单，但我们代报关业务中这些都是已知的（先进行申报后再出库），装箱单等都已经完成，所以没有拣货、不进行包装称重。

            //--出库通知对象的设计
            //报关单list(销售单)
            //库位（sorting packing）
            //产品

            string id = Request["ExitNoticeID"];
            //1.根据ID获取出库通知内容
            var exitNoticeItem = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.ExitProductData.GetExitProductData();
            var data = exitNoticeItem.Where(x => x.ExitNoticeID == id);

            Func<HKExitProduct, object> convert = item => new
            {
               // CaseNumber = item.DecList.DeclarationNoticeItem.Sorting.StorageBoxIndex,//使用产品所在库位中的箱号
                CaseNumber =item.StoreStorage.BoxIndex,//产品所在库位中的箱号
                PackingDate = item.PackingDate.ToString("yyyy-MM-dd"),//使用产品所在库位中的箱号所在的装箱日期 及sorting 的paking 的packingdate
                NetWeight = item.DecList.NetWt,
                GrossWeight = item.DecList.GrossWt,
                ProductName = item.DecList.GName,
                Model = item.DecList.GoodsModel,
                Manufactor = item.DecList.GoodsBrand,
                Quantity = item.Quantity,
                StockCode = item.StoreStorage.StockCode        //使用产品所在库位中的库位号。        
            };
            this.Paging(data, convert);
        }

        /// <summary>
        /// 确认出库
        /// </summary>
        protected void SaveOutStock()
        {
            try
            {
                string ID = Request["ID"];

                var ExitNotice = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.HKExitNotice[ID];
                ExitNotice.OutStock();
                Response.Write((new { success = true, message = "出库成功！" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "处理失败：" + ex.Message }).Json());
            }
        }
    }
}