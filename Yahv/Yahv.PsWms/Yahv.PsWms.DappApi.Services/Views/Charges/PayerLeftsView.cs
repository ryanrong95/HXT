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
    /// 应付视图
    /// </summary>
    public class PayerLeftsView : UniqueView<PayerLeft, PsWmsRepository>
    {
        #region 构造函数

        public PayerLeftsView()
        {
        }

        public PayerLeftsView(PsWmsRepository reponsitory) : base(reponsitory)
        {
        }

        public PayerLeftsView(PsWmsRepository reponsitory, IQueryable<PayerLeft> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        #endregion

        protected override IQueryable<PayerLeft> GetIQueryable()
        {
            var view = from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.PayerLefts>()
                       select new PayerLeft
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
                           CutDate = entity.CutDate,
                           CutDateIndex = entity.CutDateIndex,
                           NoticeID = entity.NoticeID,
                           FormID = entity.FormID,
                           WaybillCode = entity.WaybillCode,
                           AdminID = entity.AdminID,
                       };

            return view;
        }

        #region 搜索方法
        /// <summary>
        /// 根据通知ID来搜索
        /// </summary>
        /// <param name="noticeID"></param>
        /// <returns></returns>
        public PayerLeftsView SearchByNoticeID(string noticeID)
        {
            var view = this.IQueryable.Cast<PayerLeft>();
            var linq = from payerLeft in view
                       where payerLeft.NoticeID == noticeID
                       select payerLeft;
            var resultView = new PayerLeftsView(this.Reponsitory, linq)
            {
            };

            return resultView;
        }

        /// <summary>
        /// 根据订单ID搜索
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public PayerLeftsView SearchByFormID(string orderID)
        {
            var view = this.IQueryable.Cast<PayerLeft>();
            var linq = from payerLeft in view
                       where payerLeft.FormID == orderID
                       select payerLeft;
            var resultView = new PayerLeftsView(this.Reponsitory, linq)
            {
            };

            return resultView;
        }
        #endregion
    }
}
