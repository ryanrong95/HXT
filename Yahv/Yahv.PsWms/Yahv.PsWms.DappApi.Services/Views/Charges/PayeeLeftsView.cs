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
    /// 应收视图
    /// </summary>
    public class PayeeLeftsView : UniqueView<PayeeLeft, PsWmsRepository>
    {
        #region 构造函数

        public PayeeLeftsView()
        {
        }

        public PayeeLeftsView(PsWmsRepository reponsitory) : base(reponsitory)
        {
        }

        public PayeeLeftsView(PsWmsRepository reponsitory, IQueryable<PayeeLeft> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        #endregion

        protected override IQueryable<PayeeLeft> GetIQueryable()
        {
            var view = from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.PayeeLefts>()
                       select new PayeeLeft
                       {
                           ID = entity.ID,
                           Source = (AccountSource)entity.Source,
                           PayerID = entity.PayerID,
                           PayeeID = entity.PayeeID,
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
        public PayeeLeftsView SearchByNoticeID(string noticeID)
        {
            var view = this.IQueryable.Cast<PayeeLeft>();
            var linq = from payeeLeft in view
                       where payeeLeft.NoticeID == noticeID
                       select payeeLeft;
            var resultView = new PayeeLeftsView(this.Reponsitory, linq)
            {
            };

            return resultView;
        }

        /// <summary>
        /// 根据订单ID搜索
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public PayeeLeftsView SearchByFormID(string orderID)
        {
            var view = this.IQueryable.Cast<PayeeLeft>();
            var linq = from payeeLeft in view
                       where payeeLeft.FormID == orderID
                       select payeeLeft;
            var resultView = new PayeeLeftsView(this.Reponsitory, linq)
            {
            };

            return resultView;
        }
        #endregion
    }
}
