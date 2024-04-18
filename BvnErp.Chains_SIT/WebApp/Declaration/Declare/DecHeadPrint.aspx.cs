using Needs.Ccs.Services;
using Needs.Ccs.Services.Models;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Declaration.Declare
{
    public partial class DecHeadPrint : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Print()
        {
            string DeclarationID = Request.Form["ID"];
            var DecHead = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DeclareHead[DeclarationID];
            var vendor = new VendorContext(VendorContextInitParam.Pointed, DecHead.OwnerName).Current1;
            var InPort = "";//入境口岸
            var CusPort = "";//关区代码
            var CusPortName = "";//关区代码
            var CIQPort = "";//检验检疫机关代码
            switch (DecHead.EntyPortCode)
            {
                //文锦渡
                case "470401":
                    InPort = "470401";
                    CusPort = "5320";
                    CIQPort = "475400";
                    CusPortName = "文锦渡";
                    break;
                //皇岗
                case "470201":
                    InPort = "470201";
                    CusPort = "5301";
                    CIQPort = "475200";
                    CusPortName = "皇岗";
                    break;
                //沙头角
                case "470501":
                    InPort = "470501";
                    CusPort = "5303";
                    CIQPort = "475500";
                    CusPortName = "沙头角";
                    break;
                //默认皇岗
                default:
                    InPort = "470201";
                    CusPort = "5301";
                    CIQPort = "475200";
                    CusPortName = "皇岗";
                    break;
            }

            string jsonResult = new {
                InPort = InPort,
                CusPort = CusPort,
                CIQPort = CIQPort,
                CusPortName = CusPortName,
                ContrNo = DecHead.ContrNo,

                CustomsCode = PurchaserContext.Current.CustomsCode,//消费使用单位代码-10位
                Code = PurchaserContext.Current.Code,//消费单位 统一社会信用代码
                CiqCode = PurchaserContext.Current.CiqCode,//消费单位 检验检疫代码
                CompanyName = PurchaserContext.Current.CompanyName,//消费使用单位-客户名称

                OwnerName = DecHead.OwnerName,//消费使用单位-客户名称
                OwnerCusCode = DecHead.OwnerCusCode,//消费使用单位代码-10位
                OwnerScc = DecHead.OwnerScc,//消费单位 同意社会信用代码               

                VendorCompanyName = vendor.CompanyName,

                VoyNo = DecHead.VoyNo,//航次号
                BillNo = DecHead.BillNo,//提运单号
                PackNo = DecHead.PackNo,//件数
                GrossWt = DecHead.GrossWt,
                NetWt = DecHead.NetWt,

                IsInspection = (DecHead.IsInspection || (DecHead.IsQuarantine==null?false: DecHead.IsQuarantine.Value))?"是":"否",
            }.Json();
            
            Response.Write(new { result = true, info = jsonResult }.Json());
        }
    }
}