using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Utils.Flow.Process
{
    /// <summary>
    /// 上下文
    /// </summary>
    /// <typeparam name="TParam">上下文的参数类型</typeparam>
    /// <typeparam name="TNode">节点的参数类型</typeparam>
    public class Context<TParam, TNode>
    {
        private TParam param { get; set; }

        public Context(TParam param)
        {
            this.param = param;
        }

        /// <summary>
        /// 上下文中的参数
        /// </summary>
        public TParam Param
        {
            get { return param; }
        }

        private BaseNode<TParam, TNode> node;

        /// <summary>
        /// 在上下文中保存的当前节点
        /// </summary>
        public BaseNode<TParam, TNode> Node
        {
            get { return node; }
            set { node = value; }
        }

        /// <summary>
        /// 在上下文中执行当前节点的处理方式
        /// </summary>
        /// <param name="param"></param>
        public void Execute(TNode param)
        {
            this.node.Handler(this, param);
        }
    }
}
