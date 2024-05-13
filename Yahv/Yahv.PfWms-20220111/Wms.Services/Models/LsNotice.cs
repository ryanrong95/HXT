using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms.Services.Extends;
using Yahv.Linq;
using Yahv.Linq.Persistence;
using Layers.Data;
using Yahv.Usually;
using Yahv.Underly;

namespace Wms.Services.Models
{
    /// <summary>
    /// 租赁通知
    /// </summary>
    public class LsNotice : Yahv.Services.Models.LsNotice
    {

        #region 扩展属性

        internal Shelves[] Shelves { get; set; }

        #endregion


    }
}
