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
    /// 产品预归类已完成中的归类
    /// </summary>
    public class PD_PreClassifyDoneEdit : PD_Classify
    {
        public PD_PreClassifyDoneEdit(PD_IClassifyProduct product) : base(product)
        {
            this.Product.Classified += Product_Classified;
        }

        /** 归类完成之后需要做的操作
         * 1.更新归类操作人
         * 2.写信息推送通知
         */
        private void Product_Classified(object sender, ProductClassifiedEventArgs e)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var classifyProduct = (PD_PreClassifyProduct)e.Product;

                classifyProduct.UpdateSecondOperator(reponsitory);

                //判断该产品是否已经通过接口下单
                int count = new Views.Origins.OrderItemsOrigin()
                    .Where(t => t.ProductUniqueCode == classifyProduct.ProductUnionCode && t.Status == Enums.Status.Normal).Count();
                //如果该产品尚未通过接口下单，则生成推送通知；否则不需要再推送
                if (count == 0)
                {
                    classifyProduct.ToApiNotice(reponsitory);
                }
            }
        }

        public override void DoClassify()
        {
            this.Product.ClassifyStatus = Enums.ClassifyStatus.Done;
            this.Product.ClassifyStep = Enums.ClassifyStep.PreDoneEdit;
            base.DoClassify();
        }

        public override void Return()
        {
            this.Product.ClassifyStatus = Enums.ClassifyStatus.Anomaly;
            this.Product.ClassifyStep = Enums.ClassifyStep.PreDoneEdit;
            base.Return();
        }

        public override void Delete()
        {
            base.Delete();
        }
    }
}
