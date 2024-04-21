using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;
using Yahv.Web.Forms;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApp.Crm.WarehousePlates
{
    public partial class Edit : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //地区
                this.Model.District = ExtendsEnum.ToArray<District>().Select(item => new
                {
                    value = (int)item,
                    text = item.GetDescription()
                });
                this.Model.Entity = new ConsigneesRoll()[Request.QueryString["id"]];
                this.Model.EnterpriseType = Request["enterprisetype"];
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string clientid = Request.QueryString["clientid"];
            var id = Request.QueryString["id"];

            Consignee entity = new ConsigneesRoll()[id] ?? new Consignee();
            string platecode = Request["PlateCode"].Trim();
            if (string.IsNullOrWhiteSpace(id) && CheckPlatecode(platecode))//新增时验证门牌Code是否存在
            {
                Easyui.Reload("提示", "门牌编号已存在", Yahv.Web.Controls.Easyui.Sign.Warning);
                return;
            }
            
            entity.PlateCode = platecode;
            entity.Title = Request["Title"].Trim();
            entity.EnterpriseID = clientid;
            entity.Address = Request.Form["Address"].Trim();
            entity.District = (District)int.Parse(Request["selDistrict"]);
            entity.Postzip = Request.Form["Postzip"].Trim();
            entity.DyjCode = Request.Form["DyjCode"].Trim();
            entity.Name = Request.Form["Name"].Trim();
            entity.Tel = Request.Form["Tel"].Trim();
            entity.Mobile = Request.Form["Mobile"].Trim();
            entity.Email = Request.Form["Email"].Trim();

            if (string.IsNullOrWhiteSpace(id))
            {
                entity.CreatorID = Yahv.Erp.Current.ID;
            }
            entity.EnterSuccess += Entity_EnterSuccess;
            entity.Enter();

        }

        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            Easyui.Dialog.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }
        /// <summary>
        /// 验证编码是否重复
        /// </summary>
        /// <returns></returns>
        bool CheckPlatecode(string platecode)
        {
            string clientno = Request.Form["ClientNo"];
            bool flag1 = new ConsigneesRoll().Any(item => item.PlateCode == platecode);//门牌编码是否重复
            bool flag2 = Erp.Current.Crm.WareHouses.Any(item => item.WsCode == platecode);//库房编码是否重
            return flag1 || flag2;
        }
    }
}