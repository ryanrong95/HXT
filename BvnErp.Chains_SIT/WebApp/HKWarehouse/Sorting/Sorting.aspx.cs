using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using Needs.Wl;
using Needs.Wl.Warehouse.Services.PageModels;
using Needs.Wl.Warehouse.Services.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.HKWarehouse.Sorting
{
    public partial class Sorting : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        protected void LoadData()
        {

            string ID = Request.QueryString["ID"];
            var entryNotice = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.HKEntryNotice[ID];
            // 产地
            this.Model.OriginData = Needs.Wl.Admin.Plat.AdminPlat.Countries.Select(item => new
            {
                OriginValue = item.Code,
                OriginText = item.Code + " " + item.Name
            }).Json();

            //分拣要求
            this.Model.SortingRequireData = "".Json();

            if (entryNotice != null)
            {
                this.Model.SortingRequireData = new
                {
                    SortingRequireValue = entryNotice.SortingRequire,
                    SortingRequireText = entryNotice.SortingRequire.GetDescription(),
                }.Json();

                var orderVoyage = Needs.Wl.Admin.Plat.AdminPlat.Current.Voyage.OrderVoyageNo.Where(t => t.Order.ID == entryNotice.Order.ID);
                string spcialType = "";
                foreach (var item in orderVoyage)
                {
                    spcialType += item.Type.GetDescription() + "|";
                }

                var orderConsignee = new OrderConsigneesView().Where(t => t.OrderID == entryNotice.Order.ID).FirstOrDefault();
                string[] orderids = entryNotice.Order.ID.Split('-');
                var Files = new Needs.Ccs.Services.Views.MainOrderFilesView().Where(x => x.MainOrderID == orderids[0] && x.FileType == FileType.DeliveryFiles && x.Status == Status.Normal).FirstOrDefault();
                this.Model.BaseInfo = new
                {

                    ClientCode = entryNotice.Order.Client.ClientCode + "|" + entryNotice.Order.Client.Company.Name,
                    OrderID = entryNotice.Order.ID,
                    CreateDate = entryNotice.Order.CreateDate.ToString(),
                    entryNotice.EntryNoticeStatus,
                    orderVoyageType = spcialType == "" ? "无" : spcialType.TrimEnd('|'),
                    deliveryType = orderConsignee == null ? "" : orderConsignee.Type.GetDescription(),
                    PickUpDate = orderConsignee?.PickUpTime?.ToString(),
                    PickUpURL = FileDirectory.Current.FileServerUrl + "/" + Files?.Url.ToUrl(),// 查看路径
                    FileName = Files?.Name == null ? "" : Files?.Name,
                    orderConsignee.Type
                }.Json();
            }


            this.Model.WraptypeValue = entryNotice.Order.WarpType.Json();
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
                EntryNoticeID = ID,
                EntryNoticeItemID = item.ID,
                OrderItemID = item.OrderItem.ID,
                //ProductID = item.OrderItem.Product.ID,
                ProductModel = item.OrderItem.Model,//型号
                SpecialType = item.OrderItem.Category.GetSpecialTypeForHKWarehouse(),
                ProductName = item.OrderItem.Category.Name,  //归类后品名
                OrderQuantity = item.RelQuantity,
                Quantity = item.RelQuantity,
                Manufacturer = item.OrderItem.Manufacturer,
                Batch = item.OrderItem.Batch,
                Origin = item.OrderItem.Origin,
                IsSportCheck = item.IsSportCheck,
                OrderID = entryNotice.Order.ID,
                IsMatched = false
            };

            Response.Write(new
            {
                rows = data.Select(convert).ToArray(),
                total = data.Count()
            }.Json());
        }

        /// <summary>
        /// 已装箱产品列表(data2)
        /// </summary>
        protected void LoadPackedProduct()
        {
            //新写法
            string orderID = Request.QueryString["OrderID"];
            PackedProductsView view = new PackedProductsView(orderID);
            view.AllowPaging = false;
            IList<PackedProductListModel> list = view.ToList();

            Response.Write(new
            {
                rows = list.Select(item => new
                {
                    ID = item.ID,
                    PackingID = item.PackingID,
                    SortingID = item.ID,
                    BoxIndex = item.BoxIndex,
                    NetWeight = item.NetWeight,
                    GrossWeight = item.GrossWeight,
                    Model = item.Model,//产品型号
                    Name = item.Name,  //产品名称
                    Quantity = item.Quantity,
                    Origin = item.Origin,
                    Manufacturer = item.Manufacturer,
                    Status = item.PackingStatus.GetDescription(),
                    StatusValue = item.PackingStatus,
                    SpecialType = item.SpecialType == Needs.Ccs.Services.Enums.ItemCategoryType.Normal ? "-" : item.SpecialType.GetFlagsDescriptions("|"),
                }
                ).ToArray(),
            }.Json());

            //string OrderID = Request.QueryString["OrderID"];
            //var packingBill = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.SortingPackings.GetSortingPacking();
            //var data = packingBill.Where(Item => Item.OrderID == OrderID);

            //Func<SortingPacking, object> convert = item => new
            //{
            //    ID = item.ID,
            //    PackingID = item.Packing.ID,
            //    SortingID = item.ID,
            //    BoxIndex = item.BoxIndex,
            //    NetWeight = item.NetWeight,
            //    GrossWeight = item.GrossWeight,
            //    Model = item.OrderItem.Model,//产品型号
            //    Name = item.OrderItem.Category.Name,  //产品名称
            //    Quantity = item.Quantity,
            //    Origin = item.OrderItem.Origin,
            //    Manufacturer = item.OrderItem.Manufacturer,
            //    Status = item.Packing.PackingStatus.GetDescription(),
            //    StatusValue = item.Packing.PackingStatus,
            //    SpecialType = item.OrderItem.Category.GetSpecialType()
            //};
            //Response.Write(new { rows = data.Select(convert).ToArray(), }.Json());
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
        /// 修改型号
        /// </summary>
        protected void ChangeProductModel()
        {
            try
            {
                string orderItemID = Request.Form["OrderItemID"];

                string productmodel = Request.Form["ProductModel"].Replace("amp;", "");
                var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);

                var orderItem = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.OrderItems[orderItemID];
                if (productmodel != orderItem.Model)
                {
                    orderItem.SorterAdmin = admin;
                    orderItem.ChangeProductModel(productmodel);
                }

                NoticeLog noticeLog = new NoticeLog();
                noticeLog.MainID = orderItem.OrderID;
                noticeLog.NoticeType = Needs.Ccs.Services.Enums.SendNoticeType.ModelChange;
                noticeLog.SendNotice();

                Response.Write((new { success = true, message = "修改成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "修改失败：" + ex.Message }).Json());
            }
        }
        /// <summary>
        /// 修改原产地
        /// </summary>
        protected void ChangeOrigin()
        {
            try
            {
                string origin = Request.Form["OriginValue"];
                string orderItemID = Request.Form["OrderItemID"];
                //int Origincount = Needs.Wl.Admin.Plat.AdminPlat.Countries.Where(x=>x.Code==origin);
                //if (Origincount == 0)
                //{
                //    Response.Write((new { success = false, message = "产地不正确！" }).Json());
                //    return;
                //}


                var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);

                var orderItem = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.OrderItems[orderItemID];
                if (origin != orderItem.Origin)
                {
                    orderItem.SorterAdmin = admin;
                    orderItem.ChangeOrigin(origin);
                }

                NoticeLog noticeLog = new NoticeLog();
                noticeLog.MainID = orderItem.OrderID;
                noticeLog.NoticeType = Needs.Ccs.Services.Enums.SendNoticeType.OriginChange;
                noticeLog.SendNotice();

                Response.Write((new { success = true, message = "修改成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "操作失败：" + ex.Message }).Json());
            }
        }

      
        /// <summary>
        /// 修改品牌
        /// </summary>

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

                NoticeLog noticeLog = new NoticeLog();
                noticeLog.MainID = orderItem.OrderID;
                noticeLog.NoticeType = Needs.Ccs.Services.Enums.SendNoticeType.ManufactureChange;
                noticeLog.SendNotice();

                Response.Write((new { success = true, message = "修改成功" }).Json());

            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "修改失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 修改批次号
        /// </summary>

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
                    //  orderItem.Product.Batch = batch;
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
                ////判断订单是否挂起
                //var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.Orders[entryNotice.Order.ID];

                ////排除用户的、排除非用户的已处理的 剩下的未处理的管控 的 数量
                ////order.IsHangUp == true && exceptUserUnAuditControlCount > 0 才是原本要阻止的订单挂起
                //var exceptUserUnAuditControlCount = new Needs.Ccs.Services.Views.ExceptUserUnAuditControlView().Where(t => t.OrderID == entryNotice.Order.ID).Count();

                //if (order != null && order.IsHangUp && exceptUserUnAuditControlCount > 0)
                //{
                //    Response.Write((new { success = false, message = "封箱失败：订单已经挂起。" }).Json());
                //    return;
                //}
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

        /// <summary>
        /// 检查是否有未处理的审批
        /// </summary>
        protected void CheckIsHasUnApproved()
        {
            string tinyOrderID = Request.Form["TinyOrderID"];

            List<LambdaExpression> lamdas = new List<LambdaExpression>();
            Expression<Func<UnApprovedForHkWarehouseViewModel, bool>> lambda1 = item => item.TinyOrderID == tinyOrderID;
            lamdas.Add(lambda1);
            var unApproved = new Needs.Ccs.Services.Views.UnApprovedForHkWarehouseView().GetResult(lamdas.ToArray());

            if (unApproved != null && unApproved.Any())
            {
                //有未处理的审批
                string[] controlTypes = unApproved.Select(t => t.ControlType.GetDescription()).ToArray();
                string strControlTypes = string.Join("、", controlTypes);

                Response.Write((new { isHas = true, message = "该订单有未处理的审批项（" + strControlTypes + "），您所要的操作咱不能进行。" }).Json());
            }
            else
            {
                //没有未处理的审批
                Response.Write((new { isHas = false, message = "没有未处理的审批" }).Json());
            }
        }


    }
}