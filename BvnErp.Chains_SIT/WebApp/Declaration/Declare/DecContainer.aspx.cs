using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Declaration.Declare
{
    public partial class DecContainer : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void data()
        {
            string DeclarationID = Request.QueryString["ID"];
            var data = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DeclareContainer.Where(item => item.DeclarationID == DeclarationID).AsQueryable(); 

            Func<Needs.Ccs.Services.Models.DecContainer, object> convert = declareLicense => new
            {
                ID = declareLicense.ID,
                ContainerID = declareLicense.ContainerID,
                ContainerMd =  declareLicense.Container==null? declareLicense.ContainerMd : declareLicense.Container.Code+"-"+declareLicense.Container.Name,
                GoodsNo = declareLicense.GoodsNo,
                LclFlag = declareLicense.LclFlag==1?"是":"否",
                GoodsContaWt = String.Format("{0:N5} ", declareLicense.GoodsContaWt) ,
            };
            this.Paging(data, convert);
        }

        protected void Delete()
        {
            string ID = Request.Form["ID"];
            Needs.Ccs.Services.Models.DecContainer declicense = new Needs.Ccs.Services.Models.DecContainer();
            declicense.ID = ID;
            declicense.EnterError += DecHead_EnterError;
            declicense.EnterSuccess += DecHead_EnterSuccess;
            declicense.PhysicalDelete();
        }

        /// <summary>
        /// 保存异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DecHead_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Response.Write(new { success = false, message = e.Message });
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DecHead_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "保存成功", ID = e.Object }).Json());
        }
    }
}