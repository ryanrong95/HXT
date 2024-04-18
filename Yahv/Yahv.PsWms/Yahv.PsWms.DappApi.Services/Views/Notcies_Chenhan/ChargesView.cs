using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PsWms.DappApi.Services.Enums;
using Yahv.PsWms.DappApi.Services.Models;
using Yahv.PsWms.DappApi.Services.Views;
using Yahv.Underly;

namespace Yahv.PsWms.DappApi.Services.Notcies_Chenhan.Views
{
    /// <summary>
    /// 费用视图
    /// </summary>
    public class ChargesView
    {
        #region 搜索方法

        /// <summary>
        /// 根据通知ID来搜索
        /// </summary>
        /// <param name="noticeID"></param>
        /// <returns></returns>
        public Charges[] SearchByNoticeID(string noticeID)
        {
            using (var reponsitoryr = new PsWmsRepository())
            using (var payeeView = new PayeeLeftsView())
            using (var payerView = new PayerLeftsView())
            {
                var payee = payeeView.Where(item => item.NoticeID == noticeID).Select(item => new Charges
                {
                    ID = item.ID,
                    Source = item.Source,
                    PayerID = item.PayerID,
                    PayeeID = item.PayeeID,
                    TakerID = "",
                    Conduct = item.Conduct,
                    Subject = item.Subject,
                    Currency = item.Currency,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    Unit = item.Unit,
                    Total = item.Total,
                    CreateDate = item.CreateDate,
                    NoticeID = item.NoticeID,
                    FormID = item.FormID,
                    WaybillCode = item.WaybillCode,
                    AdminID = item.AdminID,
                    Type = ChargeType.Income
                }).ToArray();
                var payer = payerView.Where(item => item.NoticeID == noticeID).Select(item => new Charges
                {
                    ID = item.ID,
                    Source = item.Source,
                    PayerID = item.PayerID,
                    PayeeID = item.PayeeID,
                    TakerID = item.TakerID,
                    Conduct = item.Conduct,
                    Subject = item.Subject,
                    Currency = item.Currency,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    Unit = item.Unit,
                    Total = item.Total,
                    CreateDate = item.CreateDate,
                    NoticeID = item.NoticeID,
                    FormID = item.FormID,
                    WaybillCode = item.WaybillCode,
                    AdminID = item.AdminID,
                    Type = ChargeType.Pay
                }).ToArray();

                return payee.Union(payer).ToArray();
            }
        }

        /// <summary>
        /// 根据订单ID搜索
        /// </summary>
        /// <param name="formID"></param>
        /// <returns></returns>
        public Charges[] SearchByFormID(string formID)
        {
            using (var reponsitoryr = new PsWmsRepository())
            using (var payeeView = new PayeeLeftsView())
            using (var payerView = new PayerLeftsView())
            {
                var payee = payeeView.Where(item => item.FormID == formID).Select(item => new Charges
                {
                    ID = item.ID,
                    Source = item.Source,
                    PayerID = item.PayerID,
                    PayeeID = item.PayeeID,
                    TakerID = "",
                    Conduct = item.Conduct,
                    Subject = item.Subject,
                    Currency = item.Currency,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    Unit = item.Unit,
                    Total = item.Total,
                    CreateDate = item.CreateDate,
                    NoticeID = item.NoticeID,
                    FormID = item.FormID,
                    WaybillCode = item.WaybillCode,
                    AdminID = item.AdminID,
                    Type = ChargeType.Income
                }).ToArray();
                var payer = payerView.Where(item => item.FormID == formID).Select(item => new Charges
                {
                    ID = item.ID,
                    Source = item.Source,
                    PayerID = item.PayerID,
                    PayeeID = item.PayeeID,
                    TakerID = item.TakerID,
                    Conduct = item.Conduct,
                    Subject = item.Subject,
                    Currency = item.Currency,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    Unit = item.Unit,
                    Total = item.Total,
                    CreateDate = item.CreateDate,
                    NoticeID = item.NoticeID,
                    FormID = item.FormID,
                    WaybillCode = item.WaybillCode,
                    AdminID = item.AdminID,
                    Type = ChargeType.Pay
                }).ToArray();

                return payee.Union(payer).ToArray();
            }
        }

        #endregion
    }

    public class Charges : IUnique
    {
        #region 属性

        public string ID { get; set; }

        public AccountSource Source { get; set; }

        /// <summary>
        /// 付款人
        /// </summary>
        public string PayerID { get; set; }

        /// <summary>
        /// 收款人（公司）
        /// </summary>
        public string PayeeID { get; set; }

        /// <summary>
        /// 收款人（个人）
        /// </summary>
        public string TakerID { get; set; }

        public Conduct Conduct { get; set; }

        /// <summary>
        /// 科目
        /// </summary>
        public string Subject { get; set; }

        public Currency Currency { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 总额
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// 发生日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 发生通知ID
        /// </summary>
        public string NoticeID { get; set; }

        /// <summary>
        /// 发生单据ID
        /// </summary>
        public string FormID { get; set; }

        /// <summary>
        /// 发生运单ID
        /// </summary>
        public string WaybillCode { get; set; }

        /// <summary>
        /// 录入人
        /// </summary>
        public string AdminID { get; set; }

        public ChargeType Type { get; set; }
        #endregion
    }
}
