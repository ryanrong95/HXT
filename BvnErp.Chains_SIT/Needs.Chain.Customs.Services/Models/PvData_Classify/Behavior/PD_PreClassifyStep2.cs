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
    public class PD_PreClassifyStep2 : PD_Classify
    {
        public PD_PreClassifyStep2(PD_IClassifyProduct product) : base(product)
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

                //如果在系统管控库中为禁运型号，报关员归类为非禁运型号，生成禁运管控审批
                bool needForbidControl = classifyProduct.IsSysForbid && !classifyProduct.IsForbid;
                if (needForbidControl)
                    classifyProduct.ToPreProductControl(Enums.ItemCategoryType.Forbid, reponsitory);

                //如果在系统管控库中为3C认证型号，报关员归类为无需3C认证，生成3C管控审批
                bool needCCCControl = classifyProduct.IsSysCCC && !classifyProduct.IsCCC;
                if (needCCCControl)
                    classifyProduct.ToPreProductControl(Enums.ItemCategoryType.CCC, reponsitory);

                //不需要管控审批，则生成推送通知
                if (!needForbidControl && !needCCCControl)
                    classifyProduct.ToApiNotice(reponsitory);
            }
        }

        public override void DoClassify()
        {
            this.Product.ClassifyStatus = Enums.ClassifyStatus.Done;
            this.Product.ClassifyStep = Enums.ClassifyStep.PreStep2;
            base.DoClassify();
        }

        public override void QuickClassify()
        {
            this.Product.ClassifyStatus = Enums.ClassifyStatus.Done;
            this.Product.ClassifyStep = Enums.ClassifyStep.PreStep2;

            base.QuickClassify();
        }

        public override void Return()
        {
            this.Product.ClassifyStatus = Enums.ClassifyStatus.Anomaly;
            this.Product.ClassifyStep = Enums.ClassifyStep.PreStep2;
            base.Return();
        }

        public override void Delete()
        {
            base.Delete();
        }
    }
}
