using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;
using Yahv.Web.Forms;
using YaHv.Csrm.Services.Models.Origins;

namespace Yahv.Csrm.WebApp.Srm.WsSuppliers
{
    public partial class Edit : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //            if (!IsPostBack)
            //            {
            //                var supplierid = Request.QueryString["id"];
            //                this.Model.Entity = Erp.Current.Srm.WsSuppliers[supplierid];
            //                init();
        }

        void init()
        {
            //            //级别
            //            this.Model.Grade = ExtendsEnum.ToArray<SupplierGrade>().Select(item => new
            //            {
            //                value = (int)item,
            //                text = item.GetDescription()
            //            });
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            //            var id = Request.QueryString["id"];
            //            var entity = Erp.Current.Srm.WsSuppliers[id] ?? new WsSupplier();
            //            string admincode = Request["AdminCode"].Trim();
            //            string corporation = Request["Corporation"].Trim();
            //            string regAddress = Request["RegAddress"].Trim();
            //            string uscc = Request["Uscc"].Trim();
            //            string summary = Request["Summary"];
            //            string chinesename = Request["ChineseName"];
            //            string englishName = Request["EnglishName"];

            //            entity.Summary = summary;
            //            entity.ChineseName = chinesename;
            //            entity.EnglishName = englishName;
            //            entity.Enterprise = new Enterprise
            //            {
            //                Name = Request.Form["Name"],
            //                AdminCode = string.IsNullOrWhiteSpace(admincode) ? "" : admincode,
            //                Corporation = corporation,
            //                RegAddress = regAddress,
            //                Uscc = uscc
            //            };
            //            if (string.IsNullOrEmpty(id))
            //            {
            //                //录入人
            //                entity.Admin = new Admin
            //                {
            //                    ID = Yahv.Erp.Current.ID
            //                };
            //                entity.StatusUnnormal += Entity_StatusUnnormal;
            //            }
            //            else
            //            {
            //                //可编辑级别
            //                entity.Grade = (SupplierGrade)int.Parse(Request["Grade"]);
            //            }
            //            entity.EnterSuccess += suppliers_EnterSuccess;
            //            entity.Enter();

        }

        //        private void Entity_StatusUnnormal(object sender, Usually.ErrorEventArgs e)
        //        {
        //            var entity = sender as WsSupplier;
        //            Easyui.Reload("提示", "代仓储供应商已存在，供应商状态：" + entity.WsSupplierStatus.GetDescription(), Yahv.Web.Controls.Easyui.Sign.Warning);
        //        }

        //        private void suppliers_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        //        {
        //            var entity = sender as WsSupplier;
        //            //操作日志
        //            if (string.IsNullOrEmpty(Request.QueryString["id"]))
        //            {
        //                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
        //                                         nameof(Yahv.Systematic.Crm),
        //                                        "WsClientInsert", "新增代仓储供应商：" + entity.Enterprise.ID, "");
        //            }
        //            else
        //            {
        //                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
        //                                         nameof(Yahv.Systematic.Crm),
        //                                        "WsClientUpdate", "修改代仓储供应商：" + entity.Enterprise.ID, "");
        //            }
        //            //Easyui.Alert("提示", "保存成功", Yahv.Web.Controls.Easyui.Sign.Info, true, Web.Controls.Easyui.Method.Window);
        //            Easyui.Window.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        //        }
    }
}
