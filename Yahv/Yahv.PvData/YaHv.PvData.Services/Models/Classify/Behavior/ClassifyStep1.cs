using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Utils.Converters.Contents;
using YaHv.PvData.Services.Extends;
using YaHv.PvData.Services.Handlers;
using YaHv.PvData.Services.Interfaces;

namespace YaHv.PvData.Services.Models
{
    /// <summary>
    /// 产品归类预处理一
    /// </summary>
    public sealed class ClassifyStep1 : Classify
    {
        public ClassifyStep1(IClassifyProduct product) : base(product)
        {
            this.Product.Classified += Product_Classified;
        }

        /** 预处理一归类完成之后需要做的操作
        * 1.写产品归类操作日志
        * 2.写系统映射关系
        */
        private void Product_Classified(object sender, ProductClassifiedEventArgs e)
        {
            using (var reponsitory = LinqFactory<PvDataReponsitory>.Create())
            {
                var op = (OrderedProduct)e.Product;
                op.Log_ClassifyOperatingEnter(reponsitory);
                op.MapsSystem_Enter(ClassifyStep.Step1, reponsitory);

                //判断报关员角色
                if (op.Role != DeclarantRole.JuniorDeclarant)
                {
                    //是否是特殊类型
                    bool isSpecialType = op.Ccc || op.Embargo || op.HkControl || op.Coo || op.CIQ || op.IsHighPrice || op.IsSysCcc || op.IsSysEmbargo;
                    //关税率
                    decimal rate = op.ImportPreferentialTaxRate + op.OriginATRate;
                    //如果无关税，并且不是特殊类型，也不属于海关验估编码，系统自动做预处理二到已完成
                    if (rate == 0 && !isSpecialType && !op.IsCustomsInspection)
                    {
                        op.Log_ClassifiedPartNumberEnter(reponsitory);
                        op.MapsSystem_Enter(ClassifyStep.Step2, reponsitory);
                        op.ProductQuoteEnter(reponsitory);

                        SyncManager.Current.Classify.For(op).DoSync();
                    }
                }
            }
        }

        public override void DoClassify()
        {
            this.Product.Step = ClassifyStep.Step1;
            base.DoClassify();
        }

        public override void Lock()
        {
            this.Product.Step = ClassifyStep.Step1;
            base.Lock();
        }

        public override void UnLock()
        {
            this.Product.Step = ClassifyStep.Step1;
            base.UnLock();
        }
    }
}
