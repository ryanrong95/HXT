using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.HKWarehouse.Sorting
{
    public partial class Packing : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        protected void LoadData()
        {
            //包装种类
            this.Model.WarpTypeData = Needs.Wl.Admin.Plat.AdminPlat.BasePackType.Select(item => new
            {
                item.ID,
                item.Code,
                item.Name
            }).OrderBy(x => x.Code).Json();
            //承运商
            this.Model.CarrierData = "".Json();
            var data = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.Carriers;
            this.Model.CarrierData = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.Carriers.Where(x => x.Status == Status.Normal).Select(item => new
            {
                value = item.ID,
                text = item.Name,
                Type = item.CarrierType
            }).Where(x => x.Type == CarrierType.InteExpress).Json();

            string ID = Request.QueryString["ID"];
            var entryNotice = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.HKEntryNoticeSimple[ID];
            this.Model.WraptypeValue = entryNotice.Order.WarpType.Json();
            //订单国际快递
            this.Model.OrderWaybillData = "".Json();
            string OrderID = Request.QueryString["OrderID"];
            var waybill = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.OrderWaybill.Where(item => item.OrderID == OrderID);
            this.Model.OrderWaybillData = waybill.Select(item => new
            {
                value = item.ID,
                text = item.WaybillCode
            }).Json();

        }


        /// <summary>
        /// 装箱
        /// </summary>
        protected void PackingBoxIndex()
        {
            try
            {
                string OrderID = Request.Form["OrderID"];
                string EntryNoticeID = Request.Form["EntryNoticeID"];
                string BoxIndex = Request.Form["BoxIndex"];
                DateTime PackingDate = Convert.ToDateTime(Request.Form["PackingDate"]);


                //判断箱号是否已经用过
                var packingView = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.Packing;
                var packings = packingView.Where(item => item.PackingDate == PackingDate && item.BoxIndex.StartsWith("WL"));
                int[] arr1 = this.GetCaseNumbers(BoxIndex);
                int[] arr2 = this.GetCaseNumbers(packings);
                var diffArr = arr1.Where(c => arr2.Contains(c)).ToArray();
                if (diffArr.Count() > 0)
                {
                    string caseNumber = "WL" + diffArr.First().ToString().PadLeft(2, '0');
                    Response.Write((new { success = false, message = "箱号" + caseNumber + "已使用过：请选择其它箱号。" }).Json());
                    return;
                }

                decimal Weight = decimal.Parse(Request.Form["Weight"]);
                string PackingType = Request.Form["PackingType"];
                string ShelveNumber = Request.Form["ShelveNumber"];
                decimal Quantity = decimal.Parse(Request.Form["Quantity"]);
                string isExpress = Request.Form["isExpress"];
                string WaybillCode = Request.Form["WaybillCode"];
                var waybill = new Needs.Ccs.Services.Models.OrderWaybill();
                if (!string.IsNullOrEmpty(isExpress))
                {
                    string Carrier = Request.Form["Carrier"];
                    string ArrivalTime = Request.Form["ArrivalTime"];
                    ///1.如果承运商名称不存在 ，则创建承运商
                    var result = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.Carriers.Where(x => x.Status == Status.Normal && x.Name == Carrier.Trim());
                    if (result.Count() < 1)
                    {
                        //增加承运商信息
                        var entity = new Needs.Ccs.Services.Models.Carrier();
                        entity.Code = Request.Form["Carrier"];
                        entity.Name = Request.Form["Carrier"];
                        entity.CarrierType = CarrierType.InteExpress;
                        entity.Contact = new Needs.Ccs.Services.Models.Contact();
                        entity.Contact.Name = "";
                        entity.Contact.Mobile = "";
                        entity.Enter();
                        // 增加运单信息
                         waybill = new Needs.Ccs.Services.Models.OrderWaybill();
                        waybill.OrderID = OrderID;
                        waybill.ArrivalDate = Convert.ToDateTime(ArrivalTime);
                        waybill.Carrier = entity;
                        waybill.WaybillCode = WaybillCode;
                        waybill.AdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
                        waybill.Enter();
                    }
                    else {

                        // 增加运单信息
                        waybill.OrderID = OrderID;
                        waybill.ArrivalDate = Convert.ToDateTime(ArrivalTime);
                        waybill.Carrier = result.FirstOrDefault();
                        waybill.WaybillCode = WaybillCode;
                        waybill.AdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
                        waybill.Enter();
                    }

                };



                var hkSorting = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.HKSortingContext;
                hkSorting.Admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                hkSorting.EntryNoticeID = EntryNoticeID;
                hkSorting.ToShelve(ShelveNumber, BoxIndex);
                hkSorting.SetWaybill(waybill.ID);
                //创建packing对象
                PackingModel packing = new PackingModel();

                packing.AdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
                packing.OrderID = OrderID;
                packing.BoxIndex = BoxIndex;
                packing.Weight = Weight;
                packing.WrapType = PackingType;
                packing.PackingDate = Convert.ToDateTime(PackingDate);
                packing.Quantity = Quantity;
                hkSorting.SetPacking(packing);
                //选择的装箱列表数据
                string sortings = Request.Form["Sortings"].Replace("&quot;", "'");
                IEnumerable<SortingModel> list = sortings.JsonTo<IEnumerable<SortingModel>>();
                hkSorting.Items = list;
              
                //开始装箱
                hkSorting.Pack();
                Response.Write((new { success = true, message = "装箱成功！" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "装箱失败：" + ex.Message }).Json());
            }
        }

        private int[] GetCaseNumbers(string CaseNumber)
        {
            List<int> list = new List<int>();
            if (CaseNumber.Contains("-"))
            {
                string[] str = CaseNumber.Split('-');
                int box1 = int.Parse(str[0].Remove(0, 2));
                int box2 = int.Parse(str[1].Remove(0, 2));
                for (int i = box1; i < box2 + 1; i++)
                {
                    list.Add(i);
                }
            }
            else
            {
                list.Add(int.Parse(CaseNumber.Remove(0, 2)));
            }
            return list.ToArray();
        }

        private int[] GetCaseNumbers(IQueryable<Needs.Ccs.Services.Models.Packing> packings)
        {

            List<int> list = new List<int>();
            foreach (var packing in packings)
            {
                int[] array = this.GetCaseNumbers(packing.BoxIndex);
                list.AddRange(array);
            }
            return list.ToArray();
        }

        /// <summary>
        /// 国际快递信息
        /// </summary>
        protected void WaybillData()
        {
            string OrderID = Request.Form["OrderID"];

            var waybill = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.OrderWaybill;
            var data = waybill.Where(item => item.OrderID == OrderID);
            Func<Needs.Ccs.Services.Models.OrderWaybill, object> convert = item => new
            {
                ID = item.ID,
                CompanyName = item.Carrier.Name,
                WaybillCode = item.WaybillCode,
                ArrivalDate = item.ArrivalDate.ToString("yyyy-MM-dd"),
            };
            Response.Write(new
            {
                rows = data.Select(convert).ToArray()
            }.Json());
        }

    }
}