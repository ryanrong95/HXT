using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PsWms.DappApi.Services.Enums;
using Yahv.PsWms.DappApi.Services.Models;
using Yahv.Underly;

namespace Yahv.PsWms.DappApi.Services.Views
{
    /// <summary>
    /// 费用视图
    /// </summary>
    public class ChargesView : UniqueView<Charges, PsWmsRepository>
    {
        #region 构造函数

        public ChargesView()
        {
        }

        public ChargesView(PsWmsRepository reponsitory) : base(reponsitory)
        {
        }

        public ChargesView(PsWmsRepository reponsitory, IQueryable<Charges> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        #endregion

        protected override IQueryable<Charges> GetIQueryable()
        {
            var Pay = from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.PayerLefts>()
                      select new Charges
                      {
                          ID = entity.ID,
                          Source = (AccountSource)entity.Source,
                          PayerID = entity.PayerID,
                          PayeeID = entity.PayeeID,
                          TakerID = entity.TakerID,
                          Conduct = (Conduct)entity.Conduct,
                          Subject = entity.Subject,
                          Currency = (Currency)entity.Currency,
                          Quantity = entity.Quantity,
                          UnitPrice = entity.UnitPrice,
                          Unit = entity.Unit,
                          Total = entity.Total,
                          CreateDate = entity.CreateDate,
                          NoticeID = entity.NoticeID,
                          FormID = entity.FormID,
                          WaybillCode = entity.WaybillCode,
                          AdminID = entity.AdminID,
                          Type = ChargeType.Pay
                      };
            var income = from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.PayeeLefts>()
                         select new Charges
                         {
                             ID = entity.ID,
                             Source = (AccountSource)entity.Source,
                             PayerID = entity.PayerID,
                             PayeeID = entity.PayeeID,
                             TakerID = "",
                             Conduct = (Conduct)entity.Conduct,
                             Subject = entity.Subject,
                             Currency = (Currency)entity.Currency,
                             Quantity = entity.Quantity,
                             UnitPrice = entity.UnitPrice,
                             Unit = entity.Unit,
                             Total = entity.Total,
                             CreateDate = entity.CreateDate,
                             NoticeID = entity.NoticeID,
                             FormID = entity.FormID,
                             WaybillCode = entity.WaybillCode,
                             AdminID = entity.AdminID,
                             Type = ChargeType.Income,
                         };

            return Pay.Union(income);
        }

        #region 搜索方法

        /// <summary>
        /// 根据通知ID来搜索
        /// </summary>
        /// <param name="noticeID"></param>
        /// <returns></returns>
        public ChargesView SearchByNoticeID(string noticeID)
        {
            var view = this.IQueryable.Cast<Charges>();
            var linq = from payerLeft in view
                       where payerLeft.NoticeID == noticeID
                       select payerLeft;
            var resultView = new ChargesView(this.Reponsitory, linq)
            {
            };

            return resultView;
        }

        /// <summary>
        /// 根据订单ID搜索
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public ChargesView SearchByFormID(string orderID)
        {
            var view = this.IQueryable.Cast<Charges>();
            var linq = from payerLeft in view
                       where payerLeft.FormID == orderID
                       select payerLeft;
            var resultView = new ChargesView(this.Reponsitory, linq)
            {
            };

            return resultView;
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
