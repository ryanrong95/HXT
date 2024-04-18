using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Underly.Attributes;

namespace Yahv.PvWsOrder.Services.XDTClientView
{
    /// <summary>
    /// 发票详情查询视图
    /// </summary>
    public class XDTOrderInvoiceView : UniqueView<XDTOrderInvoiceInfo, ScCustomReponsitory>
    {
        public XDTOrderInvoiceView()
        {

        }

        protected override IQueryable<XDTOrderInvoiceInfo> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.OrderInovieInfoView>()
                   select new XDTOrderInvoiceInfo
                   {
                       ID = entity.InvoiceNoticeItemID,
                       InvoiceType = (XDTInvoiceType)(entity.InvoiceType ?? 0),
                       InvoiceNoticeID = entity.InvoiceNoticeID,
                       TinyOrderID = entity.TinyOrderID,
                       OrderItemID = entity.OrderItemID,
                       TaxName = entity.TaxName,
                       Amount = entity.Amount,
                       InvoiceNo = entity.InvoiceNo,
                       InvoiceNoticeStatus = (XDTInvoiceNoticeStatus)entity.InvoiceNoticeStatus,
                       CreateDate = entity.CreateDate,
                       Quantity = entity.Quantity,
                       Model = entity.Model,
                       UnitName = entity.UnitName,
                       InvoiceTaxRate = entity.InvoiceTaxRate,
                       Price = entity.JinE,
                       TaxPrice = entity.ShuiE,
                   };
        }

        /// <summary>
        /// 根据大订单ID查询发票详情信息
        /// </summary>
        /// <param name="OrderID"></param>
        /// <returns></returns>
        public XDTOrderInvoiceInfo[] GetDetailByOrderID(string OrderID)
        {
            //根据订单ID获取所有小订单ID
            string[] TinyOrderids = this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.Orders>().Where(item => item.MainOrderId == OrderID).
                Select(item => item.ID).ToArray();

            XDTOrderInvoiceInfo[] data = new XDTOrderInvoiceInfo[0];

            foreach(var tinyorderid in TinyOrderids)
            {
                var linq = this.GetIQueryable().Where(item => item.TinyOrderID.Contains(tinyorderid)).ToArray();
                data = data.Union(linq).ToArray();
            }

            //去除重复数据
            data = data.Distinct().ToArray();

            return data;
        }
    }

    /// <summary>
    /// 发票详情信息
    /// </summary>
    public class XDTOrderInvoiceInfo : IUnique
    {
        #region 属性
        /// <summary>
        /// 发票详情主键
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 发票类型
        /// 0：全额发票;1: 服务费发票
        /// </summary>
        public XDTInvoiceType InvoiceType { get; set; }

        /// <summary>
        /// 发票通知ID
        /// </summary>
        public string InvoiceNoticeID { get; set; }

        /// <summary>
        /// 小订单号
        /// </summary>
        public string TinyOrderID { get; set; }

        /// <summary>
        /// 订单项ID
        /// </summary>
        public string OrderItemID { get; set; }

        /// <summary>
        /// 税则名称
        /// </summary>
        public string TaxName { get; set; }

        /// <summary>
        /// 开票总金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 发票号
        /// </summary>
        public string InvoiceNo { get; set; }

        /// <summary>
        /// 发票状态
        /// </summary>
        public XDTInvoiceNoticeStatus InvoiceNoticeStatus { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public decimal? Quantity { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string UnitName { get; set; }

        /// <summary>
        /// 税率
        /// </summary>
        public decimal? InvoiceTaxRate { get; set; }

        /// <summary>
        /// 开票金额
        /// </summary>
        public decimal? Price { get; set; }

        /// <summary>
        /// 税额
        /// </summary>
        public decimal? TaxPrice { get; set; }
        #endregion
    }


    /// <summary>
    /// 开票类型
    /// </summary>
    public enum XDTInvoiceType
    {
        /// <summary>
        /// 全额发票
        /// </summary>
        [Description("全额发票")]
        Full = 0,

        /// <summary>
        /// 服务费发票
        /// </summary>
        [Description("服务费发票")]
        Service = 1,
    }


    /// <summary>
    /// 开票通知状态
    /// </summary>
    public enum XDTInvoiceNoticeStatus
    {
        [Description("待开票")]
        UnAudit = 1,

        [Description("开票中")]
        Auditing = 2,

        [Description("已完成")]
        Confirmed = 3,

        [Description("已取消")]
        Canceled = 4
    }
}
