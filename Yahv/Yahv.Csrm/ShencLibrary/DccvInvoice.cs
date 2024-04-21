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
    /// 个人客户发票
    /// </summary>
    public class SyncvInvoice
    {
        #region 属性
        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { set; get; }
        /// <summary>
        /// 企业ID
        /// </summary>
        public string EnterpriseID { get; set; }
        /// <summary>
        /// 是否个人发票，否是企业发票
        /// </summary>
        public bool IsPersonal { set; get; }

        /// <summary>
        /// 发票类型 1 普通发票 2 增值税发票 3 海关发票
        /// </summary>
        public InvoiceType Type { get; set; }
        /// <summary>
        /// 发票抬头
        /// </summary>
        public string Title { set; get; }
        /// <summary>
        /// 纳税人识别号
        /// </summary>
        public string TaxNumber { set; get; }
        /// <summary>
        /// 企业注册地址
        /// </summary>
        public string RegAddress { set; get; }
        /// <summary>
        /// 企业电话
        /// </summary>
        public string Tel { set; get; }
        /// <summary>
        /// 开户银行
        /// </summary>
        public string BankName { get; set; }
        /// <summary>
        /// 银行账号
        /// </summary>
        public string BankAccount { set; get; }
        /// <summary>
        /// 收票地址
        /// </summary>
        public string PostAddress { get; set; }
        /// <summary>
        /// 收票人
        /// </summary>
        public string PostRecipient { set; get; }
        /// <summary>
        /// 收票人联系电话
        /// </summary>
        public string PostTel { set; get; }

        /// <summary>
        /// 邮编
        /// </summary>
        public string PostZipCode { set; get; }

        /// <summary>
        /// 交付方式
        /// </summary>
        public InvoiceDeliveryType DeliveryType { set; get; }

        public string CreatorID { set; get; }
        /// <summary>
        /// 是否默认
        /// </summary>
        public bool IsDefault { set; get; }

        #endregion

    }
    public class DccvInvoice
    {
        public DccvInvoice()
        {

        }
        bool EnterSuccess = false;
        public bool Enter(SyncvInvoice entity, string userid = null)
        {
            using (var roll = new WsClientsRoll())
            {
                var client = roll[entity.EnterpriseID];
                if (client.Enterprise.Name.StartsWith("reg-", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
                vInvoice data;
                data = new vInvoice
                {
                    ID = entity.ID,
                    EnterpriseID = entity.EnterpriseID,
                    IsPersonal = entity.IsPersonal,
                    Type = entity.Type,
                    Title = entity.Title,
                    TaxNumber = entity.TaxNumber,
                    RegAddress = entity.RegAddress,
                    Tel = entity.Tel,
                    BankName = entity.BankName,
                    BankAccount = entity.BankAccount,
                    PostAddress = entity.PostAddress,
                    PostRecipient = entity.PostRecipient,
                    PostTel = entity.PostTel,
                    PostZipCode = entity.PostZipCode,
                    DeliveryType = entity.DeliveryType,
                    IsDefault = entity.IsDefault,
                    CreatorID = entity.CreatorID
                };
                data.EnterSuccess += Data_EnterSuccess;
                data.Enter();
                return EnterSuccess;
            }
        }

        private void Data_EnterSuccess(object sender, Yahv.Usually.SuccessEventArgs e)
        {
            EnterSuccess = true;
        }

        bool AbandonSuccess = false;
        public bool Abandon(string id)
        {
            var entity = new vInvoicesRoll()[id];
            entity.AbandonSuccess += Entity_AbandonSuccess;
            entity.Abandon();
            return AbandonSuccess;
        }

        private void Entity_AbandonSuccess(object sender, Yahv.Usually.SuccessEventArgs e)
        {
            AbandonSuccess = true;
        }
    }
}
