using Yahv.Underly.Erps;
using Yahv.Underly.Logs;
using Layers.Data.Sqls;

namespace Yahv.Systematic
{
    /// <summary>
    /// 询报价领域
    /// </summary>
    public class RFQ : IAction
    {
        IErpAdmin admin;
        /// <summary>
        /// 构造函数
        /// </summary>
        public RFQ(IErpAdmin admin)
        {
            this.admin = admin;
        }

        #region Views

        ///// <summary>
        ///// 采购单(陈翰修改)
        ///// </summary>
        //public Yahv.RFQ.Services.Views.MyPurchasesView Purchases_chenhan
        //{
        //    get
        //    {
        //        return new Yahv.RFQ.Services.Views.MyPurchasesView(this.admin);
        //    }
        //}

        ///// <summary>
        ///// 询报价业务下的管理员
        ///// </summary>
        //public Yahv.RFQ.Services.AdminsView Admins
        //{
        //    get
        //    {
        //        return new Yahv.RFQ.Services.AdminsView();
        //    }
        //}

        ///// <summary>
        ///// 我的优势型号
        ///// </summary>
        //public Yahv.RFQ.Services.Views.Rolls.AdvantagePNOsRoll AdvantagePNOs
        //{
        //    get
        //    {
        //        return new Yahv.RFQ.Services.Views.Rolls.AdvantagePNOsRoll(this.admin);
        //    }
        //}

        ///// <summary>
        ///// 我的积压库存
        ///// </summary>
        //public Yahv.RFQ.Services.Views.Rolls.OverStocksRoll OverStocks
        //{
        //    get
        //    {
        //        return new Yahv.RFQ.Services.Views.Rolls.OverStocksRoll(this.admin);
        //    }
        //}

        #endregion

        #region 其它通用视图
        public Yahv.Services.Views.WarehousesTopView<HvRFQReponsitory> Warehouses
        {
            get
            {
                return new Yahv.Services.Views.WarehousesTopView<HvRFQReponsitory>();
            }
        }
        #endregion

        #region Action
        /// <summary>
        /// 写入错误日志
        /// </summary>
        /// <param name="log"></param>
        public void Logs_Error(Logs_Error log)
        {
            using (var repository = new HvRFQReponsitory())
            {
                repository.Insert<Layers.Data.Sqls.HvRFQ.Logs_Errors>(new Layers.Data.Sqls.HvRFQ.Logs_Errors
                {
                    AdminID = admin.ID,
                    Page = log.Page,
                    Message = log.Message,
                    Codes = log.Codes,
                    Source = log.Source,
                    Stack = log.Stack,
                    CreateDate = log.CreateDate
                });
            }
        }
        #endregion

    }
}
