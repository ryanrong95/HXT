using Needs.Utils.Flow.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Utils.Flow.Process
{
    /// <summary>
    /// 基础节点类
    /// </summary>
    /// <typeparam name="TParam">上下文的参数类型</typeparam>
    /// <typeparam name="TNode">节点的参数类型</typeparam>
    public abstract class BaseNode<TParam, TNode>
    {
        protected EventBuilder<TNode> EventBuilder = new EventBuilder<TNode>();

        /// <summary>
        /// 每个节点自己的处理方式
        /// </summary>
        /// <param name="context"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public abstract object Handler(Context<TParam, TNode> context, TNode param);
    }
}
