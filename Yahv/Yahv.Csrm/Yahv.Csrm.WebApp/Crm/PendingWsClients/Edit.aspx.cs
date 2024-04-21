using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;
using Yahv.Web.Forms;
using YaHv.Csrm.Services.Extends;
using YaHv.Csrm.Services.Models.Origins;

namespace Yahv.Csrm.WebApp.Crm.PendingWsClients
{
    public partial class Edit : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var clientid = Request.QueryString["id"];
                this.Model.Entity = Erp.Current.Whs.WsClients[clientid];
                init();
            }
        }
        void init()
        {
            //级别
            this.Model.Grade = ExtendsEnum.ToArray<ClientGrade>().Select(item => new
            {
                value = (int)item,
                text = item.GetDescription()
            });
        }
        protected void btnPass_Click(object sender, EventArgs e)
        {
            var id = Request.QueryString["id"];
            var entity = Erp.Current.Whs.WsClients[id];
            if (entity == null)
            {
                Easyui.Alert("操作提示", "审批失败!", Web.Controls.Easyui.Sign.Info, true);
            }
            string admincode = Request["AdminCode"].Trim();
            string corporation = Request["Corporation"].Trim();
            string regAddress = Request["RegAddress"].Trim();
            string uscc = Request["Uscc"].Trim();
            string entercode = Request["EnterCode"].Trim();
            string customsCode = Request["CustomsCode"].Trim();
            string summary = Request["Summary"];

            entity.CustomsCode = customsCode;
            entity.EnterCode = entercode;
            entity.Summary = summary;
            entity.Enterprise = new Enterprise
            {
                Name = Request.Form["Name"],
                AdminCode = string.IsNullOrWhiteSpace(admincode) ? "" : admincode,
                Corporation = corporation,
                RegAddress = regAddress,
                Uscc = uscc
            };
            entity.Grade = (ClientGrade)int.Parse(Request["Grade"]);
            entity.Vip = Request["Vip"] == null ? false : true;
            entity.Approve(ApprovalStatus.Normal);
            string url = this.hidurl.Value;
            string FileFormat = url.Substring(url.LastIndexOf('.') + 1);
            string FileName = url.Substring(url.LastIndexOf("/") + 1);
            var file = new FileDescription
            {
                EnterpriseID = entity.Enterprise.ID,
                Enterprise = entity.Enterprise,
                Name = FileName,
                Type = FileType.BusinessLicense,
                Url = url,
                FileFormat = FileFormat,
                CreatorID = Erp.Current.ID,
            };
            //file.Enter();
            Easyui.Window.Close("审批完成，已通过!", Web.Controls.Easyui.AutoSign.Success);
        }

        /// <summary>
        /// 审批不通过
        /// </summary>
        protected JMessage reject()
        {
            var id = Request["id"];
            var entity = Erp.Current.Whs.WsClients[id];
            return new JMessage
            {
                success = true,
                code = 200,
                data = "",
            };
        }

    }
}