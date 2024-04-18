using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class AccountCatalog : IUnique
    {           
        #region 属性
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 父节点
        /// </summary>
        public string FatherID { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        public string CreatorID { get; set; }

        /// <summary>
        /// 修改人ID
        /// </summary>
        public string ModifierID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifyDate { get; set; }

        /// <summary>
        /// 状态 启用、停用
        /// </summary>
        public GeneralStatus Status { get; set; }
        #endregion

       
       
    }
}
