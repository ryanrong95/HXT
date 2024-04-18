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
    /// 产品归类已完成中的归类
    /// </summary>
    public sealed class ClassifyDoneEdit : Classify
    {
        public ClassifyDoneEdit(IClassifyProduct product) : base(product)
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
                var orderedProduct = (OrderedProduct)e.Product;
                orderedProduct.Log_ClassifyOperatingEnter(reponsitory);
                orderedProduct.Log_ClassifiedPartNumberEnter(reponsitory);
                orderedProduct.MapsSystem_Enter(ClassifyStep.DoneEdit, reponsitory);

                SyncManager.Current.Classify.For(orderedProduct).DoSync();
            }
        }

        public override void DoClassify()
        {
            this.Product.Step = ClassifyStep.DoneEdit;
            base.DoClassify();
        }

        public override void Lock()
        {
            this.Product.Step = ClassifyStep.DoneEdit;
            base.Lock();
        }

        public override void UnLock()
        {
            this.Product.Step = ClassifyStep.DoneEdit;
            base.UnLock();
        }
    }
}
