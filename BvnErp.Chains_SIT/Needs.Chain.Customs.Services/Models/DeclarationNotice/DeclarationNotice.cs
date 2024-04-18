using Needs.Ccs.Services.Hanlders;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 报关通知
    /// </summary>
    public class DeclarationNotice : IUnique, IPersist
    {
        #region 属性
        public string ID { get; set; }

        public string OrderID { get; set; }

        public Interfaces.IOrder order { get; set; }

        /// <summary>
        /// 报关员
        /// </summary>
        public string AdminID { get; set; }
        public Admin Admin { get; set; }

        /// <summary>
        /// 卖方，默认香港宏图
        /// </summary>
        //public Party Consignor { get; set; }

        /// <summary>
        /// 买方
        /// </summary>
        //public Party Consignee { get; set; }

        /// <summary>
        /// 代理报关公司
        /// </summary>
        //public Company Agent { get; set; }

        /// <summary>
        /// 报关通知状态：未处理、部分处理、已处理
        /// </summary>
        public Enums.DeclareNoticeStatus Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }


        OrderItems _orderItems;

        OrderItems OrderItems
        {
            get
            {
                if (_orderItems == null)
                {
                    _orderItems = this.order.Items;
                }
                return _orderItems;
            }
        }

        ///// <summary>
        ///// 冗余 是否商检
        ///// </summary>
        //public bool IsInspection
        //{
        //    get
        //    {
        //        return this.OrderItems.Any(t => (t.Category.Type.GetHashCode() & Needs.Ccs.Services.Enums.ItemCategoryType.Inspection.GetHashCode()) > 0);
        //    }
        //}
        ///// <summary>
        ///// 冗余 是否检疫
        ///// </summary>
        //public bool IsQuarantine
        //{
        //    get
        //    {
        //        return this.OrderItems.Any(t => (t.Category.Type.GetHashCode() & Needs.Ccs.Services.Enums.ItemCategoryType.Quarantine.GetHashCode()) > 0);
        //    }
        //}

        //public string InspQuarName
        //{
        //    get
        //    {
        //        if (this.IsInspection && this.IsQuarantine)
        //        {
        //            return "商检/检疫";
        //        }
        //        else if (this.IsInspection == true && this.IsQuarantine == false)
        //        {
        //            return "商检/";
        //        }
        //        else if (this.IsInspection == false && this.IsQuarantine == true)
        //        {
        //            return "/检疫";
        //        }
        //        else
        //        {
        //            return "";
        //        }
        //    }
        //}

        /// <summary>
        /// 是否包车
        /// </summary>
        //[Description("是否包车")]
        public bool IsCharterBus { get; set; }

        /// <summary>
        /// 是否高价值
        /// </summary>
        //[Description("是否高价值")]
        public bool IsHighValue { get; set; }

        /// <summary>
        /// 是否商检
        /// </summary>
        //[Description("是否商检")]
        public bool IsInspection { get; set; }

        /// <summary>
        /// 是否检疫
        /// </summary>
        //[Description("是否检疫")]
        public bool IsQuarantine { get; set; }

        /// <summary>
        /// 是否3C
        /// </summary>
        //[Description("是否3C")]
        public bool IsCCC { get; set; }

        /// <summary>
        /// 运输批次号
        /// </summary>
        public string VoyageID { get; set; }

        /// <summary>
        /// 运输类型
        /// </summary>
        public Enums.VoyageType VoyageType { get; set; }

        /// <summary>
        /// 报关通知项
        /// </summary>
        DeclarationNoticeItems items;
        public DeclarationNoticeItems Items
        {
            get
            {
                if (items == null)
                {
                    using (var view = new Views.DeclarationNoticeItemsView())
                    {
                        var query = view.Where(item => item.DeclarationNoticeID == this.ID);
                        this.Items = new DeclarationNoticeItems(query);
                    }
                }
                return this.items;
            }
            set
            {
                if (value == null)
                {
                    return;
                }

                this.items = new DeclarationNoticeItems(value, new Action<DeclarationNoticeItem>(delegate (DeclarationNoticeItem item)
                {
                    item.DeclarationNoticeID = this.ID;
                }));
            }
        }

        /// <summary>
        /// 件数(从库中 Sorting 表查询)
        /// </summary>
        public int TotalPacks
        {
            get
            {
                try
                {
                    string orderPre = this.OrderID.Substring(0, 5);
                    string[] innerCompanies = DyjInnerCompanies.Current.Companies.Split(',');
                    if (innerCompanies.Contains(orderPre))
                    {
                        var boxcodes = this.Items.Select(item => item.Sorting.BoxIndex).Distinct().ToList();
                        return new CalculateContext(Enums.CompanyTypeEnums.Inside, boxcodes).CalculatePacks();
                    }
                    else
                    {
                        var boxcodes = this.Items.Select(item => item.Sorting.BoxIndex).Distinct().ToList();
                        return new CalculateContext(Enums.CompanyTypeEnums.OutSide, boxcodes).CalculatePacks();
                    }
                }
                catch (Exception ex)
                {
                    ex.CcsLog("制单计算大赢家箱号出错");
                    return 0;
                }
            }
        }

        /// <summary>
        /// 总数量
        /// </summary>
        public decimal TotalQty { get; set; }

        /// <summary>
        /// 型号数量
        /// </summary>
        public int TotalModelQty { get; set; }

        /// <summary>
        /// 报关通知日志
        /// </summary>
        DeclarationNoticeLogs logs;
        public DeclarationNoticeLogs Logs
        {
            get
            {
                if (logs == null)
                {
                    using (var view = new Views.DeclarationNoticeLogsView())
                    {
                        var query = view.Where(item => item.DeclarationNoticeID == this.ID);
                        this.Logs = new DeclarationNoticeLogs(query);
                    }
                }
                return this.logs;
            }
            set
            {
                if (value == null)
                {
                    return;
                }

                this.logs = new DeclarationNoticeLogs(value, new Action<DeclarationNoticeLog>(delegate (DeclarationNoticeLog item)
                {
                    item.DeclarationNoticeID = this.ID;
                }));
            }
        }

        public string DecHeadID { get; set; }
        public DecHead DecHead { get; set; }

        /// <summary>
        /// 订单特殊类型(包车)
        /// </summary>
        public OrderVoyage OrderVoyageCharterBus { get; set; }

        /// <summary>
        /// 订单特殊类型(高价值)
        /// </summary>
        public OrderVoyage OrderVoyageHighValue { get; set; }

        /// <summary>
        /// 订单特殊类型(商检)
        /// </summary>
        public OrderVoyage OrderVoyageInspection { get; set; }

        /// <summary>
        /// 订单特殊类型(检疫)
        /// </summary>
        public OrderVoyage OrderVoyageQuarantine { get; set; }

        /// <summary>
        /// 报关通知的运输批次
        /// </summary>
        public DecNoticeVoyage DecNoticeVoyage { get; set; }

        #endregion

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;
        public event DeclarationNoticeHanlder DecNoticeCancel;


        public DeclarationNotice()
        {
            this.Status = Enums.DeclareNoticeStatus.UnDec;
            this.UpdateDate = this.CreateDate = DateTime.Now;
            DecNoticeCancel += DeclarationNotice_Cancel;
            EnterSuccess += DeclareNotice_EnterSuccess;
        }

        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DeclarationNotices>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.DeclareNotice);
                    reponsitory.Insert(this.ToLinq());
                }
                else
                {
                    reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
                }
            }

            this.OnEnter();
        }

        /// <summary>
        /// 创建报关通知
        /// </summary>
        public void CreateNotice()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //报关通知
                this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.DeclareNotice);
                reponsitory.Insert(this.ToLinq());

                //报关通知项
                foreach (var item in this.Items)
                {
                    reponsitory.Insert(item.ToLinq());
                    //只更新sorting状态
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.Sortings>(new { DecStatus = Enums.SortingDecStatus.Yes }, t => t.ID == item.Sorting.ID);
                    //更新装箱结果状态
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.Packings>(new { PackingStatus = Enums.PackingStatus.Sealed }, t => t.OrderID == this.OrderID && t.BoxIndex == item.Sorting.BoxIndex);
                }



                //reponsitory.Update<Layer.Data.Sqls.ScCustoms.Sortings>(new { DecStatus = Enums.SortingDecStatus.Yes }, t => t.OrderID == this.OrderID);
            }

            this.OnEnter();
        }

        virtual protected void OnEnter()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }

        /// <summary>
        /// 报关通知取消
        /// </summary>
        public bool CancelDeclaration()
        {
            bool IsSuccess = false;
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {                
                var Declaration = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>().Where(item => item.DeclarationNoticeID == this.ID).Select(item => new { ID = item.ID, BillNo = item.BillNo }).FirstOrDefault();
                if (Declaration != null)
                {

                    var DecHead = new Ccs.Services.Views.DecHeadsView().Where(item => item.ID == Declaration.ID).FirstOrDefault();
                    if (DecHead != null)
                    {
                        if (DecHead.IsSuccess == false)
                        {
                            //更改报关通知状态
                            reponsitory.Update<Layer.Data.Sqls.ScCustoms.DeclarationNotices>(new { Status = (int)Enums.DeclareNoticeStatus.Cancel }, item => item.ID == this.ID);
                            //更改报关通知项状态
                            reponsitory.Update<Layer.Data.Sqls.ScCustoms.DeclarationNoticeItems>(new { Status = (int)Enums.DeclareNoticeItemStatus.Cancel }, item => item.DeclarationNoticeID == this.ID);
                            DecHead.CancelDecHead();
                            this.OnCanceled(new DeclarationNoticeEventArgs(this));
                            IsSuccess = true;
                        }
                    }
                }
                else
                {
                    //更改报关通知状态
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.DeclarationNotices>(new { Status = (int)Enums.DeclareNoticeStatus.Cancel }, item => item.ID == this.ID);
                    //更改报关通知项状态
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.DeclarationNoticeItems>(new { Status = (int)Enums.DeclareNoticeItemStatus.Cancel }, item => item.DeclarationNoticeID == this.ID);
                    IsSuccess = true;
                }
            }

            return IsSuccess;
        }

        public void OnCanceled(DeclarationNoticeEventArgs args)
        {
            this.DecNoticeCancel?.Invoke(this, args);
        }

        /// <summary>
        /// 取消log
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void DeclarationNotice_Cancel(object sender, DeclarationNoticeEventArgs args)
        {
            args.DecNotice.Trace("报关通知已取消");
        }

        private void DeclareNotice_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            var dec = (DeclarationNotice)e.Object;
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var sortingPackingView = new Views.SortingPackingsView(reponsitory);
                foreach (var Item in dec.Items)
                {
                    var packing = sortingPackingView.Where(item => item.ID == Item.Sorting.ID).FirstOrDefault();
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.Packings>(
                           new
                           {
                               UpdateDate = DateTime.Now,
                               PackingStatus = Enums.PackingStatus.Sealed
                           }, t => t.ID == packing.Packing.ID);
                }
            }
        }
    }

    /// <summary>
    /// 报关通知,待制单,使用 Model
    /// </summary>
    public class DecNoticeListModel : IUnique
    {
        public string ID { get; set; } = string.Empty;

        /// <summary>
        /// 报关通知ID
        /// </summary>
        //[Description("报关通知ID")]
        public string DecNoticeID { get; set; } = string.Empty;

        /// <summary>
        /// 订单ID
        /// </summary>
        //[Description("订单ID")]
        public string OrderID { get; set; } = string.Empty;

        /// <summary>
        /// 公司ID
        /// </summary>
        public string CompanyID { get; set; } = string.Empty;

        /// <summary>
        /// 客户ID
        /// </summary>
        //[Description("客户ID")]
        public string ClientID { get; set; } = string.Empty;

        /// <summary>
        /// 客户名称
        /// </summary>
        //[Description("客户名称")]
        public string ClientName { get; set; } = string.Empty;

        /// <summary>
        /// 客户类型
        /// </summary>
        public Enums.ClientType ClientType { get; set; }

        /// <summary>
        /// 香港提货地址
        /// </summary>
        //[Description("香港提货地址")]
        public string ConsigneeAddress { get; set; } = string.Empty;

        /// <summary>
        /// 运输批次号
        /// </summary>
        //[Description("运输批次号")]
        public string VoyageID { get; set; } = string.Empty;

        /// <summary>
        /// 运输类型
        /// </summary>
        //[Description("运输类型")]
        public int VoyageType { get; set; }

        /// <summary>
        /// 运输类型名称
        /// </summary>
        public string VoyageTypeName { get; set; } = string.Empty;

        /// <summary>
        /// 币种
        /// </summary>
        //[Description("币种")]
        public string Currency { get; set; } = string.Empty;

        /// <summary>
        /// 创建日期
        /// </summary>
        //[Description("创建日期")]
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 订单特殊类型显示
        /// </summary>
        public string OrderSpecialTypeName { get; set; }

        /// <summary>
        /// 是否包车
        /// </summary>
        //[Description("是否包车")]
        public bool IsCharterBus { get; set; }

        /// <summary>
        /// 是否高价值
        /// </summary>
        //[Description("是否高价值")]
        public bool IsHighValue { get; set; }

        /// <summary>
        /// 是否商检
        /// </summary>
        //[Description("是否商检")]
        public bool IsInspection { get; set; }

        /// <summary>
        /// 是否检疫
        /// </summary>
        //[Description("是否检疫")]
        public bool IsQuarantine { get; set; }

        /// <summary>
        /// 是否3C
        /// </summary>
        //[Description("是否3C")]
        public bool IsCCC { get; set; }

        /// <summary>
        /// 是否原产地加征
        /// </summary>
        //[Description("是否原产地加征")]
        public bool IsOrigin { get; set; }

        /// <summary>
        /// 是否敏感产地
        /// </summary>
        public bool IsSenOrigin { get; set; }

        /// <summary>
        /// 报关总价
        /// </summary>
        //[Description("报关总价")]
        public decimal TotalDeclarePrice { get; set; }

        /// <summary>
        /// 型号数量
        /// </summary>
        //[Description("型号数量")]
        public int TotalModelQty { get; set; }

        /// <summary>
        /// 总数量
        /// </summary>
        //[Description("总数量")]
        public decimal TotalQty { get; set; }

        /// <summary>
        /// 报关员姓名
        /// </summary>
        public string DeclarantName { get; set; } = string.Empty;

        /// <summary>
        /// 流转箱号
        /// </summary>
        public string BoxIndex { get; set; } = string.Empty;

        /// <summary>
        /// 显示的件数
        /// </summary>
        public int PackNo { get; set; }
        /// <summary>
        /// 箱号
        /// </summary>
        public string PackBox { get; set; }

        /// <summary>
        /// Icgoo/大赢家订单号
        /// </summary>
        public string IcgooOrder { get; set; } = string.Empty;

        /// <summary>
        /// 总毛重
        /// </summary>
        public decimal TotalGrossWeight { get; set; }

        /// <summary>
        /// 制单人ID
        /// </summary>
        public string CreateDeclareAdminID { get; set; } = string.Empty;

        /// <summary>
        /// 制单人姓名
        /// </summary>
        public string CreateDeclareAdminName { get; set; } = string.Empty;
    }

    /// <summary>
    /// 报关通知,待制单,查看装箱单,使用 Model
    /// </summary>
    public class PackingListModel : IUnique
    {
        public string ID { get; set; } = string.Empty;

        /// <summary>
        /// 订单ID
        /// </summary>
        public string OrderID { get; set; } = string.Empty;

        /// <summary>
        /// 箱号
        /// </summary>
        public string BoxIndex { get; set; } = string.Empty;

        /// <summary>
        /// 品名
        /// </summary>
        public string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// 型号
        /// </summary>
        public string Model { get; set; } = string.Empty;

        /// <summary>
        /// OrderItemCategoryType
        /// </summary>
        public int OrderItemCategoryType { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// 净重
        /// </summary>
        public decimal? NetWeight { get; set; }

        /// <summary>
        /// 毛重
        /// </summary>
        public decimal? GrossWeight { get; set; }

        /// <summary>
        /// 产地
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// 税号
        /// </summary>
        public string HSCode { get; set; }
    }
}
