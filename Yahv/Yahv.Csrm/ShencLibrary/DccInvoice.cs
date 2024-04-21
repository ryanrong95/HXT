using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace ShencLibrary
{
    /// <summary>
    /// 客户发票
    /// </summary>
    public class SyncInvoice
    {
        /// <summary>
        /// 联系人姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 联系人电话
        /// </summary>
        public string Tel { get; set; }
        /// <summary>
        /// 联系人手机号
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 联系人邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 发票类型
        /// </summary>
        public InvoiceType Type { set; get; }
        /// <summary>
        /// 企业电话
        /// </summary>
        public string CompanyTel { set; get; }
        /// <summary>
        /// 开户行
        /// </summary>
        public string Bank { set; get; }
        /// <summary>
        /// 开户行地址
        /// </summary>
        public string BankAddress { set; get; }

        /// <summary>
        /// 客户注册地址,实际是InvoiceAddress
        /// </summary>
        public string RegAddress { get; set; }
        /// <summary>
        /// 账号
        /// </summary>
        public string Account { set; get; }
        /// <summary>
        /// 纳税人识别号
        /// </summary>
        public string TaxperNumber { set; get; }
        /// <summary>
        /// 邮寄类型
        /// </summary>
        public InvoiceDeliveryType DeliveryType { set; get; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { set; get; }
        /// <summary>
        /// 邮编
        /// </summary>
        public string Postzip { set; get; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { set; get; }

    }
    public class DccInvoice
    {
        public DccInvoice()
        {

        }
        public void Enter(string clientid, SyncInvoice entity, string userid = null)
        {
            using (var roll = new WsClientsRoll())
            {
                var client = roll[clientid];
                if (client.Enterprise.Name.StartsWith("reg-", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
                //if (!string.IsNullOrWhiteSpace(entity.RegAddress))
                //{
                //    client.Enterprise.RegAddress = entity.RegAddress;
                //    client.Enterprise.Enter();
                //}

                WsInvoice data;
                (data = new WsInvoice
                {
                    Enterprise = client.Enterprise,
                    CompanyTel = entity.CompanyTel,
                    Bank = string.IsNullOrWhiteSpace(entity.Bank) ? "" : entity.Bank,
                    Account = entity.Account,
                    Type = entity.Type,
                    TaxperNumber = entity.TaxperNumber,
                    Address = entity.Address,
                    BankAddress = entity.BankAddress,
                    Postzip = entity.Postzip,
                    Name = entity.Name,
                    Tel = entity.Tel,
                    Mobile = entity.Mobile,
                    Email = entity.Email,
                    DeliveryType = entity.DeliveryType,
                    EnterpriseID = client.Enterprise.ID,
                    InvoiceAddress = entity.RegAddress,
                    IsDefault = true,
                    CreatorID = ""
                }).Enter();

                this.Synchro(data, userid);//向xdt同步
            }
        }
        public void Synchro(WsInvoice entity, string userid)
        {
            string url = Commons.UnifyApiUrl + "/clients/invoice";
            Commons.HttpPostRaw(url, new
            {
                UserID = userid,
                Enterprise = entity.Enterprise,
                Bank = entity.Bank,
                CompanyTel = entity.CompanyTel,
                BankAddress = entity.BankAddress,
                TaxperNumber = entity.TaxperNumber,
                Account = entity.Account,
                DeliveryType = entity.DeliveryType,
                ConsigneeAddress = entity.Address,
                InvoiceAddress = entity.InvoiceAddress,
                Name = entity.Name,
                Tel = entity.Tel,
                Email = entity.Email,
                Mobile = entity.Mobile,
                Postzip = entity.Postzip,
                Summary = ""
            }.Json());
        }
    }
}
