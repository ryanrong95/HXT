using Needs.Ccs.Services.Interfaces;
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
    public abstract class PD_Classify
    {
        /// <summary>
        /// 归类的产品
        /// </summary>
        protected PD_IClassifyProduct Product { get; set; }

        public PD_Classify(PD_IClassifyProduct product)
        {
            this.Product = product;
        }

        /// <summary>
        /// 归类
        /// </summary>
        public virtual void DoClassify()
        {
            this.Product.DoClassify();
        }

        /// <summary>
        /// 一键归类
        /// </summary>
        public virtual void QuickClassify()
        {
            this.Product.QuickClassify();
        }

        /// <summary>
        /// 退回
        /// </summary>
        public virtual void Return()
        {
            this.Product.Return();
        }

        public virtual void Delete()
        {
            this.Product.Delete();
        }
    }
}
