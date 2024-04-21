using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Services;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;

namespace Yahv.Payments
{
    /// <summary>
    /// 模块
    /// </summary>
    public class Accountors
    {
        private PayInfo payInfo;

        internal Accountors(PayInfo payInfo)
        {
            this.payInfo = payInfo;
        }

        /// <summary>
        /// 数字账号
        /// </summary>
        /// <remarks>
        /// 银行账款
        /// </remarks>
        //public DigitalAccount Digital
        //{
        //    get
        //    {
        //        return new DigitalAccount(this, payInfo);
        //    }
        //}

        /// <summary>
        /// 信用
        /// </summary>
        public CreditCatalogs Credit
        {
            get
            {
                return new CreditCatalogs(payInfo);
            }
        }

        /// <summary>
        /// 账期条款
        /// </summary>
        public DebtTerms DebtTerm
        {
            get
            {
                return new DebtTerms(payInfo);
            }
        }

        /// <summary>
        /// 应收账款
        /// </summary>
        public Receivables Receivable
        {
            get
            {
                return new Receivables(payInfo);
            }
        }

        /// <summary>
        /// 应付账款
        /// </summary>
        public Payables Payable
        {
            get
            {
                return new Payables(payInfo);
            }
        }
    }

    /// <summary>
    /// 业务管理
    /// </summary>
    public class ConductManager
    {
        private PayInfo payInfo;

        internal ConductManager(PayInfo payInfo)
        {
            this.payInfo = payInfo;
        }

        public Accountors this[string business]
        {
            get
            {
                //payInfo.Conduct = business;
                payInfo.Conduct = ConductConsts.供应链;
                return new Accountors(payInfo);
            }
        }

        /// <summary>
        /// 数字账号
        /// </summary>
        /// <remarks>
        /// 银行账款
        /// </remarks>
        public DigitalAccount Digital
        {
            get
            {
                return new DigitalAccount(payInfo);
            }
        }
    }

    /// <summary>
    /// 财务模块
    /// </summary>
    public class PaymentManager
    {
        PayInfo payInfo;
        PaymentManager()
        {
        }

        /// <summary>
        /// 匿名付款人
        /// </summary>
        /// <param name="payee">收款人</param>
        /// <param name="anonym">匿名</param>
        /// <returns>业务管理</returns>
        public ConductManager AnonymPayer(string anonym, string payee)
        {
            using (PvbCrmReponsitory reponsitory = new PvbCrmReponsitory())
            {
                var dpayee = reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Enterprises>()
                     .SingleOrDefault(item => item.Name == payee || item.ID == payee);

                if (dpayee == null)
                {
                    throw new Exception("存在为空的情况！");
                }

                this.payInfo.Payee = dpayee.ID;
                this.payInfo.PayerAnonymous = anonym;
                this.payInfo.Payer = AnonymousEnterprise.Current.ID;      //匿名ID

                return new ConductManager(payInfo);
            }
        }

        /// <summary>
        /// 匿名收款人
        /// </summary>
        /// <param name="payer">付款人</param>
        /// <param name="anonym">匿名</param>
        /// <returns>业务管理</returns>
        public ConductManager AnonymPayee(string anonym, string payer)
        {
            using (PvbCrmReponsitory reponsitory = new PvbCrmReponsitory())
            {
                var dpayee = reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Enterprises>()
                     .SingleOrDefault(item => item.Name == payer || item.ID == payer);

                if (dpayee == null)
                {
                    throw new Exception("存在为空的情况！");
                }

                this.payInfo.Payer = dpayee.ID;
                this.payInfo.PayeeAnonymous = anonym;
                this.payInfo.Payee = AnonymousEnterprise.Current.ID;      //匿名ID

                return new ConductManager(payInfo);
            }
        }

        public ConductManager this[string payer, string payee]
        {
            get
            {
                string dpayer = GetEpID(payer);
                string dpayee = GetEpID(payee);

                if (string.IsNullOrWhiteSpace(dpayer))
                {
                    throw new Exception($"未找到付款人[{payer}]！");
                }

                if (string.IsNullOrWhiteSpace(dpayee))
                {
                    throw new Exception($"未找到收款人[{payee}]！");
                }

                this.payInfo.Payer = dpayer;
                this.payInfo.Payee = dpayee;

                return new ConductManager(payInfo);
            }
        }

        //public ConductManager this[string payer, string payee]
        //{
        //    get
        //    {
        //        using (PvbCrmReponsitory reponsitory = new PvbCrmReponsitory())
        //        {
        //            var dpayer = reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Enterprises>()
        //                .SingleOrDefault(item => item.Name == payer || item.ID == payer);

        //            var dpayee = reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Enterprises>()
        //                .SingleOrDefault(item => item.Name == payee || item.ID == payee);

        //            if (dpayer == null)
        //            {
        //                throw new Exception($"未找到付款人[{payer}]！");
        //            }

        //            if (dpayee == null)
        //            {
        //                throw new Exception($"未找到收款人[{payee}]！");
        //            }

        //            this.payInfo.Payer = dpayer.ID;
        //            this.payInfo.Payee = dpayee.ID;

        //            return new ConductManager(payInfo);
        //        }
        //    }
        //}

        //static PaymentManager current;
        //static object tlocker = new object();
        //static PaymentManager Current
        //{
        //    get
        //    {
        //        if (current == null)
        //        {
        //            lock (tlocker)
        //            {
        //                if (current == null)
        //                {
        //                    current = new PaymentManager();
        //                }
        //            }
        //        }

        //        return current;
        //    }
        //}

        private static PaymentManager npc;
        static object locker = new object();
        /// <summary>
        /// 获取Npc机器人调用
        /// </summary>
        static public PaymentManager Npc
        {
            get
            {
                if (npc == null)
                {
                    lock (locker)
                    {
                        if (npc == null)
                        {
                            npc = new PaymentManager();
                            npc.payInfo = new PayInfo()
                            {
                                Inputer = new Inputer(Underly.Npc.Robot.Obtain(), InputerType.Erp)
                            };
                        }
                    }
                }

                return npc;
            }
        }

        /// <summary>
        /// 获取Erp用户调用
        /// </summary>
        /// <param name="adminID">Erp用户ID</param>
        /// <returns>Erp用户调用</returns>
        static public PaymentManager Erp(string adminID)
        {
            return new PaymentManager
            {
                payInfo = new PayInfo()
                {
                    Inputer = new Inputer(adminID, InputerType.Erp)
                }
            };
        }

        /// <summary>
        /// 获取网站用户调用
        /// </summary>
        /// <param name="siteUserID">网站用户ID</param>
        /// <returns>网站用户调用</returns>
        static public PaymentManager Site(string siteUserID)
        {
            return new PaymentManager
            {
                payInfo = new PayInfo()
                {
                    Inputer = new Inputer(siteUserID, InputerType.Erp)
                }
            };
        }

        /// <summary>
        /// 实收账款
        /// </summary>
        public Receiveds Received
        {
            get
            {
                return new Receiveds(payInfo.Inputer);
            }
        }

        /// <summary>
        /// 实付账款
        /// </summary>
        public Payments Payment
        {
            get
            {
                return new Payments(payInfo);
            }
        }

        /// <summary>
        /// 信用记账
        /// </summary>
        public CreditCatalogs Credit
        {
            get
            {
                return new CreditCatalogs(new PayInfo() { Inputer = payInfo.Inputer });
            }
        }

        #region 私有函数
        /// <summary>
        /// 获取企业ID
        /// </summary>
        /// <remarks>避免Crm存在重名带空格的客户</remarks>
        /// <param name="val"></param>
        /// <returns></returns>
        private string GetEpID(string val)
        {
            using (PvbCrmReponsitory reponsitory = new PvbCrmReponsitory())            
            {
                var array = reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Enterprises>()
                    .Where(item => item.Name == val || item.ID == val).ToArray();

                if (array.Length == 1)
                {
                    return array[0].ID;
                }

                if (array.Length > 1)
                {
                    string valMd5 = val.MD5();      //根据名称MD5查询

                    array = reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Enterprises>()
                        .Where(item => item.ID == valMd5).ToArray();

                    if (array.Length == 1)
                    {
                        return array[0].ID;
                    }
                }

                return string.Empty;
            }
        }
        #endregion
    }
}
