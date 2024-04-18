using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Linq.Generic;
using Yahv.Underly;
using Yahv.Underly.Attributes;

namespace Yahv.PvWsOrder.Services.XDTClientView
{
    /// <summary>
    /// 海关关税、增值税缴税报表
    /// </summary>
    public class ClientCustomsTaxReportView : UniqueView<CustomsTaxReport, ScCustomReponsitory>
    {
        private IUser user;

        public ClientCustomsTaxReportView(IUser User)
        {
            this.user = User;
        }

        protected override IQueryable<CustomsTaxReport> GetIQueryable()
        {
            var orders = this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.Orders>().Where(item => item.ClientID == user.XDTClientID);
            //if (!user.IsMain)
            //{
            //    orders = orders.Where(item => item.AdminID == this.user.ID);
            //}
            var dectaxs = this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.DecTaxs>().Where(item => item.InvoiceType == 1);

            var linq = from order in orders
                       join decHead in this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.DecHeads>() on order.ID equals decHead.OrderID
                       join decTax in dectaxs on decHead.ID equals decTax.ID
                       join decTaxFlow in this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.DecTaxFlows>() on decTax.ID equals decTaxFlow.DecTaxID
                       select new CustomsTaxReport
                       {
                           ID = decTaxFlow.ID,
                           DecTaxID = decTaxFlow.DecTaxID,
                           OrderID = decHead.OrderID,
                           ContractNo = decHead.ContrNo,
                           Amount = decTaxFlow.Amount,
                           TaxNumber = decTaxFlow.TaxNumber,
                           TaxType = (DecTaxType)decTaxFlow.TaxType,
                           PayDate = decTaxFlow.PayDate,
                           CreateDate = decTaxFlow.CreateDate
                       };
            return linq;
        }
    }

    /// <summary>
    /// 缴费流水
    /// </summary>
    public class ClientTaxRecordsView : UniqueView<DecTaxFlowForUser, ScCustomReponsitory>
    {
        private IUser user;

        public ClientTaxRecordsView(IUser User)
        {
            this.user = User;
        }

        protected override IQueryable<DecTaxFlowForUser> GetIQueryable()
        {
            var decheadfiles = this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.DecHeadFiles>().Where(item => item.Status == 200);

            var linq = from decTaxFlow in this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.DecTaxFlows>()
                       join file in decheadfiles on decTaxFlow.DecTaxID equals file.DecHeadID into files
                       select new DecTaxFlowForUser
                       {
                           ID = decTaxFlow.ID,
                           DecTaxID = decTaxFlow.DecTaxID,
                           Amount = decTaxFlow.Amount,
                           TaxNumber = decTaxFlow.TaxNumber,
                           TaxType = (DecTaxType)decTaxFlow.TaxType,
                           PayDate = decTaxFlow.PayDate,
                           CreateDate = decTaxFlow.CreateDate,
                           files = files.Select(item => new DecHeadFile
                           {
                               ID = item.ID,
                               DecHeadID = item.DecHeadID,
                               FileType = item.FileType,
                               Url = item.Url,
                               Name = item.Name,
                           }).ToArray(),
                       };
            return linq;
        }

        /// <summary>
        /// 根据传入参数获取数据
        /// </summary>
        /// <param name="expressions"></param>
        /// <returns></returns>
        virtual protected IQueryable<DecTaxFlowForUser> GetExpressionData(LambdaExpression[] expressions)
        {
            var results = this.GetIQueryable();
            if (expressions != null)
            {
                foreach (var expression in expressions)
                {
                    results = results.Where(expression as Expression<Func<DecTaxFlowForUser, bool>>);
                }
            }

            var decheads = this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.DecHeads>();
            var dectaxs = this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.DecTaxs>();
            var orders = this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.Orders>().Where(item => item.ClientID == user.XDTClientID);
            if (!user.IsMain)
            {
                orders = orders.Where(item => item.AdminID == user.ID);
            }

            var linq = from result in results
                       join dectax in dectaxs on result.DecTaxID equals dectax.ID
                       join dechead in decheads on dectax.ID equals dechead.ID
                       join order in orders on dechead.OrderID equals order.ID
                       select result;

            return linq;
        }
    }

    /// <summary>
    /// 查询客户 未付汇视图
    /// </summary>
    public class ClientUnPayexchangeView : UniqueView<XDTOrder, ScCustomReponsitory> {
        private IUser user;

        public ClientUnPayexchangeView(IUser User)
        {
            this.user = User;
        }

        protected override IQueryable<XDTOrder> GetIQueryable()
        {
            var orders = this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.Orders>().Where(item => item.ClientID == user.XDTClientID);

            var exccddate = DateTime.Now.AddDays(-180);

            var linq = from order in orders
                       join decHead in this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.DecHeads>() on order.ID equals decHead.OrderID
                       where (order.PaidExchangeAmount + 5 ) < order.DeclarePrice
                       && decHead.DDate < exccddate
                       && order.Type == 200
                       select new XDTOrder
                       {
                           ID = order.ID,
                           Type = order.Type,
                           ClientID = order.ClientID,
                       };
            return linq;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool CanDownLoadTaxFiles() 
        {
            var peFlag = true;
            var client = this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.Clients>().FirstOrDefault(item => item.ID == user.XDTClientID);

            //if (client != null && !client.IsDownloadDecTax.Value)
            //{
            //    if (!string.IsNullOrEmpty(client.DecTaxExtendDate))
            //    {
            //        //有宽限日期
            //        var extend = DateTime.Parse(client.DecTaxExtendDate);
            //        peFlag = DateTime.Compare(extend, DateTime.Now) > 0;
            //    }
            //    else
            //    {
            //        //没有宽限日期
            //        peFlag = false;
            //    }
            //}

            if (client != null && client.UnPayExchangeAmount < 500000)
            {
                peFlag = true;
            }
            else
            {
                peFlag = false;
            }

            return peFlag;
        }
    }

    public class DecTaxFlowForUser : IUnique
    {
        public string ID { get; set; }

        public string DecTaxID { get; set; }

        public string DecheadID { get; set; }

        public string ContrNo { get; set; }

        public string TaxNumber { get; set; }

        public DecTaxType TaxType { get; set; }

        public decimal Amount { get; set; }

        public DateTime? PayDate { get; set; }

        public string BankName { get; set; }

        public DateTime? DeductionTime { get; set; }

        public DecTaxStatus Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

        public string OrderID { get; set; }

        public string EntryID { get; set; }

        public string ClientID { get; set; }

        public string UserID { get; set; }


        public DecHeadFile[] files { get; set; }
    }

    public class CustomsTaxReport : IUnique
    {
        public string ID { get; set; }

        /// <summary>
        /// 报关单ID 
        /// DecHeadID
        /// TODO:数据库中修改字段名称 DecHeadID
        /// </summary>
        public string DecTaxID { get; set; }

        /// <summary>
        /// 税费单号
        /// </summary>
        public string TaxNumber { get; set; }

        /// <summary>
        /// 税费类型
        /// </summary>
        public DecTaxType TaxType { get; set; }

        /// <summary>
        /// 支付金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 缴税日期
        /// </summary>
        public DateTime? PayDate { get; set; }

        /// <summary>
        /// 缴税银行
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// 海关扣款时间
        /// </summary>
        public DateTime? DeductionTime { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 合同号
        /// </summary>
        public string ContractNo { get; set; }

        public DateTime CreateDate { get; set; }
    }

    /// <summary>
    /// 报关文件
    /// </summary>
    public class DecHeadFile
    {
        public string ID { get; set; }

        public string DecHeadID { get; set; }

        public int FileType { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }
    }

    /// <summary>
    /// 报关单缴税状态
    /// </summary>
    public enum DecTaxStatus
    {
        [Description("未缴税")]
        Unpaid = 1,

        [Description("已缴税")]
        Paid = 2,

        [Description("已抵扣")]
        Deducted = 3,

    }

    /// <summary>
    /// 报关单税费类型
    /// </summary>
    public enum DecTaxType
    {
        [Description("进口关税")]
        Tariff = 1,

        [Description("进口增值税")]
        AddedValueTax = 2,

        [Description("消费税")]
        ConsumptionTax = 3,
    }
}
