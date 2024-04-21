using System;

using Yahv.Underly;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace ShencLibrary
{
    public class SynPayer
    {
        #region 属性
        /// <summary>
        /// 唯一标识
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 企业ID
        /// </summary>
        public string EnterpriseID { get; set; }
        /// <summary>
        /// 企业名称
        /// </summary>
        public string EnterpriseName { set; get; }
        /// <summary>
        /// 真实企业ID
        /// </summary>
        public string RealID { set; get; }
        /// <summary>
        /// 真实企业名称
        /// </summary>
        public string RealName { set; get; }
        /// <summary>
        /// 开户银行
        /// </summary>
        public string Bank { get; set; }
        /// <summary>
        /// 开户行地址
        /// </summary>
        public string BankAddress { get; set; }
        /// <summary>
        /// 银行账户
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 银行编码 (国际)
        /// </summary>
        public string SwiftCode { get; set; }
        /// <summary>
        /// 汇款方式
        /// </summary>
        public Methord Methord { get; set; }
        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { get; set; }
        /// <summary>
        /// 联系人姓名
        /// </summary>
        public string Contact { set; get; }
        /// <summary>
        /// 联系电话
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
        /// 状态
        /// </summary>
        public GeneralStatus Status { get; set; }

        /// <summary>
        /// 国家或地区
        /// </summary>
        public string Place { set; get; }
        /// <summary>
        /// 录入人ID
        /// </summary>
        public string CreatorID { set; get; }

        #endregion

    }
    public class DccPayer
    {
        public DccPayer()
        {
            //dynamic entity = new System.Dynamic.ExpandoObject();
            //var kkk = (SyncBeneficiary)entity;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientid">客户ID</param>
        /// <param name="nsupplierid">供应商ID</param>
        /// <param name="entity"></param>
        /// <param name="npayeeid">收款人ID：修改时要赋值</param>
        /// <returns></returns>

        public string Enter(string clientid, SynPayer entity, string npayerid = null)
        {
            using (var roll = new WsClientsRoll())
            {
                var client = roll[clientid];
                if (client.Enterprise.Name.StartsWith("reg-", StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }
                nPayer data;
                (data = new nPayer
                {
                    ID = npayerid,
                    EnterpriseID = clientid,
                    RealID = entity.RealID,
                    RealEnterprise = new Enterprise
                    {
                        ID = entity.RealID,
                        Name = entity.RealName
                    },
                    Bank = entity.Bank,
                    BankAddress = entity.BankAddress,
                    Account = entity.Account,
                    SwiftCode = entity.SwiftCode,
                    Methord = entity.Methord,
                    Currency = entity.Currency,
                    Contact = entity.Contact ?? "",
                    Tel = entity.Tel,
                    Mobile = entity.Mobile,
                    Email = entity.Email,
                    Creator = entity.CreatorID

                }).Enter();

                return data.ID;
            }
        }

        public void Test()
        {
            new DccPayer().Enter(new MyClientPayer
            {

            });
        }


        /// <summary>
        /// 保存客户付款人
        /// </summary>
        /// <param name="clientid">客户ID，理论上应该开发在构造其中</param>
        /// <param name="entity">付款人</param>
        /// <returns>付款人ID</returns>
        public string Enter(MyClientPayer entity)
        {
            using (var roll = new WsClientsRoll())
            {
                var client = roll[entity.ClientID];
                if (client.Enterprise.Name.StartsWith("reg-", StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }
            }

            var payer = new Yahv.Services.Models.wsPayer
            {
                EnterpriseID = entity.ClientID,
                RealEnterpriseName = entity.RealName,
                Methord = entity.Methord,
                Currency = entity.Currency,
                Place = entity.Place,
                Contact = entity.Contact,
                Status = GeneralStatus.Normal
            };

            payer.Enter();
            return payer.ID;
        }

        public void Abandon(string npayerid)
        {
            new Yahv.Services.Models.wsPayer
            {
                ID = npayerid
            }.Abandon();
        }


        /// <summary>
        /// 李鹏前端专用
        /// </summary>
        public class MyClientPayer
        {

            public string ClientID { get; set; }

            public string RealName { set; get; }

            public Methord Methord { get; set; }
            /// <summary>
            /// 币种
            /// </summary>
            public Currency Currency { get; set; }

            /// <summary>
            /// 国家或地区
            /// </summary>
            public string Place { set; get; }

            /// <summary>
            /// 联系人姓名
            /// </summary>
            public string Contact { set; get; }
        }
    }

}
