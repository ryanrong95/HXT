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

namespace Yahv.Csrm.WebApp.Crm.WareHouses
{
    public partial class Edit : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var id = Request.QueryString["id"];
                //地区
                this.Model.Region = ExtendsEnum.ToArray<Region>().Select(item => new
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
                this.Model.Entity = new WareHousesRoll()[id];
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                var id = Request.QueryString["id"];
                var entity = new WareHousesRoll()[id] ?? new WareHouse();
                string admincode = Request["AdminCode"];
                entity.Enterprise = new Enterprise
                {
                    Name = Request.Form["Name"],
                    AdminCode = string.IsNullOrWhiteSpace(admincode) ? "" : admincode,
                    Corporation = Request["Corporation"].Trim(),
                    RegAddress = Request["RegAddress"].Trim(),
                    Uscc = Request["Uscc"].Trim(),
                    Status = ApprovalStatus.Normal
                };
                entity.Grade = (WarehouseGrade)int.Parse(Request["Grade"]);
                string address = Request.Form["Address"].Trim();
                entity.Address = string.IsNullOrWhiteSpace(address) ? "" : address;
                string dyjcode = Request.Form["DyjCode"].Trim();
                entity.DyjCode = string.IsNullOrWhiteSpace(dyjcode) ? "" : dyjcode;
                var region = int.Parse(Request["District"]);
                entity.District = (Region)region;
                //库房简码
                //var regions = new WareHousesRoll().Count(t => (int)t.District == region);
                //var index = regions < 9 ? "0" + (regions + 1).ToString() : (regions + 1).ToString();
                //entity.WsCode = $"{(Region)region}{index}";
                entity.EnterSuccess += Entity_EnterSuccess;
                if (string.IsNullOrWhiteSpace(id))
                {
                    entity.CreatorID = ID = Yahv.Erp.Current.ID;
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
            var entity = sender as WareHouse;
            Easyui.Reload("提示", "库房已存在", Yahv.Web.Controls.Easyui.Sign.Warning);
        }

        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as WareHouse;
            //操作日志
            if (string.IsNullOrEmpty(Request.QueryString["id"]))
            {
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                         nameof(Yahv.Systematic.Crm),
                                        "WareHouseInsert", "新增库房：" + entity.Enterprise.Name, "");
            }
            else
            {
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                         nameof(Yahv.Systematic.Crm),
                                        "WareHouseUpdate", "修改库房：" + entity.Enterprise.Name, "");
            }
            Easyui.Window.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }
    }
}