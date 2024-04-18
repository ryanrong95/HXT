using Layers.Data.Sqls;
using Yahv.Underly.Erps;
using Yahv.Underly.Logs;

namespace Yahv.Systematic
{
    public class Srm : IAction
    {
        IErpAdmin admin;
        /// <summary>
        /// 构造函数
        /// </summary>
        public Srm(IErpAdmin admin)
        {
            this.admin = admin;
        }

        #region Views
        /// <summary>
        /// 所有管理员
        /// </summary>
        public YaHv.Csrm.Services.Views.Rolls.AdminsAppendRoll Admins
        {
            get
            {
                return new YaHv.Csrm.Services.Views.Rolls.AdminsAppendRoll();
            }
        }
        /// <summary>
        /// 所有公司
        /// </summary>
        public YaHv.Csrm.Services.Views.Rolls.EnterprisesRoll Enterprises
        {
            get
            {
                return new YaHv.Csrm.Services.Views.Rolls.EnterprisesRoll();
            }
        }
        /// <summary>
        /// 所有供应商
        /// </summary>
        public YaHv.Csrm.Services.Views.Rolls.TradingSuppliersRoll Suppliers
        {
            get
            {
                return new YaHv.Csrm.Services.Views.Rolls.TradingSuppliersRoll();
            }
        }
        /// <summary>
        /// 我的供应商
        /// </summary>
        public YaHv.Csrm.Services.Views.Rolls.MyTradingSuppliersRoll MySuppliers
        {
            get
            {
                return new YaHv.Csrm.Services.Views.Rolls.MyTradingSuppliersRoll(this.admin);
            }
        }

        /// <summary>
        /// 品牌
        /// </summary>
        public YaHv.Csrm.Services.Views.Rolls.ManufacturersRoll Manufacturers
        {
            get
            {
                return new YaHv.Csrm.Services.Views.Rolls.ManufacturersRoll();
            }
        }
        /// <summary>
        /// 型号
        /// </summary>
        public YaHv.Csrm.Services.Views.Rolls.PartNumbersRoll PartNumbers
        {
            get
            {
                return new YaHv.Csrm.Services.Views.Rolls.PartNumbersRoll();
            }
        }
        /// <summary>
        /// 我的代仓储供应商
        /// </summary>
        //public YaHv.Csrm.Services.Views.Rolls.MyWsSuppliersRoll MyWsSuppliers
        //{
        //    get
        //    {
        //        return new YaHv.Csrm.Services.Views.Rolls.MyWsSuppliersRoll(this.admin);
        //    }
        //}
        /// <summary>
        /// 代仓储供应商
        /// </summary>
        //public YaHv.Csrm.Services.Views.Rolls.WsSuppliersRoll WsSuppliers
        //{
        //    get
        //    {
        //        return new YaHv.Csrm.Services.Views.Rolls.WsSuppliersRoll();
        //    }
        //}
        #endregion

        #region Action
        /// <summary>
        /// 写入错误日志
        /// </summary>
        /// <param name="log"></param>
        public void Logs_Error(Logs_Error log)
        {
            using (var repository = new PvbCrmReponsitory())
            {
                repository.Insert<Layers.Data.Sqls.PvbCrm.Logs_Errors>(new Layers.Data.Sqls.PvbCrm.Logs_Errors
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
