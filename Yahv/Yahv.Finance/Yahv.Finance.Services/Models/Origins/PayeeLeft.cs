using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Usually;

namespace Yahv.Finance.Services.Models.Origins
{
    public class PayeeLeft : IUnique
    {
        #region 事件

        /// <summary>
        /// AddSuccess
        /// </summary>
        public event SuccessHanlder AddSuccess;

        /// <summary>
        /// Update
        /// </summary>
        public event SuccessHanlder UpdateSuccess;

        /// <summary>
        /// EnterError
        /// </summary>
        public event ErrorHanlder EnterError;

        /// <summary>
        /// 生成待认领数据
        /// </summary>
        public event SuccessHanlder AddAccountWork;
        #endregion

        #region 数据库属性

        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 账款分类
        /// </summary>
        public string AccountCatalogID { get; set; }

        /// <summary>
        /// 我方收款账户ID
        /// </summary>
        public string AccountID { get; set; }

        /// <summary>
        /// 付款人名称
        /// </summary>
        public string PayerName { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 本位币种1
        /// </summary>
        public Currency Currency1 { get; set; }

        /// <summary>
        /// 本位币种汇率1
        /// </summary>
        public decimal ERate1 { get; set; }

        /// <summary>
        /// 本位金额1
        /// </summary>
        public decimal Price1 { get; set; }

        /// <summary>
        /// 录入人
        /// </summary>
        public string CreatorID { get; set; }

        /// <summary>
        /// 录入时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 流水ID
        /// </summary>
        public string FlowID { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public GeneralStatus Status { get; set; }

        /// <summary>
        /// 摘要
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 付款公司性质（公司 or 个人）
        /// </summary>
        public NatureType PayerNature { get; set; }
        #endregion

        #region 其它属性

        /// <summary>
        /// 账款分类名称
        /// </summary>
        public string AccountCatalogName { get; set; }

        /// <summary>
        /// 流水号
        /// </summary>
        public string FormCode { get; set; }

        /// <summary>
        /// 银行名称
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        public string AccountCode { get; set; }

        /// <summary>
        /// 创建人名称
        /// </summary>
        public string CreatorName { get; set; }

        /// <summary>
        /// 收款日期
        /// </summary>
        public DateTime? ReceiptDate { get; set; }

        /// <summary>
        /// 收款名称
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// 可用余额
        /// </summary>
        public decimal Balance { get; set; }
        #endregion

        #region 持久化

        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                //添加
                if (!reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.PayeeLefts>().Any(item => item.ID == this.ID))
                {
                    this.ID = this.ID ?? PKeySigner.Pick(PKeyType.PayeeLeft);

                    reponsitory.Insert(new Layers.Data.Sqls.PvFinance.PayeeLefts()
                    {
                        ID = this.ID,
                        AccountCatalogID = this.AccountCatalogID,
                        AccountID = this.AccountID,
                        PayerName = this.PayerName,
                        Currency = (int)this.Currency,
                        Price = this.Price,
                        Currency1 = (int)this.Currency1,
                        ERate1 = this.ERate1,
                        Price1 = this.Price1,
                        CreatorID = this.CreatorID,
                        CreateDate = DateTime.Now,
                        FlowID = this.FlowID,
                        Status = (int)GeneralStatus.Normal,
                        Summary = this.Summary,
                        PayerNature = (int)this.PayerNature,
                    });

                    if (ConfigurationManager.AppSettings["Companies"] != null && ConfigurationManager.AppSettings["Companies"].Contains($",{this.AccountName},"))
                    {
                        this.AddAccountWork?.Invoke(this, new SuccessEventArgs(this));
                    }

                    this.AddSuccess?.Invoke(this, new SuccessEventArgs(this));
                }
                //修改
                else
                {
                    reponsitory.Update<Layers.Data.Sqls.PvFinance.PayeeLefts>(new
                    {
                        AccountCatalogID = this.AccountCatalogID,
                        AccountID = this.AccountID,
                        PayerName = this.PayerName,
                        Currency = (int)this.Currency,
                        Price = this.Price,
                        Currency1 = (int)this.Currency1,
                        ERate1 = this.ERate1,
                        Price1 = this.Price1,
                        FlowID = this.FlowID,
                        PayerNature = (int)this.PayerNature,
                    }, item => item.ID == this.ID);

                    this.UpdateSuccess?.Invoke(this, new SuccessEventArgs(this));
                }
            }
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <remarks>主要是为了使用事务的时候用</remarks>
        public void Add()
        {
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                this.ID = this.ID ?? PKeySigner.Pick(PKeyType.PayeeLeft);
                reponsitory.Insert(new Layers.Data.Sqls.PvFinance.PayeeLefts()
                {
                    ID = this.ID,
                    AccountCatalogID = this.AccountCatalogID,
                    AccountID = this.AccountID,
                    PayerName = this.PayerName,
                    Currency = (int)this.Currency,
                    Price = this.Price,
                    Currency1 = (int)this.Currency1,
                    ERate1 = this.ERate1,
                    Price1 = this.Price1,
                    CreatorID = this.CreatorID,
                    CreateDate = DateTime.Now,
                    FlowID = this.FlowID,
                    Status = (int)GeneralStatus.Normal,
                    Summary = this.Summary,
                    PayerNature = (int)this.PayerNature,
                });

                if (ConfigurationManager.AppSettings["Companies"] != null && ConfigurationManager.AppSettings["Companies"].Contains($",{this.AccountName},"))
                {
                    this.AddAccountWork?.Invoke(this, new SuccessEventArgs(this));
                }

                this.AddSuccess?.Invoke(this, new SuccessEventArgs(this));
            }
        }
        #endregion

    }
}
