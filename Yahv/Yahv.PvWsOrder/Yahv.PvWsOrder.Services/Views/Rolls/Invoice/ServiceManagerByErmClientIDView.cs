using Layers.Data.Sqls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.Views
{
    /// <summary>
    /// 通过 ErmClientID 查询业务员信息
    /// </summary>
    public class ServiceManagerByErmClientIDView
    {
        private string _CrmClientID { get; set; }

        private InvoiceFromType _FromType { get; set; }

        public ServiceManagerByErmClientIDView(string crmClientID, InvoiceFromType fromType)
        {
            this._CrmClientID = crmClientID;
            this._FromType = fromType;
        }

        public ServiceManagerByErmClientIDViewModel GetServiceManagerInfo()
        {
            if (this._FromType == InvoiceFromType.SZStore)
            {
                var apiResult = GetServiceManagerInfoBySZClientID();
                if (apiResult.success == false)
                {
                    return null;
                }

                return new ServiceManagerByErmClientIDViewModel
                {
                    CrmClientID = this._CrmClientID,
                    ErmAdminID = apiResult.serviceManager.AdminID,
                    RealName = apiResult.serviceManager.RealName,
                    Mobile = apiResult.serviceManager.Mobile,
                };
            }

            ServiceManagerByErmClientIDViewModel result;

            using (var PvWsOrderReponsitory = new PvWsOrderReponsitory())
            using (var foricScCustomsReponsitory = new foricScCustomsReponsitory())
            using (var PvbErmReponsitory = new PvbErmReponsitory())
            {
                string companyName = PvWsOrderReponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.WsClientsTopView>()
                    .Where(t => t.ID == this._CrmClientID).Select(t => t.Name).FirstOrDefault();
                if (string.IsNullOrEmpty(companyName))
                {
                    return null;
                }

                var foricClientInfo = (from client in foricScCustomsReponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.Clients>()
                                       join company in foricScCustomsReponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.Companies>()
                                            on client.CompanyID equals company.ID
                                       where company.Name == companyName && client.Status == (int)GeneralStatus.Normal
                                       select new
                                       {
                                           ClientID = client.ID,
                                           CompanyName = company.Name,
                                       }).FirstOrDefault();
                if (foricClientInfo == null)
                {
                    return null;
                }

                var clientAdminInfo = (from clientAdmin in foricScCustomsReponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.ClientAdmins>()
                                       join admin in foricScCustomsReponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.AdminsTopView2>()
                                              on clientAdmin.AdminID equals admin.OriginID
                                       where clientAdmin.Status == (int)GeneralStatus.Normal
                                          && clientAdmin.Type == 1
                                          && clientAdmin.ClientID == foricClientInfo.ClientID
                                       select new
                                       {
                                           OriginAdminID = clientAdmin.AdminID,
                                           RealName = admin.RealName,
                                           ErmAdminID = admin.ID,
                                       }).FirstOrDefault();
                if (clientAdminInfo == null)
                {
                    return null;
                }

                result = new ServiceManagerByErmClientIDViewModel
                {
                    CrmClientID = this._CrmClientID,
                    XDTClientID = foricClientInfo.ClientID,
                    ErmAdminID = clientAdminInfo.ErmAdminID,
                    OriginAdminID = clientAdminInfo.OriginAdminID,
                    RealName = clientAdminInfo.RealName,
                };

                var staffID = PvbErmReponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Admins>()
                    .Where(t => t.ID == clientAdminInfo.ErmAdminID).Select(t => t.StaffID).FirstOrDefault();
                if (!string.IsNullOrEmpty(staffID))
                {
                    var mobile = PvbErmReponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Personals>().Where(t => t.ID == staffID).Select(t => t.Mobile).FirstOrDefault();
                    if (!string.IsNullOrEmpty(mobile))
                    {
                        result.Mobile = mobile;
                    }
                }

                return result;
            }
        }

        private SZGetServiceManagerInfoResult GetServiceManagerInfoBySZClientID()
        {
            try
            {
                var strJsonReq = JsonConvert.SerializeObject(new { ClientID = this._CrmClientID, });

                string SzApiUrl = ConfigurationManager.AppSettings["SzApiUrl"];
                string url = string.Join(@"/", SzApiUrl, "ClientApi/GetServiceManagerInfo");

                string apiResult = string.Empty;
                using (WebClient client = new WebClient { Encoding = Encoding.UTF8 })
                {
                    client.Headers.Add("Content-Type", "application/json");
                    client.Headers.Add("Accept", "application/json");
                    client.Headers.Add("User-Agent", "POST");
                    apiResult = client.UploadString(url, "POST", strJsonReq);
                }

                var apiResultModel = JsonConvert.DeserializeObject<SZGetServiceManagerInfoResult>(apiResult);
                return apiResultModel;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public class SZGetServiceManagerInfoResult
        {
            public bool success { get; set; }

            public string msg { get; set; }

            public ServiceManager serviceManager { get; set; }
        }

        public class ServiceManager
        {
            /// <summary>
            /// 
            /// </summary>
            public string AdminID { get; set; }
            /// <summary>
            /// 赖翠红
            /// </summary>
            public string RealName { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string Mobile { get; set; }
        }
    }

    public class ServiceManagerByErmClientIDViewModel
    {

        public string CrmClientID { get; set; } = string.Empty;

        public string XDTClientID { get; set; } = string.Empty;

        public string ErmAdminID { get; set; } = string.Empty;

        public string OriginAdminID { get; set; } = string.Empty;

        public string RealName { get; set; } = string.Empty;

        public string Mobile { get; set; } = string.Empty;
    }
}
