using System;
using Yahv;
using Yahv.Web.Erp;

namespace WebApp
{
    public partial class Panels : ErpSsoPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    this.spanUsername.InnerHtml = Yahv.Erp.Current.RealName;

                    this.userIphone.InnerHtml = "未填写";
                    this.userEmail.InnerHtml = "未填写";

                    this.Model.StaffID = string.Empty;
                    this.Model.IsBind = false;
                    this.Title = Yahv.Erp.Current.RealName + "-" + "远大协同管理系统";

                    //荣检要求增加
                    if (Yahv.Erp.Current.Plat.MyMenus.IsXdt)
                    {
                        this.Title = Yahv.Erp.Current.RealName + "-" + "芯达通协同系统";
                    }

                    //Yahv.Erp.Current.Role.Name


                    if (!string.IsNullOrWhiteSpace(Erp.Current.StaffID))
                    {
                        using (var staffs = new Yahv.Plats.Services.Views.Origins.StaffsOrigin())
                        using (var staffWxView = new Yahv.Plats.Services.Views.Origins.StaffWxsOrigin())
                        {
                            var staff = staffs[Erp.Current.StaffID];
                            if (staff != null)
                            {
                                this.userIphone.InnerHtml = staff.Mobile;
                                this.userEmail.InnerHtml = staff.Email;
                            }

                            this.Model.StaffID = Erp.Current.StaffID;
                            this.Model.IsBind = staffWxView[Erp.Current.StaffID] == null ? false : true;
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}