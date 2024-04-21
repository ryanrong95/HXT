using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Linq.Extends;
using Yahv.Underly;
using YaHv.Csrm.Services.Extends;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApp.Srm.Admins
{

    //供应商的采购人员
    public partial class Selected_Admins : BasePage
    {
        protected string SupplierID
        {
            get
            {
                return Request.QueryString["id"];
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model = new EnterprisesRoll()[SupplierID];
            }
        }
        protected object data()
        {
            var supplierid = Request.QueryString["id"];
            Expression<Func<TradingAdmin, bool>> predicate = item => true;

            var purchasers = Erp.Current.Srm.Suppliers[supplierid].Purchasers;
            var name = Request["s_name"];
            if (!string.IsNullOrWhiteSpace(name))
            {
                purchasers = purchasers.Where(item => item.ID == name || item.RealName.Contains(name) || item.StaffID.Contains(name));
            }
            return new
            {
                rows = purchasers.OrderBy(item => item.RealName).ToArray().Select(item => new
                {
                    item.ID,
                    item.RealName,
                    item.RoleName,
                    item.StaffID,
                    item.SelCode,
                    Status = item.Status.GetDescription(),
                    item.IsDefault
                })
            };
        }

        /// <summary>
        /// 绑定管理员
        /// </summary>
        protected void Binding()
        {
            var adminid = Request["adminid"];
            var supplierid = Request["supplierid"];
            Erp.Current.Srm.Suppliers[supplierid].AdminBinding(Erp.Current.ID, adminid);
            if (!string.IsNullOrEmpty(adminid) && !string.IsNullOrEmpty(supplierid))
            {
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                         nameof(Yahv.Systematic.Srm),
                                        "SupplierAdminBinding", "添加供应商" + supplierid + "的采购人" + adminid, "");
            }

        }
        /// <summary>
        /// 解除和管理员的绑定关系
        /// </summary>
        protected void Unbind()
        {
            var adminids = Request["adminids"];
            var supplierid = Request["supplierid"];
            Erp.Current.Srm.Suppliers[supplierid].AdminUnbind(adminids.Split(','));
            if (!string.IsNullOrEmpty(adminids) && !string.IsNullOrEmpty(supplierid))
            {
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                         nameof(Yahv.Systematic.Srm),
                                        "SupplierAdminUnbind", "删除供应商的" + supplierid + "采购人：" + adminids, "");
            }
        }
        /// <summary>
        /// 设为默认采购人
        /// </summary>
        protected void SetDefault()
        {
            var adminid = Request["adminid"];
            var supplierid = Request["supplierid"];
            Erp.Current.Srm.Suppliers[supplierid].SetDefault(adminid);
            if (string.IsNullOrEmpty(adminid) && !string.IsNullOrEmpty(supplierid))
            {
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                         nameof(Yahv.Systematic.Srm),
                                        " SupplierSetDefault", "供应商" + supplierid + "，默认采购员" + adminid, "");
            }
        }
    }
}