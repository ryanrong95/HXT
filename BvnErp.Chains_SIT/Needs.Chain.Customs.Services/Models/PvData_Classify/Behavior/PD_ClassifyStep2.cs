using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using Needs.Ccs.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 产品归类预处理二
    /// </summary>
    public class PD_ClassifyStep2 : PD_Classify
    {
        public PD_ClassifyStep2(PD_IClassifyProduct product) : base(product)
        {
            this.Product.Classified += Product_Classified;
        }

        /** 归类完成之后需要做的操作
         * 1.进行产品管控
         * 2.同步订单特殊类型(OrderVoyage)
         * 3.更新订单信息
         */
        private void Product_Classified(object sender, ProductClassifiedEventArgs e)
        {
            var classifyProduct = (PD_ClassifyProduct)e.Product;

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                classifyProduct.UpdateSecondOperator(reponsitory);
                classifyProduct.DoProductControl(reponsitory);
                classifyProduct.SetOrderVoyage(reponsitory);
                classifyProduct.UpdateOrder(reponsitory);
            }
        }

        public override void DoClassify()
        {
            this.Product.ClassifyStatus = ClassifyStatus.Done;
            this.Product.ClassifyStep = ClassifyStep.Step2;
            base.DoClassify();
        }

        public override void QuickClassify()
        {
            this.Product.ClassifyStatus = ClassifyStatus.Done;
            this.Product.ClassifyStep = ClassifyStep.Step2;

            base.QuickClassify();
        }
    }
}
