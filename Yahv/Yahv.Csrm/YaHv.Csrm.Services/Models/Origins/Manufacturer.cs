using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;
using Yahv.Usually;
using Yahv.Utils.Converters.Contents;

namespace YaHv.Csrm.Services.Models.Origins
{
    /// <summary>
    /// 供应商（优势品牌）
    /// </summary>
    public class Manufacturer : Yahv.Linq.IUnique
    {
        #region 单例
        static Manufacturer current;
        static object locker = new object();
        /// <summary>
        /// 单利实例化
        /// </summary>
        /// <returns></returns>
        static public Manufacturer Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new Manufacturer();
                        }
                    }
                }
                return current;
            }
        }
        #endregion

        string id;
        /// <summary>
        /// 
        /// </summary>
        public string ID
        {
            get
            {
                return this.id ?? this.Name.MD5();
            }
            set { this.id = value; }
        }
        /// <summary>
        /// 品牌名称
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 是否代理品牌
        /// </summary>
        public bool Agent { set; get; }


        #region 事件
        /// <summary>
        /// EnterSuccess
        /// </summary>
        public event SuccessHanlder EnterSuccess;
        /// <summary>
        /// AbandonSuccess
        /// </summary>
        public event SuccessHanlder AbandonSuccess;
        #endregion
        #region 持久化
        public void Enter()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                if (repository.GetTable<Layers.Data.Sqls.PvbCrm.Manufacturers>().Any(item => item.ID == this.ID))
                {
                    repository.Update<Layers.Data.Sqls.PvbCrm.Manufacturers>(new
                    {
                        Agent = this.Agent
                    }, item => item.ID == this.ID);
                }
                else
                {
                    repository.Insert<Layers.Data.Sqls.PvbCrm.Manufacturers>(new Layers.Data.Sqls.PvbCrm.Manufacturers
                    {
                        ID = this.ID,
                        Name = this.Name,
                        Agent = this.Agent
                    });
                }
                if (this != null && this.EnterSuccess != null)
                {
                    this.EnterSuccess(this, new SuccessEventArgs(this));
                }
            }
        }

        public void Abandon()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                if (repository.GetTable<Layers.Data.Sqls.PvbCrm.Manufacturers>().Any(item => item.ID == this.ID))
                {
                    repository.Delete<Layers.Data.Sqls.PvbCrm.Manufacturers>(item => item.ID == this.ID);
                }
                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this));
                }
            }
        }
        #endregion


    }

    public class ViewManufacturer
    {
        /// <summary>
        /// 品牌名称
        /// </summary>
        [Description("品牌名称")]
        public string Name { set; get; }
        /// <summary>
        /// 是否代理
        /// </summary>
        [Description("是否代理")]
        public bool Agent { set; get; }
    }
}
