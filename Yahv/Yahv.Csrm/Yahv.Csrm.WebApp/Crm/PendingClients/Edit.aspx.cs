using System;
using System.Linq;
using Yahv.Underly;
using Yahv.Web.Forms;
using YaHv.Csrm.Services;
using YaHv.Csrm.Services.Extends;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApp.Crm.PendingClients
{
    //审批
    public partial class Edit : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.Entity = new ClientsRoll()[Request.QueryString["id"]] ?? new Client();
                this.Model.Grade = ExtendsEnum.ToArray<ClientGrade>().Select(item => new
                {
                    value = (int)item,
                    text = item.GetDescription()
                });
                //地区类型
                this.Model.AreaType = ExtendsEnum.ToArray<AreaType>().Select(item => new
                {
                    value = (int)item,
                    text = item.GetDescription()
                });
                //客户类型
                this.Model.ClientType = ExtendsEnum.ToArray<ClientType>().Select(item => new
                {
                    value = (int)item,
                    text = item.GetDescription()
                });

                //客户Vip等级
                this.Model.VipRank = ExtendsEnum.ToArray<VIPLevel>().Select(item => new
                {
                    value = (int)item,
                    text = (int)item == 0 ? "非Vip" : item.GetDescription()
                });
            }
        }
        // 审批通过
        protected void btnPass_Click(object sender, EventArgs e)
        {
            var id = Request.QueryString["id"];
            var entity = new YaHv.Csrm.Services.Views.Rolls.ClientsRoll()[id] ?? new Client();
            if (entity != null)
            {
                entity.Nature = (ClientType)int.Parse(Request["Nature"]);
                entity.AreaType = (AreaType)int.Parse(Request["Type"]);
                 string taxpernumber = Request.Form["TaxperNumber"].Trim();
                string corporation = Request["Corporation"].Trim();
                string regAddress = Request["RegAddress"].Trim();
               // string uscc = Request["Uscc"].Trim();
                entity.TaxperNumber = string.IsNullOrWhiteSpace(taxpernumber) ? "" : taxpernumber;
                string dyjcode = Request["DyjCode"].Trim();
                entity.DyjCode = string.IsNullOrWhiteSpace(dyjcode) ? "" : dyjcode;
                string admincode = Request["AdminCode"].Trim();
                entity.Grade = (ClientGrade)int.Parse(Request["grade"]);
                //0:非VIP
                var vip= int.Parse(Request.Form["radio_vipRank"]);
                entity.Vip = (VIPLevel)vip;
                //重点客户
                entity.Major = Request["Major"] != null;

                entity.Enterprise = new Enterprise
                {
                    Name = Request.Form["Name"],
                    AdminCode = string.IsNullOrWhiteSpace(admincode) ? "" : admincode,
                    Corporation = corporation,
                    RegAddress = regAddress,
                    Uscc = taxpernumber
                };
                ///国家或地区
                entity.Place = Request["Origin"];
                    //Enum.GetValues(typeof(Origin)).Cast<Origin>().SingleOrDefault(item => item.GetOrigin().Code == Request["Origin"]);
                entity.Approve(ApprovalStatus.Normal);
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                             nameof(Yahv.Systematic.Crm),
                                            "ClientApprove", "客户" + entity.ID+"审批通过", "");
                Easyui.Alert("操作提示", "审批通过操作成功!", Web.Controls.Easyui.Sign.Info, true);
            }
            else
            {
                Easyui.Alert("操作提示", "审批失败!", Web.Controls.Easyui.Sign.Info, true);
            }
        }
        /// <summary>
        /// 审批不通过
        /// </summary>
        protected JMessage reject()
        {
            var id = Request["id"];
            var entity = new ClientsRoll()[id];
            if (entity != null)
            {
                entity.Approve(ApprovalStatus.Voted);
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                            nameof(Yahv.Systematic.Crm),
                                           "ClientApprove", "客户" + entity.ID + "审批不通过", "");
            }
            return new JMessage
            {
                success = true,
                code = 200,
                data = "",
            };
        }


    }
}