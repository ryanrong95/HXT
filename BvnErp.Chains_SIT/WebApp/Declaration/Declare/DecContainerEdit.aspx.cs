using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Declaration.Declare
{
    public partial class DecContainerEdit : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                loadcombobox();
            }
        }

        private void loadcombobox()
        {
            this.Model.Container = Needs.Wl.Admin.Plat.AdminPlat.BaseContainers.Select(item => new { Value = item.Code, Text = item.Code + "-" + item.Name }).OrderBy(item => item.Value).Json();
            string ID = Request.QueryString["DecConID"];
            var Container = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DeclareContainer[ID];
            if (Container != null)
            {
                this.Model.DefaultContainer = new
                {
                    ContainerID = Container.ContainerID,
                    ContainerMd = Container.ContainerMd,
                    GoodsNo = Container.GoodsNo,
                    LclFlag = Container.LclFlag,
                    GoodsContainerWeight = Container.GoodsContaWt==null?"":Convert.ToDecimal(Container.GoodsContaWt).ToString("f5"),
                }.Json();
            }
            else
            {
                this.Model.DefaultContainer = new{ }.Json();
            }
        }

        protected void data()
        {
            string DeclarationID = Request.QueryString["DeclarationID"];
            var DecList = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DecOriginList.Where(item => item.DeclarationID == DeclarationID).OrderBy(item=>item.GNo).AsQueryable();          

            Func<Needs.Ccs.Services.Models.DecList, object> convert = declareList => new
            {
                ID = declareList.ID,
                GNo = declareList.GNo,
                CodeTS = declareList.CodeTS,
                CiqCode = declareList.CiqCode,
                GName = declareList.GName,
                GoodsModel = declareList.GoodsModel,
                GQty = declareList.GQty,               
                CaseNo = declareList.CaseNo,
                NetWt = declareList.NetWt,
                GrossWt = declareList.GrossWt,              
            };

            Response.Write(new
            {
                rows = DecList.Select(convert).ToArray(),
                total = DecList.Count()
            }.Json());
        }

        protected void Save()
        {
            string ID = Request.Form["ID"];
            string DeclarationID = Request.Form["DeclarationID"];
            string ContainerID = Request.Form["ContainerID"];
            string ContainerMd = Request.Form["ContainerMd"];
            string GoodsNo = Request.Form["GoodsNo"];
            string LclFlag = Request.Form["LclFlag"];
            string GoodsContainerWeight = Request.Form["GoodsContainerWeight"];

       
            Needs.Ccs.Services.Models.DecContainer container = new Needs.Ccs.Services.Models.DecContainer();
            if (!string.IsNullOrEmpty(ID))
            {
                container.ID = ID;
            }
            container.DeclarationID = DeclarationID;
            container.ContainerID = ContainerID;
            container.ContainerMd = ContainerMd;
            if (!string.IsNullOrEmpty(GoodsNo))
            {
                container.GoodsNo = GoodsNo.Substring(0,GoodsNo.Length-1);
            }
            
            if (!string.IsNullOrEmpty(LclFlag))
            {
                container.LclFlag = Convert.ToInt32(LclFlag);
            }
            if (!string.IsNullOrEmpty(GoodsContainerWeight))
            {
                container.GoodsContaWt = Convert.ToDecimal(GoodsContainerWeight);
            }
           

            container.EnterError += DecContainer_EnterError;
            container.EnterSuccess += DecContainer_EnterSuccess;
            container.Enter();
        }

        /// <summary>
        /// 保存异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DecContainer_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Response.Write(new { success = false, message = e.Message });
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DecContainer_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "保存成功", ID = e.Object }).Json());
        }
    }
}