using Needs.Ccs.Services.Hanlders;
using Needs.Ccs.Services.Interfaces;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 归类操作
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
        }

        private void Product_Locked(object sender, ProductLockedEventArgs e)
        {
            var classifyProduct = (IClassifyProduct)e.Product;
            string stepName = (string)e.Step.GetDescription();
            var declarant = classifyProduct.Admin;
            if (declarant != null)
            {
                classifyProduct.Log(Enums.LogTypeEnums.Lock, "报关员【" + declarant.RealName + "】锁定了" + stepName);
            }
        }

        private void Product_UnLocked(object sender, ProductLockedEventArgs e)
        {
            var classifyProduct = (IClassifyProduct)e.Product;
            string stepName = (string)e.Step.GetDescription();
            var declarant = classifyProduct.Admin;
            if (declarant != null)
            {
                classifyProduct.Log(Enums.LogTypeEnums.Lock, "报关员【" + declarant.RealName + "】解锁了" + stepName);
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
        /// 一键归类
        /// </summary>
        public virtual void QuickClassify()
        {
            this.Product.QuickClassify();
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

        public virtual void Anomaly()
        {
            this.Product.Anomaly();
            this.Product.UnLock();
        }
    }
}
