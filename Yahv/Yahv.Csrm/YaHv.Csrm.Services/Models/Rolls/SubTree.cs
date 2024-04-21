using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace YaHv.Csrm.Services.Models.Rolls
{
    public class nSubject : TreeNode<nSubject>
    {
        /// <summary>
        /// 父ID
        /// </summary>
        new internal string FatherID { get { return base.FatherID; } set { base.FatherID = value; } }

        /// <summary>
        /// 业务、分类、科目
        /// </summary>
        public SubjectType Type { get; set; }

        /// <summary>
        /// 状态
        /// </summary> 
        public Status Status { get; set; }
    }

    public class nSubSubjects : SubTree<nSubject>
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param> 
        public nSubSubjects(IEnumerable<nSubject> data) : base(data)
        {

        }
    }
}
