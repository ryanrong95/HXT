//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.UI;
//using System.Web.UI.WebControls;
//using Yahv.Underly;
//using Yahv.Web.Forms;
//using YaHv.Csrm.Services.Extends;
//using YaHv.Csrm.Services.Models.Origins;
//using YaHv.Csrm.Services.Views.Rolls;

//namespace Yahv.Csrm.WebApp.Srm.PendingWsSuppliers
//{
//    public partial class Edit : BasePage
//    {
//        protected void Page_Load(object sender, EventArgs e)
//        {
//            if (!IsPostBack)
//            {
//                var id = Request.QueryString["id"];
//                this.Model.Entity = Erp.Current.Srm.WsSuppliers[id];
//                this.Model.Grade = ExtendsEnum.ToArray<SupplierGrade>().Select(item => new
//                {
//                    value = (int)item,
//                    text = item.GetDescription()
//                });
//                this.Model.SupplierNature = ExtendsEnum.ToArray<SupplierNature>().Select(item => new
//                {
//                    value = (int)item,
//                    text = item.GetDescription()
//                });
//                this.Model.SupplierType = ExtendsEnum.ToArray<SupplierType>().Select(item => new
//                {
//                    value = (int)item,
//                    text = item.GetDescription()
//                });
//                this.Model.InvoiceType = ExtendsEnum.ToArray<InvoiceType>().Select(item => new
//                {
//                    value = (int)item,
//                    text = item.GetDescription()
//                });
//                this.Model.Currency = ExtendsEnum.ToArray<Currency>().Select(item => new
//                {
//                    value = (int)item,
//                    text = item.GetDescription()
//                });
//            }
//        }

//        // 审批通过
//        protected void btnPass_Click(object sender, EventArgs e)
//        {
//            var id = Request.QueryString["id"];
//            var entity = Erp.Current.Srm.WsSuppliers[id];
//            if (entity == null)
//            {
//                Easyui.Window.Close("操作失败!", Yahv.Web.Controls.Easyui.AutoSign.Warning);
//            }
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
//            entity.Grade = (SupplierGrade)int.Parse(Request["Grade"]);
//            entity.Approve(ApprovalStatus.Normal);
//            Easyui.Window.Close("审批完成，已通过!", Yahv.Web.Controls.Easyui.AutoSign.Success);
//        }

//        /// <summary>
//        /// 审批不通过
//        /// </summary>
//        protected JMessage reject()
//        {
//            var id = Request["id"];
//            var entity = Erp.Current.Srm.WsSuppliers[id];
//            if (entity != null)
//            {
//                entity.Approve(ApprovalStatus.Voted);
//                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
//                                          nameof(Yahv.Systematic.Crm),
//                                         "WsSupplierApprove", "代仓储供应商" + entity.ID + "审批不通过", "");
//                return new JMessage
//                {
//                    success = true,
//                    code = 200,
//                    data = "",
//                };
//            }
//            else
//            {
//                return new JMessage
//                {
//                    success = true,
//                    code = 300,
//                    data = "内部错误",
//                };
//            }
//        }
//    }
//}