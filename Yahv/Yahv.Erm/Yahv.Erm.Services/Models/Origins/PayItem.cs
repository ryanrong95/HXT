using System;
using System.Linq;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbErm;
using Layers.Linq;
using Yahv.Linq;
using Yahv.Usually;
using Yahv.Utils.Converters.Contents;

namespace Yahv.Erm.Services.Models.Origins
{
    /// <summary>
    /// 工资表
    /// </summary>
    public class PayItem : IUnique
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

        /// <summary>
        /// AbandonSuccess
        /// </summary>
        public event SuccessHanlder AbandonSuccess;

        #endregion

        #region 属性
        string id;
        /// <summary>
        /// ID
        /// </summary>
        public string ID
        {
            get
            {
                return this.id ?? string.Join("", this.PayID, this.Name).MD5();
            }
            set
            {
                this.id = value;
            }

        }

        /// <summary>
        /// 月账单ID
        /// </summary>
        public string PayID { get; set; }

        /// <summary>
        /// 工资项名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public decimal Value { get; set; }

        /// <summary>
        /// DateIndex
        /// </summary>
        public string DateIndex { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// 实际录入人
        /// </summary>
        public string AdminID { get; set; }

        /// <summary>
        /// 修改人ID
        /// </summary>
        public string UpdateAdminID { get; set; }

        /// <summary>
        /// 工资项公式
        /// </summary>
        public string WageItemFormula { get; set; }

        /// <summary>
        /// 实际计算公式
        /// </summary>
        public string ActualFormula { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public PayItemStatus Status { get; set; }


        /// <summary>
        /// 员工ID
        /// </summary>
        public string StaffID { get; set; }

        /// <summary>
        /// 是否为新增
        /// </summary>
        public bool IsInsert { get; set; }
        #endregion

        #region 持久化

        /// <summary>
        /// 添加/修改
        /// </summary>
        public void Enter()
        {
            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                //添加
                if (!repository.ReadTable<PayItems>().Any(t => t.Name == this.Name && t.PayID == this.PayID))
                {
                    repository.Insert(new PayItems()
                    {
                        //ID = PKeySigner.Pick(PKeyType.PayItem),
                        ID = this.ID,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        DateIndex = this.DateIndex,
                        Name = this.Name,
                        Value = this.Value,
                        AdminID = this.AdminID,
                        PayID = this.PayID,
                        ActualFormula = this.ActualFormula,
                        WageItemFormula = this.WageItemFormula,
                        Description = this.Description,
                    });
                }
                //修改
                else
                {
                    repository.Update<PayItems>(new
                    {
                        Value = this.Value,
                        UpdateDate = DateTime.Now,
                        ActualFormula = this.ActualFormula,
                        WageItemFormula = this.WageItemFormula,
                        Description = this.Description,
                    }, a => a.PayID == this.PayID && a.Name == this.Name);
                }



                //操作成功
                if (this != null && EnterSuccess != null)
                    this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }

        /// <summary>
        /// 废弃
        /// </summary>
        public void Abandon()
        {
            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                repository.Delete<Layers.Data.Sqls.PvbErm.PayItems>(item => item.PayID == this.PayID);

                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this));
                }
            }
        }
        #endregion
    }
}