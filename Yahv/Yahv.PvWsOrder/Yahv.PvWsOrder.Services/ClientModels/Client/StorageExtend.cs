using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.ClientModels
{
    public class StorageExtend : _Storage
    {
        #region 拓展属性
        /// <summary>
        /// 单位
        /// </summary>
        public LegalUnit Unit { get; set; }
        #endregion
    }
}
