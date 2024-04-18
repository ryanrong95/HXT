using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using YaHv.PvData.Services.Extends;
using YaHv.PvData.Services.Handlers;
using YaHv.PvData.Services.Interfaces;

namespace YaHv.PvData.Services.Models
{
    /// <summary>
    /// 归类操作基类
    /// </summary>
    public abstract class Classify
    {
        /// <summary>
        /// 归类的产品
        /// </summary>
        protected IClassifyProduct Product { get; set; }

        public Classify(IClassifyProduct product)
        {
            this.Product = product;

            this.Product.Locked += Product_Locked;
            this.Product.UnLocked += Product_UnLocked;
            this.Product.Returned += Product_Returned;
        }

        private void Product_Locked(object sender, ProductLockedEventArgs e)
        {
            string stepDesc = e.Product.Step.GetDescription();
            if (e.Product.CreatorID != null)
            {
                using (var reponsitory = LinqFactory<PvDataReponsitory>.Create())
                {
                    e.Product.Log(LogType.Lock, "报关员【" + e.Product.CreatorName + "】锁定了" + stepDesc, reponsitory);
                }
            }
        }

        private void Product_UnLocked(object sender, ProductLockedEventArgs e)
        {
            string stepDesc = e.Product.Step.GetDescription();
            if (e.Product.CreatorID != null)
            {
                using (var reponsitory = LinqFactory<PvDataReponsitory>.Create())
                {
                    e.Product.Log(LogType.Lock, "报关员【" + e.Product.CreatorName + "】解锁了" + stepDesc, reponsitory);
                }
            }
        }

        private void Product_Returned(object sender, ProductReturnedEventArgs e)
        {
            if (e.Product.CreatorID != null)
            {
                using (var reponsitory = LinqFactory<PvDataReponsitory>.Create())
                {
                    string context = "报关员【" + e.Product.CreatorName + "】将此型号退回, 取消申报";
                    if (!string.IsNullOrEmpty(e.Product.Summary))
                    {
                        context += ",原因为:" + e.Product.Summary;
                    }
                    e.Product.Log(LogType.Return, context, reponsitory);
                }
            }
        }

        /// <summary>
        /// 归类
        /// </summary>
        public virtual void DoClassify()
        {
            this.Product.DoClassify();
            this.Product.UnLock();
        }

        /// <summary>
        /// 锁定
        /// </summary>
        public virtual void Lock()
        {
            this.Product.Lock();
        }

        /// <summary>
        /// 解锁
        /// </summary>
        public virtual void UnLock()
        {
            this.Product.UnLock();
        }

        /// <summary>
        /// 退回
        /// </summary>
        public virtual void Return()
        {
            this.Product.Return();
        }
    }
}
