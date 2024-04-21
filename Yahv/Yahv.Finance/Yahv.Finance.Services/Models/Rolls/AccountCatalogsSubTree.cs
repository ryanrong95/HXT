using System.Collections.Generic;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Finance.Services.Models.Rolls
{
    /// <summary>
    /// 账款分类
    /// </summary>
    public class nAccountCatalog : TreeNode<nAccountCatalog>
    {
        new internal string FatherID { get { return base.FatherID; } set { base.FatherID = value; } }

        /// <summary>
        /// 状态
        /// </summary>
        public GeneralStatus Status { get; set; }
    }

    public class nAccountCatalogs : SubTree<nAccountCatalog>
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param> 
        public nAccountCatalogs(IEnumerable<nAccountCatalog> data) : base(data)
        {

        }
    }
}