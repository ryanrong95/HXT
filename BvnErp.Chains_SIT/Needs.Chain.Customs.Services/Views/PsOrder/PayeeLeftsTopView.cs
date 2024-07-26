using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using Needs.Underly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq.Generic;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 应收账目
    /// </summary>
    public class PayeeLeft : IUnique
    {
        #region

        public string ID { get; set; }

        public int Source { get; set; }

        /// <summary>
        /// 付款公司ID, ClientID
        /// </summary>
        public string PayerID { get; set; }

        /// <summary>
        /// 收款公司ID, 内部来自内部公司 默认：华芯通
        /// </summary>
        public string PayeeID { get; set; }

        public int Conduct { get; set; }

        /// <summary>
        /// 科目分类, 参考《联创杰国内仓储服务协议2020-12-17》-附件：收费标准
        /// </summary>
        public string Subject { get; set; }

        public int Currency { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        public decimal Total { get; set; }

        /// <summary>
        /// 发生日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 结算日期
        /// </summary>
        public DateTime? CutDate { get; set; }

        /// <summary>
        /// 结算期号, 月结账单提取的依据 [202012]
        /// </summary>
        public int? CutDateIndex { get; set; }

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

        /// <summary>
        /// VoucherID
        /// </summary>
        public string VoucherID { get; set; }

        /// <summary>
        /// 付款人名称
        /// </summary>
        public string PayerName { get; set; }
        #endregion

        /// <summary>
        /// 实收总金额
        /// </summary>
        public decimal? ReceiptTotal { get; set; }
    }
    /// <summary>
    /// 实收对象
    /// </summary>
    public class PayeeRight : IUnique
    {
        #region 属性

        public string ID { get; set; }
        public string LeftID { get; set; }
        public int Currency { get; set; }
        public decimal Price { get; set; }
        public DateTime CreateDate { get; set; }
        public string AdminID { get; set; }
        public string AdminName { get; set; }
        public string ReviewerID { get; set; }
        public string FlowFormCode { get; set; }

        #endregion
    }
    /// <summary>
    /// 应收账单列表对象
    /// </summary>
    public class PayeeLeftShow : IUnique
    {
        public string ID { get; set; }

        public string PayerName { get; set; }

        public string Currency { get; set; }

        /// <summary>
        /// 期号
        /// </summary>
        public int? CutDateIndex { get; set; }

        /// <summary>
        /// 账单总金额
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// 实收总金额
        /// </summary>
        public decimal? ReceiptTotal { get; set; }
    }

    /// <summary>
    /// 应收视图
    /// </summary>
    public class PayeeLeftsTopView : UniqueView<PayeeLeft, ScCustomsReponsitory>, IFkoView<PayeeLeft>
    {
        public PayeeLeftsTopView()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库实体</param>
        public PayeeLeftsTopView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<PayeeLeft> GetIQueryable()
        {
            var view = from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayeeLeftsTopView>()
                       select new PayeeLeft
                       {
                           ID = entity.ID,
                           Source = entity.Source,
                           PayerID = entity.PayerID,
                           PayeeID = entity.PayeeID,
                           Conduct = entity.Conduct,
                           Subject = entity.Subject,
                           Currency = entity.Currency,
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
                           PayerName = entity.PayerName,
                       };
            return view;
        }

        public IQueryable<PayeeLeft> GetIQueryableRoll()
        {
            var payeeRights = new PayeeRightsTopView(this.Reponsitory);
            var view = from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayeeLeftsTopView>()
                       join right in payeeRights on entity.ID equals right.LeftID into rights
                       select new PayeeLeft
                       {
                           ID = entity.ID,
                           Source = entity.Source,
                           PayerID = entity.PayerID,
                           PayeeID = entity.PayeeID,
                           Conduct = entity.Conduct,
                           Subject = entity.Subject,
                           Currency = entity.Currency,
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
                           PayerName = entity.PayerName,

                           ReceiptTotal = rights.Sum(t => t.Price),
                       };
            return view;
        }
    }
    
    /// <summary>
    /// 实收视图
    /// </summary>
    public class PayeeRightsTopView : UniqueView<PayeeRight, ScCustomsReponsitory>, IFkoView<PayeeRight>
    {
        public PayeeRightsTopView()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库实体</param>
        public PayeeRightsTopView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<PayeeRight> GetIQueryable()
        {
            var admins = new AdminsTopView(this.Reponsitory);
            var view = from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayeeRightsTopView>()
                       join admin in admins on entity.AdminID equals admin.ID
                       select new PayeeRight
                       {
                           ID = entity.ID,
                           LeftID = entity.LeftID,
                           Currency = entity.Currency,
                           Price = entity.Price,
                           AdminID = entity.AdminID,
                           ReviewerID = entity.ReviewerID,
                           FlowFormCode = entity.FlowFormCode,
                           CreateDate = entity.CreateDate,

                           AdminName = admin.RealName,
                       };
            return view;
        }
    }

    /// <summary>
    /// 应收账单视图
    /// </summary>
    public class PayeeLefts_Show_View : UniqueView<PayeeLeftShow, ScCustomsReponsitory>
    {
        #region 构造函数

        public PayeeLefts_Show_View()
        {
        }

        public PayeeLefts_Show_View(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        public PayeeLefts_Show_View(ScCustomsReponsitory reponsitory, IQueryable<PayeeLeftShow> iQueryable) : base(reponsitory, iQueryable)
        {

        }

        #endregion

        protected override IQueryable<PayeeLeftShow> GetIQueryable()
        {
            var payeeLefts = new PayeeLeftsTopView(this.Reponsitory);
            var payeeRights = new PayeeRightsTopView(this.Reponsitory);
            var linq = from entity in payeeLefts
                       join right in payeeRights on entity.ID equals right.LeftID into rights
                       select new PayeeLeftShow
                       {
                           ID = entity.PayerID,
                           PayerName = entity.PayerName,
                           Total = entity.Total,
                           CutDateIndex = entity.CutDateIndex,
                           ReceiptTotal = rights.Sum(t => t.Price),
                       };
            return linq;
        }

        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<PayeeLeftShow> iquery = this.IQueryable.Cast<PayeeLeftShow>();
            var linqGroup = iquery.GroupBy(x => new { x.ID, x.PayerName, x.CutDateIndex }).Select(t => new PayeeLeftShow()
            {
                ID = t.Key.ID,
                PayerName = t.Key.PayerName,
                CutDateIndex = t.Key.CutDateIndex,
                Currency = "人民币",
                Total = t.Sum(k => k.Total),
                ReceiptTotal = t.Sum(k => k.ReceiptTotal),
            });

            int total = linqGroup.Count();
            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                linqGroup = linqGroup.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            return new
            {
                total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = linqGroup.ToArray(),
            };
        }

        /// <summary>
        /// 根据付款人查询
        /// </summary>
        /// <param name="PayerName"></param>
        /// <returns></returns>
        public PayeeLefts_Show_View SearchByPayerName(string PayerName)
        {
            var linq = from query in this.IQueryable
                       where query.PayerName == PayerName
                       select query;

            var view = new PayeeLefts_Show_View(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据期号查询
        /// </summary>
        /// <param name="PayerName"></param>
        /// <returns></returns>
        public PayeeLefts_Show_View SearchByCutDateIndex(int CutDateIndex)
        {
            var linq = from query in this.IQueryable
                       where query.CutDateIndex == CutDateIndex
                       select query;

            var view = new PayeeLefts_Show_View(this.Reponsitory, linq);
            return view;
        }
    }    
}
