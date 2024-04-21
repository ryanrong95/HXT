using System;
using System.Linq;
using Layers.Data;
using Layers.Linq;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Usually;
using Layers.Data.Sqls;

namespace Yahv.Finance.Services.Models.Origins
{
    /// <summary>
    /// 费用申请项
    /// </summary>
    public class ChargeApplyItem : IUnique
    {
        #region 事件

        /// <summary>
        /// EnterSuccess
        /// </summary>
        public event SuccessHanlder EnterSuccess;

        /// <summary>
        /// EnterError
        /// </summary>
        public event ErrorHanlder EnterError;

        #endregion

        #region 数据库属性

        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 申请ID
        /// </summary>
        public string ApplyID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsPaid { get; set; }

        /// <summary>
        /// 期望付款时间
        /// </summary>
        public DateTime? ExpectedTime { get; set; }

        /// <summary>
        /// 费用分类
        /// </summary>
        public string AccountCatalogID { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal? Price { get; set; }

        /// <summary>
        /// 备注
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
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifyDate { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public ApplyItemStauts Status { get; set; }

        #endregion

        #region 其它属性

        /// <summary>
        /// 费用分类名称
        /// </summary>
        public string AccountCatalogName { get; set; }

        #endregion

        #region 持久化

        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                //添加
                if (!reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.ChargeApplyItems>().Any(item => item.ID == this.ID))
                {
                    this.ID = this.ID ?? PKeySigner.Pick(PKeyType.ChargeApplyItem);
                    reponsitory.Insert(new Layers.Data.Sqls.PvFinance.ChargeApplyItems()
                    {
                        ID = this.ID,
                        ApplyID = this.ApplyID,
                        IsPaid = this.IsPaid,
                        ExpectedTime = this.ExpectedTime,
                        AccountCatalogID = this.AccountCatalogID,
                        Price = this.Price,
                        Summary = this.Summary,
                        FlowID = this.FlowID,
                        CallBackUrl = this.CallBackUrl,
                        CallBackID = this.CallBackID,
                        CreateDate = DateTime.Now,
                        ModifyDate = DateTime.Now,
                        Status = (int)GeneralStatus.Normal,
                    });
                }
                //修改
                else
                {
                    reponsitory.Update<Layers.Data.Sqls.PvFinance.ChargeApplyItems>(new
                    {
                        ApplyID = this.ApplyID,
                        IsPaid = this.IsPaid,
                        ExpectedTime = this.ExpectedTime,
                        AccountCatalogID = this.AccountCatalogID,
                        Price = this.Price,
                        Summary = this.Summary,
                        FlowID = this.FlowID,
                        CallBackUrl = this.CallBackUrl,
                        CallBackID = this.CallBackID,
                        ModifyDate = DateTime.Now,
                    }, item => item.ID == this.ID);
                }

                this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
            }
        }

        #endregion

    }
}