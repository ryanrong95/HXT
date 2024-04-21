using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.CrmPlus.WebApp.Srm.Suppliers
{
    public partial class QuickRegister : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                #region  参数
                //基本信息
                var name = Request.Form["Name"].Trim();
                var supplierType = Request.Form["SupplierType"].Trim();
                var invoiceType = Request.Form["InvoiceType"].Trim();
                var orderType = Request.Form["OrderType"].Trim();
                bool isInternation = Request.Form["IsInternational"] != null;
                bool isfixed = Request.Form["IsFixed"] != null;
                var area = Request.Form["Area"].Trim();
                //证照
                var license = Request.Form["licenseForJson"];
                //工商信息
                var uscc = Request.Form["Uscc"].Trim();
                var place = Request.Form["Place"];
                var currency = Request.Form["Currency"];
                var adderss = Request.Form["Address"];
                #endregion

                var entity = new Yahv.CrmPlus.Service.Models.Origins.Supplier();

                #region 供应商信息
                entity.Type = (Underly.CrmPlus.SupplierType)int.Parse(supplierType);
                entity.InvoiceType = (Underly.InvoiceType)int.Parse(invoiceType);
                entity.OrderType = (Underly.CrmPlus.OrderType)int.Parse(orderType);
                entity.IsFixed = isfixed;
                entity.CreatorID = Erp.Current.ID;
                #endregion

                #region 企业信息
                entity.IsDraft = true;
                entity.Name = name;
                entity.Place = place;
                entity.District = area;
                entity.Status = AuditStatus.Waiting;
                #endregion

                #region 工商信息
                if (!isInternation)
                {
                    entity.EnterpriseRegister = new Service.Models.Origins.EnterpriseRegister()
                    {
                        IsSecret = false,
                        IsInternational = false,
                        Uscc = uscc,
                    };
                }
                else
                {
                    entity.EnterpriseRegister = new Service.Models.Origins.EnterpriseRegister()
                    {
                        IsSecret = false,
                        IsInternational = true,
                        Currency = (Currency)int.Parse(currency),
                        RegAddress = adderss,
                    };
                }
                #endregion

                #region 附件
                if (!string.IsNullOrEmpty(license))
                {
                    entity.Lisences = license.JsonTo<List<CallFile>>();
                }
                #endregion

                entity.EnterSuccess += Supplier_EnterSuccess;
                entity.Enter();
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }
        }

        private void Supplier_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var supplier = sender as Yahv.CrmPlus.Service.Models.Origins.Supplier;
            //记录操作日志
            Service.LogsOperating.LogOperating(Erp.Current, supplier.ID, $"快速注册供应商:{supplier.Name}");
            //新增供应商注册审批
            var applyTask = new ApplyTask
            {
                MainID = supplier.ID,
                MainType = MainType.Suppliers,
                ApplierID = Erp.Current.ID,
                ApplyTaskType = ApplyTaskType.SupplierRegist,
            };
            applyTask.Enter();

            Easyui.Dialog.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }
    }
}