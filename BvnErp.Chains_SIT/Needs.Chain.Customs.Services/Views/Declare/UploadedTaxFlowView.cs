using Layer.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 已上传缴费流水列表 视图
    /// </summary>
    public class UploadedTaxFlowListView
    {
        private ScCustomsReponsitory Reponsitory;

        public UploadedTaxFlowListView()
        {
            this.Reponsitory = new ScCustomsReponsitory();
        }

        public UploadedTaxFlowListView(ScCustomsReponsitory reponsitory)
        {
            this.Reponsitory = reponsitory;
        }

        /// <summary>
        /// 已上传缴费流水列表 Model
        /// </summary>
        public class UploadedTaxFlowListModel
        {
            /// <summary>
            /// DecTaxID
            /// </summary>
            public string DecTaxID { get; set; } = string.Empty;

            /// <summary>
            /// DecHeadID
            /// </summary>
            public string DecHeadID { get; set; } = string.Empty;

            /// <summary>
            /// 关税税费单号
            /// </summary>
            public string TariffTaxNumber { get; set; } = string.Empty;

            /// <summary>
            /// 关税金额
            /// </summary>
            public string TariffAmount { get; set; } = string.Empty;

            /// <summary>
            /// 消费税税费单号
            /// </summary>
            public string ExciseTaxNumber { get; set; } = string.Empty;

            /// <summary>
            /// 消费税金额
            /// </summary>
            public string ExciseTaxAmount { get; set; } = string.Empty;

            /// <summary>
            /// 增值税税费单号
            /// </summary>
            public string AddedValueTaxTaxNumber { get; set; } = string.Empty;

            /// <summary>
            /// 增值税金额
            /// </summary>
            public string AddedValueTaxAmount { get; set; } = string.Empty;

            /// <summary>
            /// 合同号
            /// </summary>
            public string ContrNo { get; set; } = string.Empty;

            /// <summary>
            /// 海关编号
            /// </summary>
            public string EntryId { get; set; } = string.Empty;

            /// <summary>
            /// 报关日期
            /// </summary>
            public DateTime? DDate { get; set; }

            /// <summary>
            /// 公司名称
            /// </summary>
            public string OwnerName { get; set; } = string.Empty;

            /// <summary>
            /// 订单编号
            /// </summary>
            public string OrderID { get; set; } = string.Empty;

            /// <summary>
            /// 类型
            /// </summary>
            public int DecTaxFlowType { get; set; }

            public string TaxNumber { get; set; } = string.Empty;

            public decimal Amount { get; set; }
        }

        private IEnumerable<UploadedTaxFlowListModel> GetDecTaxList(params LambdaExpression[] expressions)
        {
            //避免枚举中增加了值而引发错误，所以不遍历枚举，直接直接用具体值或了
            int allHandledTypeEnumsValue = Enums.HandledType.Tariff.GetHashCode()
                                         | Enums.HandledType.AddedValueTax.GetHashCode()
                                         | Enums.HandledType.ExciseTax.GetHashCode();

            var decTaxs = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxs>().Where(t => t.HandledType == allHandledTypeEnumsValue);
            var decHeads = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>();

            var linq = from decTax in decTaxs
                       join decHead in decHeads
                            on new
                            {
                                DecTaxID = decTax.ID,
                                //DecTaxIsUpload = decTax.IsUpload,
                            }
                            equals new
                            {
                                DecTaxID = decHead.ID,
                                //DecTaxIsUpload = (int)Enums.UploadStatus.Uploaded,
                            }
                       orderby decHead.DDate descending
                       select new UploadedTaxFlowListModel()
                       {
                           DecTaxID = decTax.ID,
                           DecHeadID = decHead.ID,
                           ContrNo = decHead.ContrNo,
                           EntryId = decHead.EntryId,
                           DDate = decHead.DDate,
                           OwnerName = decHead.OwnerName,
                           OrderID = decHead.OrderID,
                       };

            foreach (var expression in expressions)
            {
                linq = linq.Where(expression as Expression<Func<UploadedTaxFlowListModel, bool>>);
            }

            return linq;
        }

        private IEnumerable<UploadedTaxFlowListModel> GetDecTaxFlowsList(IEnumerable<string> decTaxIDs)
        {
            var decTaxFlows = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxFlows>();

            var linq = from decTaxFlow in decTaxFlows
                       where decTaxIDs.Contains(decTaxFlow.DecTaxID)
                       select new UploadedTaxFlowListModel()
                       {
                           DecTaxID = decTaxFlow.DecTaxID,
                           DecTaxFlowType = decTaxFlow.TaxType,
                           TaxNumber = decTaxFlow.TaxNumber,
                           Amount = decTaxFlow.Amount,
                       };

            return linq;
        }

        public IEnumerable<UploadedTaxFlowListModel> GetResult(out int totalCount, int pageIndex, int pageSize, params LambdaExpression[] expressions)
        {
            var decTaxList = GetDecTaxList(expressions);

            totalCount = decTaxList.Count();

            decTaxList = decTaxList.Skip(pageSize * (pageIndex - 1)).Take(pageSize);

            var decTaxFlowsList = GetDecTaxFlowsList(decTaxList.Select(t => t.DecTaxID));

            var result = from decTax in decTaxList

                         join decTaxFlows in decTaxFlowsList
                            on new { DecTaxID = decTax.DecTaxID, DecTaxType = (int)Enums.DecTaxType.Tariff, }
                            equals new { DecTaxID = decTaxFlows.DecTaxID, DecTaxType = decTaxFlows.DecTaxFlowType, }
                            into TariffDecTaxFlowsList
                         from tariffDecTaxFlows in TariffDecTaxFlowsList.DefaultIfEmpty()

                         join decTaxFlows in decTaxFlowsList
                            on new { DecTaxID = decTax.DecTaxID, DecTaxType = (int)Enums.DecTaxType.ExciseTax, }
                            equals new { DecTaxID = decTaxFlows.DecTaxID, DecTaxType = decTaxFlows.DecTaxFlowType, }
                            into ExciseTaxDecTaxFlowsList
                         from exciseTaxDecTaxFlows in ExciseTaxDecTaxFlowsList.DefaultIfEmpty()

                         join decTaxFlows in decTaxFlowsList
                            on new { DecTaxID = decTax.DecTaxID, DecTaxType = (int)Enums.DecTaxType.AddedValueTax, }
                            equals new { DecTaxID = decTaxFlows.DecTaxID, DecTaxType = decTaxFlows.DecTaxFlowType, }
                            into AddedValueTaxDecTaxFlowsList
                         from addedValueTaxDecTaxFlows in AddedValueTaxDecTaxFlowsList.DefaultIfEmpty()

                         select new UploadedTaxFlowListModel()
                         {
                             DecTaxID = decTax.DecTaxID,
                             DecHeadID = decTax.DecHeadID,
                             ContrNo = decTax.ContrNo,
                             EntryId = decTax.EntryId,
                             DDate = decTax.DDate,
                             OwnerName = decTax.OwnerName,
                             OrderID = decTax.OrderID,
                             TariffTaxNumber = tariffDecTaxFlows != null ? tariffDecTaxFlows.TaxNumber : "-",
                             TariffAmount = tariffDecTaxFlows != null ? Convert.ToString(tariffDecTaxFlows.Amount) : "-",
                             ExciseTaxNumber = exciseTaxDecTaxFlows != null ? exciseTaxDecTaxFlows.TaxNumber : "-",
                             ExciseTaxAmount = exciseTaxDecTaxFlows != null ? Convert.ToString(exciseTaxDecTaxFlows.Amount) : "-",
                             AddedValueTaxTaxNumber = addedValueTaxDecTaxFlows != null ? addedValueTaxDecTaxFlows.TaxNumber : "-",
                             AddedValueTaxAmount = addedValueTaxDecTaxFlows != null ? Convert.ToString(addedValueTaxDecTaxFlows.Amount) : "-",
                         };

            return result;
        }
    }
}
