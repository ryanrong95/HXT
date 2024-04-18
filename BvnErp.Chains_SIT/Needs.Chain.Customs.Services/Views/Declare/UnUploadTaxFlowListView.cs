using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 报关单 - 上传缴费流水列表
    /// </summary>
    public class UnUploadTaxFlowListView : View<UnUploadTaxFlowListViewModel, ScCustomsReponsitory>
    {
        protected override IQueryable<UnUploadTaxFlowListViewModel> GetIQueryable()
        {
            //int allHandledTypeEnumsValue = 0;
            //foreach (Enums.HandledType item in Enum.GetValues(typeof(Enums.HandledType)))
            //{
            //    allHandledTypeEnumsValue += item.GetHashCode();
            //}
            //避免枚举中增加了值而引发错误，所以不遍历枚举，直接直接用具体值或了
            int allHandledTypeEnumsValue = Enums.HandledType.Tariff.GetHashCode()
                                         | Enums.HandledType.AddedValueTax.GetHashCode()
                                         | Enums.HandledType.ExciseTax.GetHashCode();

            var decTaxs = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxs>().Where(t => t.HandledType != allHandledTypeEnumsValue);
            var decHeads = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>();
            var decLists = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>();

            var resultIQuerys = from decTax in decTaxs
                                join decHead in decHeads
                                      on new
                                      {
                                          DecHeadID = decTax.ID,
                                          //IsUpload = decTax.IsUpload,
                                      }
                                      equals new
                                      {
                                          DecHeadID = decHead.ID,
                                          //IsUpload = (int)Enums.UploadStatus.NotUpload,
                                      }
                                select new UnUploadTaxFlowListViewModel
                                {
                                    DecHeadID = decHead.ID,
                                    ContrNo = decHead.ContrNo,
                                    OrderID = decHead.OrderID,
                                    EntryID = decHead.EntryId,
                                    DDate = decHead.DDate,
                                    CusDecStatus = decHead.CusDecStatus,
                                    HandledType = decTax.HandledType,
                                };



            var otherDatas = from decList in decLists
                             join resultIQuery in resultIQuerys on decList.DeclarationID equals resultIQuery.DecHeadID
                             group decList by new { decList.DeclarationID } into g
                             select new UnUploadTaxFlowListViewModel
                             {
                                 DecHeadID = g.Key.DeclarationID,
                                 Currency = g.First() == null ? string.Empty : g.First().TradeCurr,
                                 DecAmount = g.Sum(t => t.DeclTotal),
                             };

            return from resultIQuery in resultIQuerys
                   join otherData in otherDatas on resultIQuery.DecHeadID equals otherData.DecHeadID into otherDataInto
                   from otherData in otherDataInto.DefaultIfEmpty()

                   orderby resultIQuery.DDate descending, resultIQuery.DecHeadID ascending

                   select new UnUploadTaxFlowListViewModel
                   {
                       DecHeadID = resultIQuery.DecHeadID,
                       ContrNo = resultIQuery.ContrNo,
                       OrderID = resultIQuery.OrderID,
                       EntryID = resultIQuery.EntryID,
                       DDate = resultIQuery.DDate,
                       CusDecStatus = resultIQuery.CusDecStatus,
                       HandledType = resultIQuery.HandledType,

                       Currency = otherData.Currency,
                       DecAmount = otherData.DecAmount,
                   };
        }
    }

    public class UnUploadTaxFlowListViewModel : IUnique
    {
        /// <summary>
        /// 
        /// </summary>
        public string ID { get; set; } = string.Empty;

        /// <summary>
        /// DecHeadID
        /// </summary>
        public string DecHeadID { get; set; } = string.Empty;

        /// <summary>
        /// 合同号
        /// </summary>
        public string ContrNo { get; set; } = string.Empty;

        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderID { get; set; } = string.Empty;

        /// <summary>
        /// 海关编号
        /// </summary>
        public string EntryID { get; set; } = string.Empty;

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; } = string.Empty;

        /// <summary>
        /// 报关金额
        /// </summary>
        public decimal DecAmount { get; set; }

        /// <summary>
        /// 报关日期
        /// </summary>
        public DateTime? DDate { get; set; }

        /// <summary>
        /// 报关单状态
        /// </summary>
        public string CusDecStatus { get; set; } = string.Empty;

        /// <summary>
        /// DecTax 中的 处理类型(处理了关税、增值税)
        /// </summary>
        public int? HandledType { get; set; }
    }
}
