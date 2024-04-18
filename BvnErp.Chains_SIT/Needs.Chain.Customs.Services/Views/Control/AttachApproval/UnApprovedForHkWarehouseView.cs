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
    /// 香港库房查询一个订单是否有未审批的情况（删除型号、修改数量、拆分订单）
    /// </summary>
    public class UnApprovedForHkWarehouseView
    {
        private ScCustomsReponsitory Reponsitory { get; set; }

        public UnApprovedForHkWarehouseView()
        {
            this.Reponsitory = new ScCustomsReponsitory();
        }

        public UnApprovedForHkWarehouseView(ScCustomsReponsitory reponsitory)
        {
            this.Reponsitory = reponsitory;
        }

        public List<UnApprovedForHkWarehouseViewModel> GetResult(LambdaExpression[] expressions)
        {
            var orderControls = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.OrderControls>();
            var orderControlSteps = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.OrderControlSteps>();

            Enums.OrderControlType[] preventControlTypes =
            {
                Enums.OrderControlType.DeleteModelApproval,
                Enums.OrderControlType.ChangeQuantityApproval,
                Enums.OrderControlType.SplitOrderApproval,
            };
            int[] preventControlTypesInt = preventControlTypes.Select(t => (int)t).ToArray();

            var results = from orderControl in orderControls
                          join orderControlStep in orderControlSteps on orderControl.ID equals orderControlStep.OrderControlID
                          where orderControl.Status == (int)Enums.Status.Normal
                             && orderControlStep.Status == (int)Enums.Status.Normal
                             && preventControlTypesInt.Contains(orderControl.ControlType)
                             && orderControlStep.ControlStatus == (int)Enums.OrderControlStatus.Auditing
                          select new UnApprovedForHkWarehouseViewModel
                          {
                              OrderControlID = orderControl.ID,
                              OrderControlStepID = orderControlStep.ID,
                              MainOrderID = orderControl.MainOrderID,
                              TinyOrderID = orderControl.OrderID,
                              OrderItemID = orderControl.OrderItemID,
                              ControlType = (Enums.OrderControlType)orderControl.ControlType,
                          };

            foreach (var expression in expressions)
            {
                results = results.Where(expression as Expression<Func<UnApprovedForHkWarehouseViewModel, bool>>);
            }

            return results.ToList();
        }
    }

    public class UnApprovedForHkWarehouseViewModel
    {
        public string OrderControlID { get; set; } = string.Empty;

        public string OrderControlStepID { get; set; } = string.Empty;

        public string MainOrderID { get; set; } = string.Empty;

        public string TinyOrderID { get; set; } = string.Empty;

        public string OrderItemID { get; set; } = string.Empty;

        public Enums.OrderControlType ControlType { get; set; }
    }
}
