using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Usually;
using Yahv.Utils.Converters.Contents;

namespace YaHv.Csrm.Services.Models.Origins
{
    public class AutoQuote : Yahv.Linq.IUnique
    {
        #region 属性
        string id;
        /// <summary>
        ///  唯一码，MD5（型号、供应商、制造商、封装）
        /// </summary>
        /// <chenhan>保障全局唯一</chenhan>
        public string ID
        {
            get
            {
                return this.id ?? string.Concat(
                    this.Name,
                    Supplier,
                    Manufacturer,
                    PackageCase).MD5();

            }
            set
            {
                this.id = value;
            }
        }
        /// <summary>
        /// 型号
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 供应商ID=MD5(SupplierName)
        /// </summary>
        public string SupplierID { set; get; }
        /// <summary>
        /// 供应商名称
        /// </summary>
        public string Supplier { set; get; }
        /// <summary>
        /// 制造商名称
        /// </summary>
        public string Manufacturer { set; get; }
        /// <summary>
        /// 批次
        /// </summary>
        public string DateCode { set; get; }
        /// <summary>
        ///封装
        /// </summary>
        public string PackageCase { set; get; }
        /// <summary>
        /// 包装
        /// </summary>
        public string Packaging { set; get; }
        /// <summary>
        /// 阶梯价（做保留设计）
        /// </summary>
        public string Prices { set; get; }
        /// <summary>
        /// 单价
        /// </summary>
        public string UnitPrice { set; get; }
        /// <summary>
        /// 库存数量
        /// </summary>
        public string Quantity { set; get; }
        /// <summary>
        /// 报价人（Admin）
        /// </summary>
        public string ReporterID { set; get; }
        /// <summary>
        /// 有效期
        /// </summary>
        public DateTime Deadline { set; get; }
        /// <summary>
        /// 报价时间
        /// </summary>
        public DateTime? CreateDate { set; get; }
       // public Admin Admin { set; get; }
        #endregion

        #region 单例
        static AutoQuote current;
        static object locker = new object();
        /// <summary>
        /// 单利实例化
        /// </summary>
        /// <returns></returns>
        static public AutoQuote Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new AutoQuote();
                        }
                    }
                }
                return current;
            }
        }
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
                if (repository.GetTable<Layers.Data.Sqls.PvbCrm.AutoQuotes>().Any(item => item.ID == this.ID))
                {
                    repository.Update<Layers.Data.Sqls.PvbCrm.AutoQuotes>(new
                    {
                        DateCode = this.DateCode,
                        PackageCase = this.PackageCase,
                        Packaging = this.Packaging,
                        Prices = this.Prices,
                        UnitPrice = this.UnitPrice,
                        Quantity = this.Quantity,
                        ReporterID = this.ReporterID,
                        Deadline = this.Deadline,
                    }, item => item.ID == this.ID);
                }
                else
                {
                    repository.Insert<Layers.Data.Sqls.PvbCrm.AutoQuotes>(new Layers.Data.Sqls.PvbCrm.AutoQuotes
                    {
                        ID = this.ID,
                        Name = this.Name,
                        SupplierID = this.SupplierID,
                        Supplier = this.Supplier,
                        Manufacturer = this.Manufacturer,
                        DateCode = this.DateCode,
                        PackageCase = this.PackageCase,
                        Packaging = this.Packaging,
                        Prices = this.Prices,
                        UnitPrice = this.UnitPrice,
                        Quantity = this.Quantity,
                        ReporterID = this.ReporterID,
                        Deadline = this.Deadline,
                        CreateDate = DateTime.Now
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
                if (repository.GetTable<Layers.Data.Sqls.PvbCrm.AutoQuotes>().Any(item => item.ID == this.ID))
                {
                    repository.Delete<Layers.Data.Sqls.PvbCrm.AutoQuotes>(item => item.ID == this.ID);
                }
                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this));
                }
            }
        }
        #endregion
    }
}
