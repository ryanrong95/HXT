using System;
using System.Linq;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Finance.Services.Models.Origins
{
    /// <summary>
    /// 工资申请项
    /// </summary>
    public class SalaryApplyItem : IUnique
    {
        #region 属性
        public string ID { get; set; }

        /// <summary>
        /// 申请ID
        /// </summary>
        public string ApplyID { get; set; }

        /// <summary>
        /// 收款账户ID
        /// </summary>
        public string PayeeAccountID { get; set; }

        /// <summary>
        /// 付款账户ID
        /// </summary>
        public string PayerAccountID { get; set; }

        /// <summary>
        /// 付款分类
        /// </summary>
        public string AccountCatalogID { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal? Price { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 流水ID
        /// </summary>
        public string FlowID { get; set; }

        /// <summary>
        /// 回调地址
        /// </summary>
        public string CallBackUrl { get; set; }

        /// <summary>
        /// 回调参数
        /// </summary>
        public string CallBackID { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime ModifyDate { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public ApplyItemStauts Status { get; set; }
        #endregion

        #region 拓展属性
        /// <summary>
        /// 流水号
        /// </summary>
        public string FormCode { get; set; }

        /// <summary>
        /// 付款日期
        /// </summary>
        public DateTime? PaymentDate { get; set; }

        /// <summary>
        /// 付款账号
        /// </summary>
        public string PayerCode { get; set; }

        /// <summary>
        /// 收款账号
        /// </summary>
        public string PayeeCode { get; set; }

        /// <summary>
        /// 收款人-姓名
        /// </summary>
        public string PayeeName { get; set; }

        /// <summary>
        /// 收款人-身份证号
        /// </summary>
        public string PayeeIDCard { get; set; }

        /// <summary>
        /// 收款人-所属公司
        /// </summary>
        public string PayeeCompany { get; set; }
        #endregion

        #region 持久化

        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                //新增
                if (!reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.SalaryApplyItems>().Any(item => item.ID == this.ID))
                {
                    this.ID = this.ID ?? PKeySigner.Pick(PKeyType.SalaryApplyItems);

                    reponsitory.Insert(new Layers.Data.Sqls.PvFinance.SalaryApplyItems()
                    {
                        ID = this.ID,
                        CreateDate = DateTime.Now,
                        Status = (int)this.Status,
                        Price = this.Price,
                        Summary = this.Summary,
                        PayeeAccountID = this.PayeeAccountID,
                        ApplyID = this.ApplyID,
                        ModifyDate = DateTime.Now,
                        PayerAccountID = this.PayerAccountID,
                        AccountCatalogID = this.AccountCatalogID,
                        CallBackID = this.CallBackID,
                        CallBackUrl = this.CallBackUrl,
                        FlowID = this.FlowID,
                    });
                }
                //修改
                else
                {
                    reponsitory.Update<Layers.Data.Sqls.PvFinance.SalaryApplyItems>(new
                    {
                        Status = (int)this.Status,
                        Price = this.Price,
                        Summary = this.Summary,
                        PayeeAccountID = this.PayeeAccountID,
                        ApplyID = this.ApplyID,
                        ModifyDate = DateTime.Now,
                        PayerAccountID = this.PayerAccountID,
                        AccountCatalogID = this.AccountCatalogID,
                        CallBackID = this.CallBackID,
                        CallBackUrl = this.CallBackUrl,
                        FlowID = this.FlowID,
                    }, item => item.ID == this.ID);
                }
            }
        }
        #endregion
    }
}