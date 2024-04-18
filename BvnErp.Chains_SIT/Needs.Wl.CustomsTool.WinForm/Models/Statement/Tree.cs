using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.CustomsTool.WinForm.Models.Statement
{
    public class TreeNode
    {
        public NodeInfo Info { get; set; }
        public TreeNode Left;
        public TreeNode Right;
    }

    public class NodeInfo
    {
        /// <summary>
        /// Word
        /// </summary>
        public Word Word { get; set; }

        /// <summary>
        /// 是否是操作符
        /// </summary>
        public bool IsOperator { get; set; } = false;

        /// <summary>
        /// 是否是叶子节点
        /// </summary>
        public bool IsLeaf { get; set; } = false;

        /// <summary>
        /// lamda 表达式
        /// </summary>
        public Expression<Func<string, bool>> Expression { get; set; }
    }
}
