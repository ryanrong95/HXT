using System;
using System.Linq;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Finance.Services.Enums;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Usually;

namespace Yahv.Finance.Services.Models.Origins
{
    /// <summary>
    /// 工资申请
    /// </summary>
    public class SalaryApply : IUnique
    {
        #region 事件

        public event SuccessHanlder Success;
        #endregion

        #region 属性
        public string ID { get; set; }

        /// <summary>
        /// 工资月
        /// </summary>
        public int DateIndex { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 回调地址
        /// </summary>
        public string CallBackUrl { get; set; }

        /// <summary>
        /// 回调ID
        /// </summary>
        public string CallBackID { get; set; }

        /// <summary>
        /// 系统来源ID
        /// </summary>
        public string SenderID { get; set; }

        /// <summary>
        /// 申请人部门
        /// </summary>
        public string Department { get; set; }

        /// <summary>
        /// 创建人ID
        /// </summary>
        public string CreatorID { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public ApplyStauts Status { get; set; }
        #endregion

        #region 扩展属性
        /// <summary>
        /// 创建人名称
        /// </summary>
        public string CreatorName { get; set; }
        #endregion

        #region 持久化

        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                //新增
                if (!reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.SalaryApplies>().Any(item => item.ID == this.ID))
                {
                    this.ID = this.ID ?? PKeySigner.Pick(PKeyType.SalaryApplies);

                    reponsitory.Insert(new Layers.Data.Sqls.PvFinance.SalaryApplies()
                    {
                        Currency = (int)this.Currency,
                        ID = this.ID,
                        CreateDate = DateTime.Now,
                        CreatorID = this.CreatorID,
                        Status = (int)this.Status,
                        Price = this.Price,
                        Summary = this.Summary,
                        CallBackID = this.CallBackID,
                        CallBackUrl = this.CallBackUrl,
                        Department = this.Department,
                        SenderID = this.SenderID,
                        Title = this.Title,
                    });

                    this.Success?.Invoke(this, new SuccessEventArgs(this));
                }
            }
        }

        #endregion
    }
}