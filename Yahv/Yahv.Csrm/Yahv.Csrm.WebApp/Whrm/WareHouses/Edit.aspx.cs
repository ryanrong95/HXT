using System;
using System.Linq;
using Yahv.Underly;
using Yahv.Web.Forms;
using YaHv.Csrm.Services;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApp.Whrm.WareHouses
{
    public partial class Edit : ClientPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var id = Request.QueryString["id"];
                var entity = Erp.Current.Crm.WareHouses[id];
                //下拉
                //地区
                //级别
                this.Model.District = ExtendsEnum.ToArray<District>().Select(item => new
                {
                    value = (int)item,
                    text = item.GetDescription()
                });
                //2.级别
                this.Model.Grade = ExtendsEnum.ToArray<WarehouseGrade>().Select(item => new
                {
                    value = (int)item,
                    text = item.GetDescription()
                });
                this.Model.Entity = Erp.Current.Crm.WareHouses[id];
            }
        }


        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                var id = Request.QueryString["id"];
                var entity = Erp.Current.Crm.WareHouses[id] ?? new WareHouse();
                string admincode = Request["AdminCode"];
                entity.Enterprise = new Enterprise
                {
                    Name = Request.Form["Name"],
                    AdminCode = string.IsNullOrWhiteSpace(admincode) ? "" : admincode,
                    Corporation = Request["Corporation"].Trim(),
                    RegAddress = Request["RegAddress"].Trim(),
                    Uscc = Request["Uscc"].Trim()
                };
                entity.Grade = (WarehouseGrade)int.Parse(Request["Grade"]);
                entity.Address = Request.Form["Address"].Trim();
                entity.DyjCode = Request.Form["DyjCode"].Trim();
                entity.District = (Region)int.Parse(Request["District"]);
                entity.EnterSuccess += Entity_EnterSuccess;
                if (string.IsNullOrWhiteSpace(id))
                {
                    entity.NameRepeat += Entity_NameRepeat;
                }
                entity.Enter();
            }
            catch (Exception ex)
            {
                Easyui.Alert("提示", ex.Message, Yahv.Web.Controls.Easyui.Sign.Warning);
            }

        }

        private void Entity_NameRepeat(object sender, Usually.ErrorEventArgs e)
        {
            Easyui.Alert("提示", "库房已存在", Yahv.Web.Controls.Easyui.Sign.Warning);
        }

        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            //Easyui.Reload("提示", "保存成功", Yahv.Web.Controls.Easyui.Sign.Info);
            Easyui.Window.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }
    }
}