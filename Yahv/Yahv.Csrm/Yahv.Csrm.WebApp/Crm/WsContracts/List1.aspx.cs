using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using YaHv.Csrm.Services;

namespace Yahv.Csrm.WebApp.Crm.WsContracts
{
    public partial class List1 : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string CompanyID = Request.QueryString["CompanyID"];
                this.Model.CompanyID = CompanyID;
                var client = Erp.Current.Whs.WsClients[Request.QueryString["id"]];
                this.Model.ClientID = client.ID;
                this.Model.PartA = client.Enterprise.Name;
                this.Model.File = client.StorageAgreement;
                if (client.ServiceType == ServiceType.Warehouse || client.ServiceType == ServiceType.Both)
                {

                    switch (client.StorageType)
                    {
                        case WsIdentity.Mainland:
                            this.Model.PartB = "深圳市芯达通供应链管理有限公司";
                            break;
                        case WsIdentity.HongKong:
                            this.Model.PartB = "香港畅运国际物流有限公司";
                            break;
                        case WsIdentity.Personal:
                            this.Model.PartB = "深圳市芯达通供应链管理有限公司";
                            break;
                        default:
                            this.Model.PartB = null;
                            break;
                    }
                }
                else
                {

                    this.Model.PartB = null;
                }
            }
        }

        /// <summary>
        /// 导出协议word
        /// </summary>
        protected object ExportAgreement()
        {
            try
            {
                var clientid = Request.Form["clientid"];
                //var wsclient = Erp.Current.Whs.WsClients[clientid];
                //创建文件夹
                var fileName = DateTime.Now.Ticks + "香港本地交货协议-芯达通.docx";

                //var fileurl = Yahv.Utils.FileServices.Save();
                var dirPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files\\Dowload\\StorageAgreements");
                if (!System.IO.Directory.Exists(dirPath))
                {
                    System.IO.Directory.CreateDirectory(dirPath);
                }
                var path = System.IO.Path.Combine(dirPath, fileName);
                string fileurl = $"../../Files/Dowload/StorageAgreements/{fileName}";
                //保存文件
                //wsclient.WsContract.SaveAs(path);
                StorageAgreement.Export(clientid,path);

                return new { success = true, message = "导出成功", url = fileurl };
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
                //var client = Erp.Current.Whs.WsClients[clientid];
                //创建文件夹
                var fileName = DateTime.Now.Ticks + "香港本地交货协议-芯达通.html";

                //var fileurl = Yahv.Utils.FileServices.Save();
                var dirPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files\\Dowload\\StorageAgreements");
                if (!System.IO.Directory.Exists(dirPath))
                {
                    System.IO.Directory.CreateDirectory(dirPath);
                }
                var path = System.IO.Path.Combine(dirPath, fileName);
                string fileurl = $"../../Files/Dowload/StorageAgreements/{fileName}";
                //保存文件
               
                StorageAgreement.Export(clientid, path, true);

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