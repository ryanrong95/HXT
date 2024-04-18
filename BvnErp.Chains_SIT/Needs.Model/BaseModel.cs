using Needs.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Model
{   
    public abstract class ModelBase<TSource, TReponsitory> : IModel, IPersistence where TSource :  INotifyPropertyChanging, INotifyPropertyChanged
       where TReponsitory : Layer.Linq.IReponsitory, IDisposable, new()
    {
        #region 公共字段

        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// 数据状态
        /// //TODO:需要在框架中定义枚举
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 描述，备注
        /// </summary>
        public string Summary { get; set; }

        #endregion

        public ModelBase()
        {
            this.Status = 200;
            this.CreateDate = this.UpdateDate = DateTime.Now;
        }

        private TReponsitory reponsitory;

        public TReponsitory Reponsitory
        {
            get
            {
                if (reponsitory == null)
                {
                    reponsitory = new TReponsitory();
                }

                return reponsitory;
            }
            set
            {
                reponsitory = value;
            }
        }

        /// <summary>
        /// 当持久化成功时发生
        /// </summary>
        public event SuccessHanlder Entered;

        public virtual void Enter()
        {
            
        }

        public virtual void Abandon()
        {

        }
    }
}
