using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Web.Erp;
using YaHv.Csrm.Services.Models.Origins;
using Yahv.Web.Forms;
using Yahv.Underly;

namespace Yahv.Csrm.WebApp.Crm.WsContracts
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string CompanyID = Request.QueryString["CompanyID"];
                this.Model.CompanyID = CompanyID;
                var wsclient = Erp.Current.Whs.WsClients[Request.QueryString["id"]];
                this.Model.Trustee = "HK LIANCHUANG ELECTRONICS., LIMITED";
                this.Model.WsClient = new { ID = wsclient.ID, Name = wsclient.Enterprise.Name };
                var contract = Erp.Current.Whs.WsClients[CompanyID, Request.QueryString["id"]].WsContract;
                this.Model.WsContract = contract;
                this.Model.File = contract?.Agreement;
                this.Model.NotShowBtnSave = Erp.Current.Role.ID == FixedRole.ServiceManager.GetFixedID() && wsclient.WsClientStatus == ApprovalStatus.Normal;
                this.Model.Nonstandard = wsclient.Enterprise.Name.StartsWith("reg-", StringComparison.OrdinalIgnoreCase);
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var clientid = Request.QueryString["id"];
            string Trustee = "HK LIANCHUANG ELECTRONICS., LIMITED";
            if (string.IsNullOrEmpty(clientid))
            {
                return;
            }
            var client = Erp.Current.Whs.WsClients[clientid];
            if (client.Enterprise.Name.StartsWith("reg-", StringComparison.OrdinalIgnoreCase))
            {
                Easyui.Reload("提示!", "请先规范客户名称", Web.Controls.Easyui.Sign.None);
            }
            var wscontract = client.WsContract ?? new WsContract();
            wscontract.Trustee = Trustee;
            wscontract.StartDate = Convert.ToDateTime(Request["StartDate"]);
            wscontract.EndDate = Convert.ToDateTime(Request["EndDate"]);
            wscontract.WsClient = client.Enterprise;
            wscontract.Currency = Underly.Currency.HKD;
            wscontract.ContainerNum = int.Parse(Request["ContainerNum"]);
            wscontract.Charges = decimal.Parse(Request["Charges"]);
            wscontract.CreatorID = Yahv.Erp.Current.ID;

            wscontract.Summary = Request["Summary"].Trim();
            wscontract.EnterSuccess += Wscontract_EnterSuccess;
            wscontract.Enter();
        }

        private void Wscontract_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            //var url = hidurl.Value;
            //var wscontract = sender as WsContract;
            //var wsclient = Erp.Current.Whs.WsClients[wscontract.WsClient.ID];
            //string oldurl = wscontract.Agreement == null ? "" : wscontract.Agreement.Url;

            //if (!string.IsNullOrWhiteSpace(url) && url != oldurl)
            //{
            //    var file = new FileDescription
            //    {
            //        EnterpriseID = wsclient.ID,
            //        CompanyID = wscontract.Trustee,
            //        Enterprise = wsclient.Enterprise,
            //        Name = hidName.Value,
            //        Type = Underly.FileType.WsAgreement,
            //        Url = url,
            //        FileFormat = hidFormat.Value,
            //        CreatorID = Erp.Current.ID

            //    };
            //    file.Enter();
            //}
            Easyui.Reload("提示!", "保存成功", Web.Controls.Easyui.Sign.None);
        }

        /// <summary>
        /// 导出协议word
        /// </summary>
        protected object ExportAgreement()
        {
            try
            {
                var clientid = Request.Form["clientid"];
                var wsclient = Erp.Current.Whs.WsClients[clientid];
                //创建文件夹
                var fileName = DateTime.Now.Ticks + "委托代收、代发、代管协议书.docx";

                //var fileurl = Yahv.Utils.FileServices.Save();
                var dirPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files\\Dowload\\WsContracts");
                if (!System.IO.Directory.Exists(dirPath))
                {
                    System.IO.Directory.CreateDirectory(dirPath);
                }
                var path = System.IO.Path.Combine(dirPath, fileName);
                string fileurl = $"../../Files/Dowload/WsContracts/{fileName}";
                //保存文件
                wsclient.WsContract.SaveAs(path);

                return new { success = true, message = "导出成功", url = fileurl };
                //return null;
            }
            catch (Exception ex)
            {
                return new { success = false, message = "导出失败" + ex.Message };
            }
        }
        /// <summary>
        /// 导出协议word
        /// </summary>
        protected object PreviewAgreement()
        {
            try
            {
                var clientid = Request.Form["clientid"];
                var wsclient = Erp.Current.Whs.WsClients[clientid];
                //创建文件夹
                var fileName = DateTime.Now.Ticks + "委托代收、代发、代管协议书.html";

                //var fileurl = Yahv.Utils.FileServices.Save();
                var dirPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files\\Dowload\\WsContracts");
                if (!System.IO.Directory.Exists(dirPath))
                {

                    System.IO.Directory.CreateDirectory(dirPath);
                }
                var path = System.IO.Path.Combine(dirPath, fileName);
                string fileurl = $"../../Files/Dowload/WsContracts/{fileName}";
                //保存文件
                wsclient.WsContract.SaveAs(path, true);

                return new { success = true, message = "成功", url = fileurl };
                //return null;
            }
            catch (Exception ex)
            {
                return new { success = false, message = "失败" + ex.Message };
            }
        }


    }
}