using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Forms;
using YaHv.Csrm.Services;
using YaHv.Csrm.Services.Extends;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApp.Crm.WsClients
{
    public partial class Edit : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var clientid = Request.QueryString["id"];
                string companyid = Request.QueryString["CompanyID"];
                var client = Erp.Current.Whs.WsClients[companyid, clientid];
                if (client != null)
                {
                    client.Enterprise.Name = client.Enterprise.Name.Replace("reg-", "");
                }
                this.Model.Entity = client;
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
            //客户类型
            this.Model.ClientType = ExtendsEnum.ToArray<ClientType>(ClientType.OEM, ClientType.Unknown).Select(item => new
            {
                value = (int)item,
                text = item.GetDescription()
            });
            this.Model.ServiceType = ExtendsEnum.ToArray<ServiceType>(ServiceType.Unknown).Select(item => new
            {
                value = (int)item,
                text = item.GetDescription()
            });
            this.Model.Indentify = ExtendsEnum.ToArray<WsIdentity>(WsIdentity.Unknown).Select(item => new
            {
                value = (int)item,
                text = item.GetDescription()
            });
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var id = Request.QueryString["id"];
            string companyid = Request.QueryString["CompanyID"];
            var wsclient = Erp.Current.Whs.WsClients[companyid, id];
            var entity = wsclient ?? new WsClient();
            //string admincode = Request["AdminCode"].Trim();
            string corporation = Request["Corporation"].Trim();
            string regAddress = Request["RegAddress"].Trim();
            string uscc = Request["Uscc"].Trim();

            string customsCode = Request["CustomsCode"].Trim();
            string summary = Request["Summary"];

            string url = hidurl.Value;
            entity.CustomsCode = customsCode;
            entity.Summary = summary;
            entity.Enterprise = new Enterprise
            {
                Name = Request.Form["Name"].Trim(),
                //AdminCode = string.IsNullOrWhiteSpace(admincode) ? "" : admincode,
                Corporation = corporation,
                RegAddress = regAddress,
                Uscc = uscc
            };
            entity.Nature = (ClientType)int.Parse(Request["Nature"]);
            entity.Grade = (ClientGrade)int.Parse(Request["Grade"]);
            entity.Vip = Request["Vip"] != null;
            entity.Place = Request["Origin"];

            //ServiceType servicetype = ServiceType.Unknown;
            //string[] service = Request["ServiceType"].Split(',');
            //foreach (var item in service)
            //{
            //    servicetype |= (ServiceType)int.Parse(item);
            //}
            ServiceType servicetype = (ServiceType)int.Parse(Request["ServiceType"]);
            entity.ServiceType = servicetype;
            if (servicetype == ServiceType.Warehouse)
            {
                entity.StorageType = (WsIdentity)int.Parse(Request["Identity"]);
            }
            else
            {
                entity.StorageType = WsIdentity.Unknown;
            }

            if (wsclient == null)
            {
                entity.Company = new CompaniesRoll()[companyid].Enterprise;
                //录入人
                entity.CreatorID = Yahv.Erp.Current.ID;
                entity.EnterCode = Request.Form["radio_EnterType"];
                entity.StatusUnnormal += Entity_StatusUnnormal;
            }
            entity.EnterSuccess += Clients_EnterSuccess;
            entity.Enter();
        }

        private void Entity_StatusUnnormal(object sender, Usually.ErrorEventArgs e)
        {
            var entity = sender as WsClient;

            Easyui.Reload("提示", "代仓储客户已存在，客户状态：" + entity.WsClientStatus.GetDescription(), Yahv.Web.Controls.Easyui.Sign.Warning);
        }

        private void Clients_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as WsClient;
            //Logo
            string logourl = hidUrl1.Value;
            string logoformat = hidFormat1.Value;
            string logoname = hidName1.Value;
            string oldLogoUrl = entity.Logo == null ? "" : entity.Logo.Url;
            if (!string.IsNullOrWhiteSpace(logourl) && logourl != oldLogoUrl)
            {
                var file = new FileDescription
                {
                    EnterpriseID = entity.Enterprise.ID,
                    Enterprise = entity.Enterprise,
                    Name = logoname,
                    Type = FileType.EnterpriseLogo,
                    Url = logourl,
                    FileFormat = logoformat,
                    CreatorID = Erp.Current.ID,
                };
                file.EnterSuccess += File_EnterSuccess;
                file.Enter();
            }
            //营业执照
            var url = hidurl.Value;
            string oldurl = entity.BusinessLicense == null ? "" : entity.BusinessLicense.Url;
            if (!string.IsNullOrWhiteSpace(url) && url != oldurl)
            {
                var file = new FileDescription
                {
                    EnterpriseID = entity.Enterprise.ID,
                    CompanyID = Request.QueryString["CompanyID"],
                    Enterprise = entity.Enterprise,
                    Name = hidName.Value,
                    Type = FileType.BusinessLicense,
                    Url = url,
                    FileFormat = hidFormat.Value,
                    CreatorID = Erp.Current.ID
                };
                file.EnterSuccess += File_EnterSuccess;
                file.Enter();
            }
            else
            {
                if (entity.ServiceManager != null && entity.Contact != null)
                {
                    string apiurl = System.Configuration.ConfigurationManager.AppSettings["UnifyApiUrl"];
                    //芯达通客户时同步
                    string companyid = Request.QueryString["CompanyID"];
                    if (!string.IsNullOrWhiteSpace(apiurl) && entity.Company.ID == companyid)
                    {
                        Unify(apiurl, entity);
                    }
                }
                Easyui.Window.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
            }
        }

        private void File_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as FileDescription;
            var data = new WsClientsRoll()[entity.EnterpriseID];
            //添加人的角色是业务员的话分配业务员
            if (Erp.Current.Role.ID == FixedRole.ServiceManager.GetFixedID())
            {
                data.Assin(Erp.Current.ID, MapsType.ServiceManager);
            }
            //有业务员和联系人才能同步数据
            if (data.ServiceManager != null && data.Contact != null)
            {
                string apiurl = System.Configuration.ConfigurationManager.AppSettings["UnifyApiUrl"];
                string companyid = Request.QueryString["CompanyID"];//只有芯达通客户同步
                if (!string.IsNullOrWhiteSpace(apiurl) && companyid == "DBAEAB43B47EB4299DD1D62F764E6B6A")
                {
                    Unify($"{apiurl}/clients", data);
                }
            }
            Easyui.Window.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }
        object Unify(string api, WsClient data)
        {
            var grade = (int)data.Grade;
            var bussiness = data.BusinessLicense == null ? null : new
            {
                CompanyID = Request.QueryString["CompanyID"],
                Name = data.BusinessLicense.CustomName,
                Type = FileType.BusinessLicense,
                Url = data.BusinessLicense.Url,
                CreatorID = data.BusinessLicense.AdminID
            };
            var json = new
            {
                Enterprise = data.Enterprise,
                EnterCode = data.EnterCode,
                Contact = data.Contact,
                CustomsCode = data.CustomsCode,
                Rank = grade,
                BusinessLicense = bussiness,
                Creator = data.ServiceManager == null ? "" : data.ServiceManager.RealName,
                Status = data.WsClientStatus,
                ClientNature = (int)data.Nature,
                ServiceType = (int)data.ServiceType,
                StoragesType = (int)data.StorageType

            }.Json();
            var response = HttpClientHelp.HttpPostRaw($"{api}/clients", json);
            return response;
        }
        /// <summary>
        /// 客户是否存在
        /// </summary>
        /// <returns></returns>
        protected bool CheckEnterprise()
        {
            string name = Request["Name"];
            bool flag = Erp.Current.Whs.WsClients.Any(item => item.Enterprise.Name == name);
            return flag;
        }
    }
}