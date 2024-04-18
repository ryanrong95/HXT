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
    /// 产品归类预处理一
    /// </summary>
    public class PD_ClassifyStep1 : PD_Classify
    {
        public PD_ClassifyStep1(PD_IClassifyProduct product) : base(product)
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
            var cp = (PD_ClassifyProduct)e.Product;

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                cp.UpdateFirstOperator(reponsitory);

                //判断报关员角色
                var role = new Views.AdminRolesTopView(reponsitory).First(item => item.ID == cp.Admin.ID).DeclarantRole;
                if (role != DeclarantRole.JuniorDeclarant)
                {
                    //是否是特殊类型
                    bool isSpecialType = cp.IsCCC || cp.IsForbid || cp.IsOriginProof || cp.IsInsp || cp.IsHighValue || cp.IsSysCCC || cp.IsSysForbid;
                    //如果无关税，并且不是特殊类型，也不属于海关验估编码，系统自动做预处理二到已完成
                    if (cp.ImportTax.Rate == 0 && !isSpecialType && !cp.IsCustomsInspection)
                    {
                        //修改自动完成二次归类的归类人为Npc
                        cp.Admin = Needs.Underly.FkoFactory<Admin>.Create(Icgoo.DefaultCreator);
                        cp.UpdateSecondOperator(reponsitory);
                        cp.UpdateClassifyStatus(ClassifyStatus.Done, reponsitory);
                        cp.SetOrderVoyage(reponsitory);
                        cp.UpdateOrder(reponsitory);
                    }
                }
            }
        }

        public override void DoClassify()
        {
            this.Product.ClassifyStatus = ClassifyStatus.First;
            this.Product.ClassifyStep = ClassifyStep.Step1;
            base.DoClassify();
        }
    }
}
