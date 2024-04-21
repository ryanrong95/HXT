using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YaHv.Csrm.Services.Models.Origins
{
    public class AdminAppend : Admin
    {
        #region 其他
        Views.Rolls.AdminSuppliersRoll suppliers;
        /// <summary>
        /// 某管理员的供应商
        /// </summary>
        public Views.Rolls.AdminSuppliersRoll Suppliers
        {
            get
            {
                if (this.suppliers == null || this.suppliers.Disposed)
                {
                    this.suppliers = new Views.Rolls.AdminSuppliersRoll(this);
                }
                return this.suppliers;
            }
        }
        Views.Rolls.AdminClientsRoll clients;
        /// <summary>
        /// 某管理员的的客户
        /// </summary>
        public Views.Rolls.AdminClientsRoll Clients
        {
            get
            {
                if (this.clients == null || this.clients.Disposed)
                {
                    this.clients = new Views.Rolls.AdminClientsRoll(this);
                }
                return this.clients;
            }
        }
        /// <summary>
        /// 某管理员的受益人（包含不同供应商的受益人）
        /// </summary>
        Views.Rolls.AdminBeneficiaries beneficiaries;
        public Views.Rolls.AdminBeneficiaries Beneficiaries
        {
            get
            {
                if (this.beneficiaries == null || this.beneficiaries.Disposed)
                {
                    this.beneficiaries = new Views.Rolls.AdminBeneficiaries(this);
                }
                return this.beneficiaries;
            }
        }
        /// <summary>
        /// 某管理员的发票
        /// </summary>
        //Views.Rolls.AdminInvoices invoices;
        //public Views.Rolls.AdminInvoices Invoices
        //{
        //    get
        //    {
        //        if (this.invoices == null || this.invoices.Disposed)
        //        {
        //            this.invoices = new Views.Rolls.AdminInvoices(this);
        //        }
        //        return this.invoices;
        //    }
        //}
        /// <summary>
        /// /某管理员的联系人
        /// </summary>
        Views.Rolls.AdminContactsRoll contacts;
        public Views.Rolls.AdminContactsRoll Contacts
        {
            get
            {
                if (this.contacts == null || this.contacts.Disposed)
                {
                    this.contacts = new Views.Rolls.AdminContactsRoll(this, Yahv.Underly.Business.Trading);
                }
                return this.contacts;
            }
        }
        /// <summary>
        /// 某管理员的到货地址
        /// </summary>
        Views.Rolls.AdminConsigneesRoll consignees;
        public Views.Rolls.AdminConsigneesRoll Consignees
        {
            get
            {
                if (this.consignees == null || this.consignees.Disposed)
                {
                    this.consignees = new Views.Rolls.AdminConsigneesRoll(this, Yahv.Underly.Business.Trading);
                }
                return this.consignees;
            }
        }
        #endregion

    }
}
