using Layer.Data.Sqls;
using Needs.Ccs.Services.Enums;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class ReceivingImportQueryView : UniqueView<Models.ReImportModel, ScCustomsReponsitory>
    {
        public ReceivingImportQueryView()
        {
        }

        internal ReceivingImportQueryView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected ReceivingImportQueryView(ScCustomsReponsitory reponsitory, IQueryable<Models.ReImportModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<Models.ReImportModel> GetIQueryable()
        {
            var iQuery = from reimp in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderReceiptImport>()
                         where reimp.Status == (int)Status.Normal
                         select new Models.ReImportModel
                         {
                            ID = reimp.ID,
                            RequestID = reimp.RequestID,
                            OrderRecepitID = reimp.OrderReceiptID,
                            PreMoney = reimp.PreMoney,
                            Diff = reimp.Diff,
                            GoodsMoney = reimp.GoodsMoney,
                            ClientName = reimp.ClientName,
                            AddTax = reimp.AddTax,
                            Tariff = reimp.Tariff,
                            ExciseTax = reimp.ExciseTax,
                            Agency = reimp.Agency,
                            ReCreWord = reimp.ReCreWord,
                            ReCreNo = reimp.ReCreNo,
                            Status = (Status)reimp.Status,
                            CreateDate = reimp.CreateDate,
                            UpdateDate = reimp.UpdateDate,
                            InvoiceType = (InvoiceType)reimp.InvoiceType
                         };

            return iQuery;
        }

        public object ToMyPage(int? pageIndex = null, int? pageSize = null, bool IsStatistic = false)
        {

            IQueryable<Models.ReImportModel> iquery = this.IQueryable.Cast<Models.ReImportModel>().OrderByDescending(item => item.RequestID);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_mySubs = iquery.ToArray();


            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = ienum_mySubs
            };

        }

        /// <summary>
        /// 客户名称
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ReceivingImportQueryView SearchByClientName(string name)
        {
            var linq = from query in this.IQueryable
                       where query.ClientName.Contains(name)
                       select query;

            var view = new ReceivingImportQueryView(this.Reponsitory, linq);
            return view;
        }

    }
}
