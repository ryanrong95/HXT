using System.Collections.Generic;
using Yahv.Linq;

namespace Yahv.Erm.Services.Models.Origins
{

    /// <summary>
    /// 
    /// </summary>
    public class nLeague : CategoryTreeNode<nLeague>
    {
        /// <summary>
        /// 父ID
        /// </summary>
        new internal string FatherID { get { return base.FatherID; } set { base.FatherID = value; } }

        /// <summary>
        /// 地区、公司、职位、部门 （人员均需要在职位下，职位可以在任意节点下）
        /// </summary>
        public LeagueType Type { get; set; }

        /// <summary>
        /// 状态
        /// </summary> 
        public Status Status { get; set; }
    }

    public class nSubLeagues : SubTree<nLeague>
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param> 
        public nSubLeagues(IEnumerable<nLeague> data) : base(data)
        {

        }
    }
}