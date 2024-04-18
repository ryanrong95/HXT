using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.PvWsOrder.Services.Extends;
using Yahv.Services.Models;
using Yahv.Services.Views;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;

namespace Yahv.PvWsOrder.Services.ClientViews
{
    public class MyInvoicesView : WsInvoicesTopView<PvbCrmReponsitory>
    {
        private string enterpriseid;

        /// <summary>
        /// 无参构造函数
        /// </summary>
        private MyInvoicesView()
        {

        }

        /// <summary>
        /// 当前客户的开票信息
        /// </summary>
        /// <param name="EnterpriseID">客户ID</param>
        public MyInvoicesView(string EnterpriseID)
        {
            this.enterpriseid = EnterpriseID;
        }

        /// <summary>
        /// 查询结果集
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Invoice> GetIQueryable()
        {
            return base.GetIQueryable().Where(item => item.EnterpriseID == this.enterpriseid);
        }

        /// <summary>
        /// 数据持久化
        /// </summary>
        public void Enter(Invoice invoice)
        {
            var id = invoice.ID ?? string.Join("", invoice.EnterpriseID, invoice.Type, invoice.Bank, invoice.BankAddress,
                invoice.Account, invoice.TaxperNumber).MD5();

            if (this[id] == null)
            {
                this.Reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.Invoices
                {
                    ID = id,
                    EnterpriseID = invoice.EnterpriseID,
                    Type = (int)invoice.Type,
                    Bank = invoice.Bank,
                    BankAddress = invoice.BankAddress,
                    Account = invoice.Account,
                    TaxperNumber = invoice.TaxperNumber,
                    Name = invoice.Name,
                    Tel = invoice.Tel,
                    Mobile = invoice.Mobile,
                    Email = invoice.Email,
                    Address = invoice.Address,
                    Postzip = invoice.Postzip,
                    DeliveryType = (int)invoice.DeliveryType,
                    AdminID = invoice.AdminID,
                    CompanyTel = invoice.CompanyTel,
                    Status = (int)ApprovalStatus.Normal,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                });
            }
            else
            {
                this.Reponsitory.Update<Layers.Data.Sqls.PvbCrm.Invoices>(new
                {
                    Type = (int)invoice.Type,
                    Bank = invoice.Bank,
                    BankAddress = invoice.BankAddress,
                    Account = invoice.Account,
                    TaxperNumber = invoice.TaxperNumber,
                    Name = invoice.Name,
                    Tel = invoice.Tel,
                    Mobile = invoice.Mobile,
                    Email = invoice.Email,
                    Address = invoice.Address,
                    Postzip = invoice.Postzip,
                    DeliveryType = (int)invoice.DeliveryType,
                    AdminID = invoice.AdminID,
                    CompanyTel = invoice.CompanyTel,
                    UpdateDate = DateTime.Now,
                }, a => a.ID == id);
            }

            //数据插入关系表
            var maps = new Layers.Data.Sqls.PvbCrm.MapsBEnter
            {
                ID = "WsInvoice_" + invoice.EnterpriseID,
                Bussiness = (int)Business.WarehouseServicing,
                Type = (int)MapsType.Invoice,
                SubID = id,
                CreateDate = DateTime.Now,
                CtreatorID = invoice.AdminID,
                IsDefault = true,
            };
            maps.MapsBEnter(this.Reponsitory);
        }
    }
}
