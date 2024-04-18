//using Needs.Underly;
//using Needs.Utils.Serializers;
//using NtErp.Crm.Services.Enums;
//using NtErp.Crm.Services.Extends;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.UI;
//using System.Web.UI.WebControls;


//namespace WebApp.Crm.Reports
//{
//    public partial class Edit : Needs.Web.Sso.Forms.ErpPage
//    {
//        protected void Page_Load(object sender, EventArgs e)
//        {
//            if (!IsPostBack)
//            {
//                LoadComboBoxData();
//                LoadData();
//            }
//        }   


//        /// <summary>
//        /// 加载数据
//        /// </summary>
//        protected void LoadData()
//        {
//            string id = Request.QueryString["ID"];
//            string ItemID = Request.QueryString["ItemID"];
//            var Report = new NtErp.Crm.Services.Views.ReportsAlls().Where(item => item.ID == id).SingleOrDefault();
//           // var Report = Needs.Erp.ErpPlot.Current.ClientSolutions.Reports[id];          

//            if (Report!=null)
//            {                
//                this.Model.ItemData = Report.Context;
//            }
//            else
//            {
//                this.Model.ItemData = "".Json();
//            }
//        }

//        private void Plan_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
//        {
//            Alert("保存成功", Request.Url, true);
//        }

//        private void Client_Rename(object sender, Needs.Linq.ErrorEventArgs e)
//        {
//            Alert(e.Message, Request.Url, true);
//        }

//        protected void btnSumit_Click(object sender, EventArgs e)
//        {
//            string ActionID = Request.QueryString["ActionID"];
//            string id = Request.QueryString["ID"];
//            var report = new NtErp.Crm.Services.Views.ReportsAlls().Where(item=>item.ID==id).SingleOrDefault();
//             report = report==null? new NtErp.Crm.Services.Models.Report():report ;
//            //report.Actions = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.Plan>.Create(ActionID);
//            if (!string.IsNullOrEmpty(id))
//            {
//                report.ID = id;
//            }                   
//            //var reportItem = new NtErp.Crm.Services.Models.ReportItem();
              
//            foreach (var key in Request.Form.AllKeys)
//            {
//                if (key == "__VIEWSTATE" || key == "__VIEWSTATEGENERATOR" || key == "__EVENTVALIDATION" || key == "btnSumit")
//                {

//                }
//                else
//                {
//                    if (key == "FollowUpDate" || key == "NextFollowUpDate")
//                    {
//                        report[key] = Convert.ToDateTime(Request.Form[key]);
//                    }
//                    else
//                    {
//                        report[key] = Request.Form[key];
//                    }
//                }
//            }
//            report["FollowUpAdminID"] = Needs.Erp.ErpPlot.Current.ID;
//            report["FollowUpAdminName"] = Needs.Erp.ErpPlot.Current.RealName;
//            FollowUpMethod method = (FollowUpMethod)Convert.ToInt16(report["FollowUpMethod"]);
//            string methodname = Needs.Utils.Descriptions.Extends.GetDescription(method);
//            report["FollowUpMethodName"] = methodname;                              
//            //report.ActionID = ActionID;  
//            report.Admin.ID = Needs.Erp.ErpPlot.Current.ID;
//            report.EnterSuccess += Report_EnterSuccess;
//            report.Enter();

//        }


//        /// <summary>
//        /// 保存成功关闭弹出框
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        private void Report_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
//        {
//            var url = Request.UrlReferrer ?? Request.Url;
//            this.Alert("保存成功", url, true);
//        }

//        protected void LoadComboBoxData()
//        {
//            var FollowUpMethod = Needs.Utils.Descriptions.EnumUtils.ToDictionary<FollowUpMethod>();
//            this.Model.DrpFollowUpMethod = FollowUpMethod.Select(item => new { value = item.Key, text = item.Value }).Json();          
//        }

//        protected string GetContract()
//        {
//            string ID = Request.Form["ID"];
//            var contract = Needs.Erp.ErpPlot.Current.ClientSolutions.Contacts;
//            if (!string.IsNullOrWhiteSpace(ID))
//            {
//                var data = contract.Where(item => item.ClientID == ID);

//                var result = from item in data
//                             select new
//                             {
//                                 text = item.Name,
//                                 value = item.ID
//                             };

//                return result.ToList().Json();
//            }
            
//            return "";
//        }
//    }
//}