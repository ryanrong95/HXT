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
    public class PD_ReClassify : PD_Classify
    {
        public PD_ReClassify(PD_IClassifyProduct product) : base(product)
        {
            this.Product.Classified += Product_Classified;
        }

        /** 重新归类完成之后需要做的操作
         * 1.同步订单特殊类型(OrderVoyage)
         * 2.更新产品变更通知的处理状态
         */
        private void Product_Classified(object sender, ProductClassifiedEventArgs e)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var classifyProduct = (PD_ClassifyProduct)e.Product;

                classifyProduct.UpdateSecondOperator(reponsitory);
                classifyProduct.SetOrderVoyage(reponsitory);
                classifyProduct.UpdateOrderItemChangeNotice(reponsitory);
            }
        }

        public override void DoClassify()
        {
            this.Product.ClassifyStatus = ClassifyStatus.Done;
            this.Product.ClassifyStep = ClassifyStep.ReClassify;
            base.DoClassify();
        }
    }
}
