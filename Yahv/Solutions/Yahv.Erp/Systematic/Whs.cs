using Yahv.Underly.Erps;
using Yahv.Underly.Logs;
using Layers.Data.Sqls;
using YaHv.Csrm.Services.Views.Rolls;
using Wms.Services;

namespace Yahv.Systematic
{
    /// <summary>
    /// Crm 领域 客户关系 供应商关系
    /// </summary>
    public class Whs : IAction
    {
        IErpAdmin admin;
        /// <summary>
        /// 构造函数
        /// </summary>
        public Whs(IErpAdmin admin)
        {
            this.admin = admin;
        }


        //public WayBillServices WayBillServices
        //{
        //    get
        //    {
        //        return null;
        //        //return new WayBillServices(this.admin); 
        //    }
        //}


        #region Views
        /// <summary>
        /// 我的代仓储客户
        /// </summary>
        public YaHv.Csrm.Services.Views.Rolls.MyWsClientsRoll MyWsClients
        {
            get
            {
                return new YaHv.Csrm.Services.Views.Rolls.MyWsClientsRoll(this.admin);
            }

        }
        /// <summary>
        /// 代仓储客户
        /// </summary>
        public YaHv.Csrm.Services.Views.Rolls.WsClientsRoll WsClients
        {
            get
            {
                return new YaHv.Csrm.Services.Views.Rolls.WsClientsRoll();
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
