using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Yahv.Linq
{
    /// <summary>
    /// 附属主对象的集合操作 基类  
    /// </summary>
    /// <remarks>
    /// 基本开发思路已经与王亚讲解：
    /// 要明确区分，Add(Insert)、Update(Modify)、Delete(Remove、Abandon)。
    /// 这是传统面向对象的思路，Add与Delete是对集合的操作、Update是对对象本身的操作（包涵：假删除）
    /// 根据王亚的反馈，表示以上操作比较清晰。
    /// 陈翰表态：会带来一定的代码不好复用的问题，很威少的。这需要开发人员在开发时，十分注意就可以解决。
    /// </remarks>
    /// <typeparam name="TEntity">主类型</typeparam>
    abstract public class EsEnumerable<TEntity> : EsEnumerable<TEntity, TEntity> where TEntity : class, IUnique
    {
        public EsEnumerable(object father, IEnumerable<TEntity> data) : base(father, data)
        {

        }
    }

    /// <summary>
    /// 附属主对象的集合操作 基类  
    /// </summary>
    /// <typeparam name="TEntity">主类型</typeparam>
    /// <typeparam name="AddEntity">分离Add类型</typeparam>
    abstract public class EsEnumerable<TEntity, AddEntity> : IEnumerable<TEntity> where TEntity : class, IUnique
    {
        SortedDictionary<string, TEntity> data;
        object father;

        /// <summary>
        /// 构造器
        /// </summary>
        public EsEnumerable(object father, IEnumerable<TEntity> data)
        {
            this.father = father;

            if (data == null)
            {
                this.data = new SortedDictionary<string, TEntity>();
            }
            else
            {
                this.data = new SortedDictionary<string, TEntity>(data.ToDictionary(item => item.ID));
            }
        }
        //统一设置 data 中的 father

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>对象</returns>
        virtual public TEntity this[int index]
        {
            get { return data.Values.ElementAt(index); }
        }

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="isReload">是否重新读取</param>
        /// <returns>对象</returns>
        virtual public TEntity this[string index, bool isReload = false]
        {
            get
            {
                //这里的重新读取是否需要开发？
                return data[index];
            }
        }

        /// <summary>
        /// 已重写可枚举访问器
        /// </summary>
        /// <returns></returns>
        virtual public IEnumerator<TEntity> GetEnumerator()
        {
            return this.data.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// 获取总数
        /// </summary>
        public int Count { get { return this.data.Count; } }

        //到这里，以下只做说明使用

        virtual protected TEntity[] OnAdd(AddEntity[] arry)
        {
            return new TEntity[0];
        }

        virtual protected void OnRemove(string[] arry)
        {

        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="arry">数据</param>
        public void Add(params AddEntity[] arry)
        {
            var rslt = this.OnAdd(arry);

            if (typeof(AddEntity) == typeof(TEntity))
            {
                for (int index = 0; index < arry.Length; index++)
                {
                    var entity = arry[index] as TEntity;

                    this.data.Add(entity.ID, entity);
                }
            }
            else
            {
                for (int index = 0; index < rslt.Length; index++)
                {
                    var entity = rslt[index] as TEntity;

                    this.data.Add(entity.ID, entity);
                }
            }
        }
        //Add是否事件化？

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="predicate">条件表达式</param>
        public void Remove(params string[] arry)
        {
            this.OnRemove(arry);

            for (int index = 0; index < arry.Length; index++)
            {
                this.data.Remove(arry[index]);
            }
        }
        // Remove 是否事件化？
    }
}
