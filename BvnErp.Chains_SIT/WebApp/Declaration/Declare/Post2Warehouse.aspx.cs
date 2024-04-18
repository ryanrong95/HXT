using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Declaration.Declare
{
    public partial class Post2Warehouse : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //GenerateDecheadPDF();
        }

        protected void UpdateEntryID()
        {
            try
            {
                string DeclarationID = Request.Form["ID"];              
                Needs.Ccs.Services.Models.DecHead headinfo = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DeclareHead[DeclarationID];                     
                headinfo.Only2WareHouse();

                Response.Write(new { result = true,info="保存成功" }.Json());
            }
            catch(Exception ex)
            {
                Response.Write(new { result = false, info = "保存错误" + ex.ToString() }.Json());
            }
        }

        /// <summary>
        /// 批量生成PDF文件到本地
        /// </summary>
        public void GenerateDecheadPDF()
        {
            //string ID = Request.Form["ID"];


            var decheadList = new Needs.Ccs.Services.Views.DecHeadsListView().Where(t => t.CreateTime > DateTime.Parse("2020-09-11")).ToList();

            decheadList.ForEach(t=> {

                var head = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.DecHeads[t.ID];

                var vendor = new Needs.Ccs.Services.VendorContext(Needs.Ccs.Services.VendorContextInitParam.Pointed, head.OwnerName).Current1;

                head.PaymentInstructionSaveAs();
                head.ContractSaveAs();
                head.PackingListSaveAs(vendor);

            });


            

        }

    }
}