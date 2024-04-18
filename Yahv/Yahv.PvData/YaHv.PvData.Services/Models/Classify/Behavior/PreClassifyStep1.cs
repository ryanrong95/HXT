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
    /// 产品预归类预处理一
    /// </summary>
    public sealed class PreClassifyStep1 : Classify
    {
        public PreClassifyStep1(IClassifyProduct product) : base(product)
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
                var pp = (PreProduct)e.Product;
                pp.Log_ClassifyOperatingEnter(reponsitory);
                pp.MapsSystem_Enter(ClassifyStep.PreStep1, reponsitory);

                //判断报关员角色
                if (pp.Role != DeclarantRole.JuniorDeclarant)
                {
                    //是否是特殊类型
                    bool isSpecialType = pp.Ccc || pp.Embargo || pp.HkControl || pp.Coo || pp.CIQ || pp.IsHighPrice || pp.IsSysCcc || pp.IsSysEmbargo;
                    //关税率
                    decimal rate = pp.ImportPreferentialTaxRate + pp.OriginATRate;
                    //如果无关税，并且不是特殊类型，也不属于海关验估编码，系统自动做预处理二到已完成
                    if (rate == 0 && !isSpecialType && !pp.IsCustomsInspection)
                    {
                        pp.Log_ClassifiedPartNumberEnter(reponsitory);
                        pp.MapsSystem_Enter(ClassifyStep.PreStep2, reponsitory);
                    }
                }
            }
        }

        public override void DoClassify()
        {
            this.Product.Step = ClassifyStep.PreStep1;
            base.DoClassify();
        }

        public override void Lock()
        {
            this.Product.Step = ClassifyStep.PreStep1;
            base.Lock();
        }

        public override void UnLock()
        {
            this.Product.Step = ClassifyStep.PreStep1;
            base.UnLock();
        }

        public override void Return()
        {
            this.Product.Step = ClassifyStep.PreStep1;
            base.Return();
        }
    }
}
