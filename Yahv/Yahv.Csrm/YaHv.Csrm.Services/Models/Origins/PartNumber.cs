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
    ///型号
    /// </summary>
    public class PartNumber : Yahv.Linq.IUnique
    {
        #region 单例
        static PartNumber current;
        static object locker = new object();
        /// <summary>
        /// 单利实例化
        /// </summary>
        /// <returns></returns>
        static public PartNumber Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new PartNumber();
                        }
                    }
                }
                return current;
            }
        }
        #endregion

        #region 属性
        string id;
        /// <summary>
        /// 
        /// </summary>
        public string ID
        {
            get
            {
                return this.id ?? string.Join("",
                    this.Name,
                    this.Manufacturer
                    ).MD5();
            }
            set { this.id = value; }
        }
        /// <summary>
        /// 型号名称
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 品牌
        /// </summary>
        public string Manufacturer { set; get; }
        ///// <summary>
        ///// ClientID,SupplierID,AdminID
        ///// </summary>
        //public string OwnID { set; get; }
        ///// <summary>
        ///// 类型
        ///// </summary>
        //public AdvantageType Type { set; get; }
        #endregion

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
                if (repository.GetTable<Layers.Data.Sqls.PvbCrm.PartNumbers>().Any(item => item.ID == this.ID))
                {
                    repository.Update<Layers.Data.Sqls.PvbCrm.PartNumbers>(new
                    {
                        Name = this.Name,
                        Manufacturer = this.Manufacturer
                    }, item => item.ID == this.ID);
                }
                else
                {
                    repository.Insert(new Layers.Data.Sqls.PvbCrm.PartNumbers
                    {
                        ID = this.ID,
                        Name = this.Name,
                        Manufacturer = this.Manufacturer
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
                if (repository.GetTable<Layers.Data.Sqls.PvbCrm.PartNumbers>().Any(item => item.ID == this.ID))
                {
                    repository.Delete<Layers.Data.Sqls.PvbCrm.PartNumbers>(item => item.ID == this.ID);
                }
                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this));
                }
            }
        }
        #endregion
    }
    public class ViewPartNumber
    {
        /// <summary>
        /// 型号
        /// </summary>
        [Description("型号")]
        public string Name { set; get; }
        /// <summary>
        /// 品牌
        /// </summary>
        [Description("品牌")]
        public string Manufacturer { set; get; }
    }
}
