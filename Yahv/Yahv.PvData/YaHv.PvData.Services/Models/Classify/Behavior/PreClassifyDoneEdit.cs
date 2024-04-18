using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YaHv.PvData.Services.Extends;
using YaHv.PvData.Services.Handlers;
using YaHv.PvData.Services.Interfaces;

namespace YaHv.PvData.Services.Models
{
    /// <summary>
    /// 产品预归类已完成中的归类
    /// </summary>
    public sealed class PreClassifyDoneEdit : Classify
    {
        public PreClassifyDoneEdit(IClassifyProduct product) : base(product)
        {
            this.Product.Classified += Product_Classified;
        }

        /** 归类完成之后需要做的操作
        * 1.写产品归类操作日志
        * 2.写产品归类历史记录
        * 3.写系统映射关系
        */
        private void Product_Classified(object sender, ProductClassifiedEventArgs e)
        {
            using (var reponsitory = LinqFactory<PvDataReponsitory>.Create())
            {
                var preProduct = (PreProduct)e.Product;
                preProduct.Log_ClassifyOperatingEnter(reponsitory);
                preProduct.Log_ClassifiedPartNumberEnter(reponsitory);
                preProduct.MapsSystem_Enter(ClassifyStep.PreDoneEdit, reponsitory);
            }
        }

        public override void DoClassify()
        {
            this.Product.Step = ClassifyStep.PreDoneEdit;
            base.DoClassify();
        }

        public override void Lock()
        {
            this.Product.Step = ClassifyStep.PreDoneEdit;
            base.Lock();
        }

        public override void UnLock()
        {
            this.Product.Step = ClassifyStep.PreDoneEdit;
            base.UnLock();
        }

        public override void Return()
        {
            this.Product.Step = ClassifyStep.PreDoneEdit;
            base.Return();
        }
    }
}
