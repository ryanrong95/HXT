using System;
using System.Linq;
using Yahv.Erm.Services;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Erm.Services.Views.Rolls;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.Erm.WebApp.Erm_KQ.Staffs
{
    public partial class TabApproval : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Pass()
        {
            try
            {
                string id = Request.Form["ID"];
                var staff = Alls.Current.Staffs[id];
                var staffCurrent = Alls.Current.Staffs[Erp.Current.StaffID];
                if (staff != null)
                {
                    //新增员工信息添加到大赢家系统
                    var result = Services.Common.DYJRSAPI.Instance.SetUserInfo(staff, staffCurrent);
                    Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "员工信息",
                        $"员工数据通大赢家结果", result);

                    //入职审批-》通过
                    staff.Approval(true, Erp.Current.ID);
                    Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "员工信息",
                        $"审批通过", staff.Json());

                    //账号权限开通(前提一个员工一个账号)
                    staff.Admin.PermissionOpen();
                    Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "员工信息",
                        $"账号权限开通", staff.Admin.Json());


                    Response.Write((new { success = true, message = "成功" }).Json());
                }
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "失败" + ex.Message }).Json());
            }
        }

        protected void Reject()
        {
            string id = Request.Form["ID"];
            var staff = Alls.Current.Staffs[id];
            if (staff != null)
            {
                staff.Approval(false, Erp.Current.ID);
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "员工信息",
                    $"审批驳回", staff.Json());
            }
        }
    }
}