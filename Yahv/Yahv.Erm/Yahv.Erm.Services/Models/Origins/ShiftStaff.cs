using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbErm;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Usually;

namespace Yahv.Erm.Services.Models.Origins
{
    /// <summary>
    /// 轮班员工
    /// </summary>
    public class ShiftStaff : IUnique
    {
        /// <summary>
        /// 员工ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 轮班班别
        /// </summary>
        public string ShiftSchedule { get; set; }

        /// <summary>
        /// 轮班规则
        /// </summary>
        public string ShiftRules { get; set; }

        /// <summary>
        /// 下次轮班班别
        /// </summary>
        public string NextSchedulingID { get; set; }

        /// <summary>
        /// 添加人
        /// </summary>
        public string CreatorID { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        public string ModifyID { get; set; }

        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateDate { get; set; }

        #region 扩展属性

        /// <summary>
        /// 员工
        /// </summary>
        public Staff Staff { get; set; }

        /// <summary>
        /// 添加人
        /// </summary>
        public Admin Creator { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        public Admin Modify { get; set; }

        /// <summary>
        /// 当前班别
        /// </summary>
        public Scheduling CurrentScheduling { get; set; }

        /// <summary>
        /// 下次轮班班别
        /// </summary>
        public Scheduling NextScheduling { get; set; }

        #endregion

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

        #region 持久化
        /// <summary>
        /// 添加/修改
        /// </summary>
        public void Enter()
        {
            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                if (!repository.ReadTable<ShiftStaffs>().Any(t => t.ID == this.ID))
                {
                    //添加
                    repository.Insert(new ShiftStaffs()
                    {
                        ID = this.ID,
                        ShiftSchedule = this.ShiftSchedule,
                        ShiftRules = this.ShiftRules,
                        NextSchedulingID = this.NextSchedulingID,
                        CreatorID = this.CreatorID,
                        ModifyID = this.ModifyID,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                    });
                }
                else
                {
                    //修改
                    repository.Update<ShiftStaffs>(new
                    {
                        ShiftSchedule = this.ShiftSchedule,
                        ShiftRules = this.ShiftRules,
                        NextSchedulingID = this.NextSchedulingID,
                        ModifyID = this.ModifyID,
                        UpdateDate = DateTime.Now,
                    }, t => t.ID == this.ID);
                }

                //操作成功
                if (this != null && EnterSuccess != null)
                {
                    this.EnterSuccess(this, new SuccessEventArgs(this));
                }
            }
        }

        /// <summary>
        /// 废弃(物理删除)
        /// </summary>
        public void Abandon()
        {
            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                repository.Delete<ShiftStaffs>(item => item.ID == this.ID);

                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this));
                }
            }
        }
        #endregion
    }
}
