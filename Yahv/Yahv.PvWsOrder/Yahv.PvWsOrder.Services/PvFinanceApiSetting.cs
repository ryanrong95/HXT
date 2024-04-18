using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvWsOrder.Services
{
    /// <summary>
    /// Api接口设置
    /// </summary>
    public class PvFinanceApiSetting
    {
        /// <summary>
        /// 子系统接口地址
        /// </summary>
        public string ApiName { get; private set; }

        /// <summary>
        /// 新增核销记录
        /// </summary>
        public string PayeeRightEnter { get; private set; }

        /// <summary>
        /// 新增核销记录
        /// </summary>
        public string PayForGoodsEnter { get; private set; }

        public PvFinanceApiSetting()
        {
            ApiName = "FinanceApiUrl";
            PayeeRightEnter = "Payee/PayeeRightEnter";
            PayForGoodsEnter = "Apply/PayForGoodsEnter";
        }
    }


}
