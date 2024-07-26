using Needs.Ccs.Services;
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

namespace WebApp.HKWarehouse.Entry
{
    /// <summary>
    /// 装箱界面
    /// 香港操作
    /// </summary>
    public partial class Sorting : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        protected void LoadData()
        {
            //产地
            this.Model.OriginData = Needs.Wl.Admin.Plat.AdminPlat.Countries.Select(item =>
           new
           {
               OriginValue = item.Code,
               OriginText = item.Code + " " + item.Name
           }).Json();
            //包装种类
            this.Model.WarpTypeData = Needs.Wl.Admin.Plat.AdminPlat.BasePackType.Select(item => new
            {
                item.ID,
                item.Code,
                item.Name
            }).OrderBy(x => x.Code).Json();

            //订单国际快递
            this.Model.OrderWaybillData = "".Json();
            string OrderID = Request.QueryString["OrderID"];
            var waybill = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.OrderWaybill.Where(item => item.OrderID == OrderID);
            this.Model.OrderWaybillData = waybill.Select(item => new
            {
                value = item.ID,
                text = item.WaybillCode
            }).Json();

            //分拣要求
            this.Model.SortingRequireData = "".Json();
            string ID = Request.QueryString["ID"];
            var entryNotice = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.HKEntryNoticeSimple[ID];
            if (entryNotice != null)
            {
                this.Model.SortingRequireData = new
                {
                    SortingRequireValue = entryNotice.SortingRequire,
                    SortingRequireText = entryNotice.SortingRequire.GetDescription(),
                }.Json();
            }

            this.Model.WraptypeValue = entryNotice.Order.WarpType.Json();
        }

        /// <summary>
        /// 国际快递信息
        /// </summary>
        protected void LoadWayBill()
        {
            string OrderID = Request.QueryString["OrderID"];
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

        /// <summary>
        /// 未装箱产品列表
        /// </summary>
        protected void LoadNoticeItems()
        {
            //查询未装箱的产品明细
            string ID = Request.QueryString["ID"];
            var entryNotice = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.HKEntryNotice[ID];
            if (entryNotice == null)
            {
                return;
            }
            //更新通知和项的状态 //TODO:修改
            entryNotice.UpdateItemsStatus();

            //查询数据
            var data = entryNotice.HKItems.Where(item => item.EntryNoticeStatus == EntryNoticeStatus.UnBoxed);
            //前台显示
            Func<HKEntryNoticeItem, object> convert = item => new
            {
                ID = item.ID,
                EntryNoticeItemID = item.ID,
                OrderItemID = item.OrderItem.ID,
                ProductID = item.OrderItem.ID,
                ProductModel = item.OrderItem.Model,//型号
                ProductName = item.OrderItem.Category.Name,  //归类后品名
                OrderQuantity = item.RelQuantity,
                Quantity = item.RelQuantity,
                Manufacturer = item.OrderItem.Manufacturer,
                Batch = item.OrderItem.Batch,
                Origin = item.OrderItem.Origin,
                IsSportCheck = item.IsSportCheck
            };
            Response.Write(new { rows = data.Select(convert).ToArray(), }.Json());
        }

        /// <summary>
        /// 已装箱产品列表(data2)
        /// </summary>
        protected void LoadPackedProduct()
        {
            string OrderID = Request.QueryString["OrderID"];
            var packingBill = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.SortingPackings.GetSortingPacking();
            var data = packingBill.Where(Item => Item.OrderID == OrderID);

            Func<SortingPacking, object> convert = item => new
            {
                ID = item.ID,
                PackingID = item.Packing.ID,
                SortingID = item.ID,
                BoxIndex = item.BoxIndex,
                NetWeight = item.NetWeight,
                GrossWeight = item.GrossWeight,
                Model = item.OrderItem.Model,//产品型号
                Name = item.OrderItem.Category.Name,  //产品名称
                Quantity = item.Quantity,
                Origin = item.OrderItem.Origin,
                Manufacturer = item.OrderItem.Manufacturer,
                Status = item.Packing.PackingStatus.GetDescription(),
                StatusValue = item.Packing.PackingStatus,
            };
            Response.Write(new { rows = data.Select(convert).ToArray(), }.Json());
        }

        /// <summary>
        /// 删除国际快递
        /// </summary>
        /// <returns></returns>
        protected string DeleteWayBill()
        {
            string id = Request.Form["ID"];
            var waybill = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.OrderWaybill[id];
            if (waybill != null)
            {
                waybill.Abandon();
            }
            //刷新WaybillCode下拉框
            string OrderID = Request.Form["OrderID"];
            var data = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.OrderWaybill.Where(item => item.OrderID == OrderID && item.Status == Status.Normal);
            return data.Select(item => new { value = item.ID, text = item.WaybillCode }).Json();
        }

        /// <summary>
        /// 分拣异常
        /// </summary>
        protected void AbnormalSorting()
        {
            try
            {
                string ID = Request.Form["ID"];
                var entryNotice = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.HKEntryNotice[ID];
                entryNotice.AbnormalSorting();
                Response.Write((new { success = true, message = "操作成功，订单已挂起！" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "操作失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 抽检异常
        /// </summary>
        protected void CheckProduct()
        {
            try
            {
                string id = Request.Form["ID"];
                var item = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.HKEntryNoticeItems[id];
                item.SpotAbnormal();

                Response.Write((new { success = true, message = "操作成功！" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "操作失败：" + ex.Message }).Json());
            }
        }

        protected void ChangeOrigin()
        {
            try
            {
                string origin = Request.Form["OriginValue"];
                string orderItemID = Request.Form["OrderItemID"];
                var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);

                var orderItem = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.OrderItems[orderItemID];
                if (origin != orderItem.Origin)
                {
                    orderItem.SorterAdmin = admin;
                    orderItem.ChangeOrigin(origin);
                }
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "操作失败：" + ex.Message }).Json());
            }
        }

        protected void ChangeManufacturer()
        {
            try
            {
                string orderItemID = Request.Form["OrderItemID"];

                string manufacturer = Request.Form["Manufacturer"].Replace("amp;", "");
                var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);

                var orderItem = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.OrderItems[orderItemID];
                if (manufacturer != orderItem.Manufacturer)
                {
                    orderItem.SorterAdmin = admin;
                    orderItem.ChangeManufacturer(manufacturer);
                }

                Response.Write((new { success = true, message = "修改成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "修改失败：" + ex.Message }).Json());
            }
        }

        protected void ChangeBatch()
        {
            try
            {
                string orderItemID = Request.Form["OrderItemID"];

                string batch = Request.Form["Batch"].Replace("amp;", "");
                var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                var orderItem = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.OrderItems[orderItemID];
                if (batch != orderItem.Batch)
                {
                    orderItem.SorterAdmin = admin;
                    orderItem.ChangeBatch(batch);
                }

                Response.Write((new { success = true, message = "修改成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "修改失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 装箱
        /// </summary>
        protected void Packing()
        {
            try
            {
                string OrderID = Request.Form["OrderID"];
                string EntryNoticeID = Request.Form["EntryNoticeID"];
                string BoxIndex = Request.Form["BoxIndex"];
                DateTime PackingDate = Convert.ToDateTime(Request.Form["PackingDate"]);

                //判断箱号是否已经用过
                var packingView = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.Packing;
                var packings = packingView.Where(item => item.PackingDate == PackingDate && item.BoxIndex.StartsWith("HXT"));
                int[] arr1 = this.GetCaseNumbers(BoxIndex);
                //  int[] arr2 = this.GetCaseNumbers(packings.Where(t => t.BoxIndex.Contains("HXT")));//外单箱号
                int[] arr2 = this.GetCaseNumbers(packings);
                var diffArr = arr1.Where(c => arr2.Contains(c)).ToArray();
                if (diffArr.Count() > 0)
                {
                    string caseNumber = "HXT" + diffArr.First().ToString().PadLeft(3, '0');
                    Response.Write((new { success = false, message = "箱号" + caseNumber + "已使用过：请选择其它箱号。" }).Json());
                    return;
                }

                decimal Weight = decimal.Parse(Request.Form["Weight"]);
                string PackingType = Request.Form["PackingType"];
                string ShelveNumber = Request.Form["ShelveNumber"];
                string WaybillCode = Request.Form["WaybillCode"];
                decimal Quantity = decimal.Parse(Request.Form["Quantity"]);

                //if (!BoxIndex.Contains("-"))
                //{
                //int number = int.Parse(BoxIndex.Replace("HXT", ""));
                //BoxIndex = "HXT" + number.ToString().PadLeft(3, '0');
                var hkSorting = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.HKSortingContext;
                hkSorting.ToShelve(ShelveNumber, BoxIndex);
                hkSorting.SetWaybill(WaybillCode);
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
                string input = Request.Form["Data"].Replace("&quot;", "'");
                IEnumerable<SortingModel> list = input.JsonTo<IEnumerable<SortingModel>>();
                hkSorting.Items = list;
                //开始装箱
                hkSorting.Pack();
                //}
                //else
                //{
                //    string input = Request.Form["Data"].Replace("&quot;", "'");
                //    string[] arr = BoxIndex.Split('-');
                //    int number1 = int.Parse(arr[0].Replace("HXT", ""));
                //    int number2 = int.Parse(arr[1].Replace("HXT", ""));
                //    int count = number2 - number1 + 1;

                //    var hkSorting = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.HKSortingContext;
                //    hkSorting.ToShelve(ShelveNumber, BoxIndex);
                //    hkSorting.SetWaybill(WaybillCode);

                //    for (int i = number1; i < number2 + 1; i++)
                //    {
                //        PackingModel packing = new PackingModel();
                //        packing.AdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
                //        packing.OrderID = OrderID;
                //        packing.BoxIndex = "HXT" + i.ToString().PadLeft(3, '0');
                //        packing.Weight = Weight / count;
                //        packing.WrapType = PackingType;
                //        packing.PackingDate = Convert.ToDateTime(PackingDate);
                //        packing.Quantity = Quantity / count;
                //        hkSorting.SetPacking(packing);
                //        //只勾选一个产品
                //        IEnumerable<SortingModel> list = input.JsonTo<IEnumerable<SortingModel>>();
                //        list.ToList().ForEach(t => t.Quantity = (t.Quantity / count));
                //        hkSorting.Items = list;
                //        //开始装箱
                //        hkSorting.Pack();
                //    }
                //}

                Response.Write((new { success = true, message = "装箱成功！" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "装箱失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 删除装箱结果
        /// </summary>
        protected void DeletePacking()
        {
            try
            {
                string PackingID = Request.Form["PackingID"];
                string EntryNoticeID = Request.Form["EntryNoticeID"];

                var datas = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.Packing.Where(item => item.ID == PackingID);
                var data = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.Packing[PackingID];
                if (data.PackingStatus != PackingStatus.UnSealed)
                {
                    Response.Write((new { success = false, message = "删除失败：已经封箱或出库" }).Json());
                    return;
                }

                var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                data.Delete(admin, EntryNoticeID);
                Response.Write((new { success = true, message = "删除成功！" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "删除失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 封箱
        /// </summary>
        protected void Sealed()
        {
            try
            {
                string EntryNoticeID = Request.Form["ID"];
                var entryNotice = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.HKEntryNotice[EntryNoticeID];
                //判断是否已全部封箱
                if (entryNotice != null)
                {
                    if (entryNotice.EntryNoticeStatus == EntryNoticeStatus.Sealed)
                    {
                        Response.Write((new { success = false, message = "封箱失败：已封箱完成。" }).Json());
                        return;
                    }
                }
                //判断订单是否挂起
                var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.Orders[entryNotice.Order.ID];
                if (order != null && order.IsHangUp)
                {
                    Response.Write((new { success = false, message = "封箱失败：订单已经挂起。" }).Json());
                    return;
                }
                var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                entryNotice.SetAdmin(admin);
                entryNotice.Seal();
                Response.Write((new { success = true, message = "封箱成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "封箱失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 取消封箱
        /// </summary>
        protected void CancelSealed()
        {
            try
            {
                string PackingID = Request.Form["PackingID"];
                var packing = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.Packing[PackingID];
                packing.CancelSealed();
                Response.Write((new { success = true, message = "取消封箱成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "取消封箱失败：" + ex.Message }).Json());
            }
        }

        private int[] GetCaseNumbers(string CaseNumber)
        {
            List<int> list = new List<int>();
            if (CaseNumber.Contains("-"))
            {
                string[] str = CaseNumber.Split('-');
                int box1 = int.Parse(str[0].Remove(0, 3));
                int box2 = int.Parse(str[1].Remove(0, 3));
                for (int i = box1; i < box2 + 1; i++)
                {
                    list.Add(i);
                }
            }
            else
            {
                list.Add(int.Parse(CaseNumber.Remove(0, 3)));
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
    }
}