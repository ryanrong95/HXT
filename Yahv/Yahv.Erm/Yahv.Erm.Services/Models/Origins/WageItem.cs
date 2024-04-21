using System;
using System.Collections.Generic;
using System.Linq;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbErm;
using Layers.Linq;
using Yahv.Linq;
using Yahv.Usually;
using Yahv.Underly;

namespace Yahv.Erm.Services.Models.Origins
{
    /// <summary>
    /// 工资项
    /// </summary>
    public class WageItem : IUnique
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
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 录入人ID
        /// </summary>
        public string InputerId { get; set; }

        /// <summary>
        /// 创建人ID
        /// </summary>
        public string AdminID { get; set; }

        /// <summary>
        /// 普通列（Normal）、计算列（Calc）、数据列（Data）
        /// </summary>
        public WageItemType Type { get; set; }

        /// <summary>
        /// 是否可更改
        /// </summary>
        public bool IsImport { get; set; }

        /// <summary>
        /// 计算顺序
        /// </summary>
        public int? CalcOrder { get; set; }

        /// <summary>
        /// 公式内容
        /// </summary>
        public string Formula { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 管理顺序
        /// </summary>
        public int OrderIndex { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public Status Status { get; set; }

        /// <summary>
        /// 创建人姓名
        /// </summary>
        public string AdminName { get; set; }

        /// <summary>
        /// 录入人姓名
        /// </summary>
        public string InputerName { get; set; }

        /// <summary>
        /// 工资项下的员工ID
        /// </summary>
        public string[] StaffIds { get; set; }
        #endregion

        #region 持久化
        /// <summary>
        /// 添加
        /// </summary>
        public void Enter()
        {
            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                //添加
                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    //判断工资项是否已存在
                    if (repository.ReadTable<WageItems>().Any(a => a.Name == this.Name))
                    {
                        if (EnterError != null)
                            this.EnterError(this, new ErrorEventArgs("该工资项已经存在!"));

                        return;
                    }

                    repository.Insert(new WageItems()
                    {
                        Status = (int)(Status.Normal),
                        ID = PKeySigner.Pick(PKeyType.WageItem),
                        CreateDate = DateTime.Now,
                        Name = this.Name,
                        AdminID = this.AdminID,
                        OrderIndex = this.OrderIndex,
                        Type = (int)this.Type,
                        Formula = this.Formula,
                        CalcOrder = this.CalcOrder,
                        Description = this.Description,
                        IsImport = this.IsImport,
                        InputerId = this.InputerId,
                    });
                }
                else
                {
                    repository.Update<WageItems>(new
                    {
                        Type = this.Type,
                        Formula = this.Formula,
                        CalcOrder = this.CalcOrder,
                        Description = this.Description,
                        IsImport = this.IsImport,
                        InputerId = this.InputerId,
                    }, item => item.ID == this.ID);
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
                repository.Update<Layers.Data.Sqls.PvbErm.WageItems>(new
                {
                    Status = Status.Delete
                }, item => item.ID == this.ID);

                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this));
                }
            }
        }

        #endregion
    }
}