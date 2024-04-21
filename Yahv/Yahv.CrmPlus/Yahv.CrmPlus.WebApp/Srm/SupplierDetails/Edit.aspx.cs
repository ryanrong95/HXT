using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Web.Erp;
using Yahv.CrmPlus.Service;
using Yahv.Underly;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.CrmPlus.Service.Models.Origins;

namespace Yahv.CrmPlus.WebApp.Srm.SupplierDetails
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var supplier = Erp.Current.CrmPlus.Suppliers[Request.QueryString["enterpriseid"]];
                this.Model.Entity = supplier;
                this.Model.LogoUrl = new FilesDescriptionRoll()[supplier.ID, CrmFileType.Logo].FirstOrDefault()?.Url;
                
               
            }
        }
        protected object Protect()
        {
            string supplierid = Request.Form["ID"];
            var supplier = Erp.Current.CrmPlus.Suppliers[supplierid];
            if (supplier.OwnerID == Erp.Current.ID)
            {
                return new { success = false, message = "已被保护" };
            }
            var tasks = ApplyTasks.All(ApplyTaskType.SupplierProtected);
            if (tasks.Any(item => item.MainID == supplier.ID
            && item.ApplierID == Erp.Current.ID && item.Status == ApplyStatus.Waiting))
            {
                return new { success = false, message = "已申请保护，正在审批中......" };
            }
            (new Service.Models.Origins.ApplyTask
            {
                MainID = supplierid,
                MainType = MainType.Suppliers,
                ApplierID = Erp.Current.ID,
                ApplyTaskType = ApplyTaskType.SupplierProtected,
                Status = ApplyStatus.Waiting
            }).Enter();
            return new { success = true, message = "" };
        }
        bool calcelResult = false;
        protected object CancelProtect()
        {
            string supplierid = Request.Form["ID"];
            var supplier = Erp.Current.CrmPlus.Suppliers[supplierid];
            supplier.CancelProtectSuccess += Supplier_CancelProtectSuccess;
            supplier.CancelProtect();
            return calcelResult;
        }

        private void Supplier_CancelProtectSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as Supplier;
            Service.LogsOperating.LogOperating(Erp.Current, entity.ID, $"撤销Supplier:'{entity.Name}'的保护人:{entity.OwnerAdmin.RealName}");
            calcelResult = true;
        }
    }
}