using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Yahv.Linq
{
    ///// <summary>
    ///// 树节点基类
    ///// </summary>
    //public class TreeNodeBase
    //{
    //    /// <summary>
    //    /// 唯一键
    //    /// </summary>
    //    public string ID { get; set; }

    //}

    /// <summary>
    /// 
    /// </summary>
    public class TreeNode<TEntity> // : TreeNodeBase where TEntity : TreeNodeBase
    {
        /// <summary>
        /// 唯一键
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 父ID
        /// </summary>
        internal protected string FatherID { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 父类
        /// </summary>
        public TEntity Father { get; internal set; }

        /// <summary>
        /// 子类
        /// </summary>
        public SubTree<TEntity> Sons { get; set; }

        /// <summary>
        /// 已重写 
        /// </summary>
        /// <returns>子父信息</returns>
        public override string ToString()
        {
            return $"{this.ID} - {this.Name} - {this.FatherID}";
        }
    }

    /// <summary>
    /// 子树结构
    /// </summary>
    public class SubTree<TEntity> : IEnumerable<TEntity>
    {
        List<TEntity> data;

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="data"></param>
        public SubTree(IEnumerable<TEntity> data)
        {
            this.data = InitData(data) ?? new List<TEntity>();
        }

        List<TEntity> InitData(IEnumerable<TEntity> data)
        {
            if (data == null)
            {
                return null;
            }

            if (data is List<TEntity>)
            {
                return (List<TEntity>)data;
            }
            else
            {
                return new List<TEntity>(data);
            }
        }
        /// <summary>
        /// 子数量
        /// </summary>
        public int Count { get { return this.data.Count; } }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<TEntity> GetEnumerator()
        {
            return this.data.GetEnumerator();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    /// <summary>
    /// 树结构数据基类
    /// </summary>
    /// <typeparam name="TEntity">指定类型</typeparam>
    /// <typeparam name="TReponsitory">指定数据源 支持者</typeparam>
    abstract public class Tree<TEntity, TReponsitory> : QueryView<TEntity, TReponsitory>
          where TEntity : TreeNode<TEntity>, new()
          where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        /// <summary>
        /// 根节点
        /// </summary>
        public TEntity Root { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public Tree()
        {
            this.InitData();
            this.Init();
        }

        static object locker = new object();

        void InitData()
        {
            if (!this.Any(item => item.FatherID == null))
            {
                lock (locker)
                {
                    if (this.Root == null && !this.Any(item => item.FatherID == null))
                    {
                        this.GenRoot();
                    }
                }
            }
        }

        void Init()
        {
            var arry = this.ToArray();

            var linqs = from currnet in arry
                        join _father in arry on currnet.FatherID equals _father.ID into fathers
                        from father in fathers.DefaultIfEmpty()
                        join son in arry on currnet.ID equals son.FatherID into sons
                        select new
                        {
                            currnet,
                            father,
                            sons
                        };

            this.Root = linqs.Select(item =>
            {
                item.currnet.Father = item.father;
                item.currnet.Sons = new SubTree<TEntity>(item.sons.ToArray());
                return item.currnet;
            }).Single(item => item.FatherID == null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        abstract protected override IQueryable<TEntity> GetIQueryable();

        /// <summary>
        /// 生成根节点
        /// </summary>
        virtual protected void GenRoot()
        {
            throw new NotImplementedException();
        }
    }
}