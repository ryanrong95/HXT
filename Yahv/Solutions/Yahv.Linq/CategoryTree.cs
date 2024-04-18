using System;
using System.Linq;
using Yahv.Underly;

namespace Yahv.Linq
{
    public class CategoryTreeNode<TEntity> // : TreeNodeBase where TEntity : TreeNodeBase
    {
        /// <summary>
        /// 唯一键
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 分类
        /// </summary>
        public Category Category { get; set; }

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
            return $"{this.ID} - {this.Name} - {this.FatherID} - {this.Category}";
        }
    }

    abstract public class CategoryTree<TEntity, TReponsitory> : QueryView<TEntity, TReponsitory>
        where TEntity : CategoryTreeNode<TEntity>, new()
        where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        /// <summary>
        /// 根节点
        /// </summary>
        public TEntity Root { get; private set; }

        /// <summary>
        /// 分类
        /// </summary>
        public Category Category { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CategoryTree(Category category)
        {
            Category = category;
            this.InitData();
            this.Init();
        }

        static object locker = new object();

        void InitData()
        {
            if (!this.Any(item => item.Category == Category && item.FatherID == null))
            {
                lock (locker)
                {
                    if (this.Root == null && !this.Any(item => item.Category == Category && item.FatherID == null))
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
