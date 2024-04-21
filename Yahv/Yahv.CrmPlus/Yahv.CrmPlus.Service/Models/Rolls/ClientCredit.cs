using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Usually;
using YaHv.CrmPlus.Services.Models.Origins;

namespace Yahv.CrmPlus.Service.Models.Origins.Rolls
{
    public class ClientCredit : Credit
    {
        #region 属性
        /// <summary>
        /// 名称
        /// </summary>
        public Enterprise Client { set; get; }
        /// <summary>
        /// 申请人姓名
        /// </summary>
        public Enterprise Company { set; get; }
        #endregion

        #region 事件
        public event SuccessHanlder EnterSuccess;
        #endregion

        #region 持久化
        public void Enter()
        {

        }
        #endregion

    }
}
