using System;
using System.Linq;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbErm;
using Layers.Linq;
using Yahv.Linq;
using Yahv.Usually;

namespace Yahv.Erm.Services.Models.Origins
{
    /// <summary>
    /// 个人所得税预扣率表
    /// </summary>
    public class PersonalRate : IUnique
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
        /// 唯一值
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 级数
        /// </summary>
        public int Levels { get; set; }
        /// <summary>
        /// 开始
        /// </summary>
        public decimal BeginAmount { get; set; }
        /// <summary>
        /// 结束
        /// </summary>
        public decimal EndAmount { get; set; }
        /// <summary>
        /// 预税率
        /// </summary>
        public decimal Rate { get; set; }
        /// <summary>
        /// 速算扣除数
        /// </summary>
        public decimal Deduction { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? UpdateDate { get; set; }
        /// <summary>
        /// 实际录入人
        /// </summary>
        public string AdminID { get; set; }
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
                if (!repository.ReadTable<PersonalRates>().Any(t => t.ID == this.ID))
                {
                    repository.Insert(new PersonalRates()
                    {
                        ID = this.ID,
                        Levels = this.Levels,
                        BeginAmount = this.BeginAmount,
                        EndAmount = this.EndAmount,
                        Rate = this.Rate,
                        Deduction = this.Deduction,
                        Description = this.Description,
                        CreateDate = DateTime.Now,                        
                        AdminID = this.AdminID,
                    });
                }
                //修改
                else
                {
                    repository.Update<PersonalRates>(new
                    {
                        ID = this.ID,
                        Levels = this.Levels,
                        BeginAmount = this.BeginAmount,
                        EndAmount = this.EndAmount,
                        Rate = this.Rate,
                        Deduction = this.Deduction,
                        Description = this.Description,
                        UpdateDate = DateTime.Now,
                        AdminID = this.AdminID,
                    }, t => t.ID == this.ID);
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
                repository.Delete<Layers.Data.Sqls.PvbErm.PersonalRates>(item => item.ID == this.ID);

                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this));
                }
            }
        }

        #endregion
    }
}