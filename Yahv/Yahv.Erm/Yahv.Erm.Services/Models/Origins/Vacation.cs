using System.Linq;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbErm;
using Layers.Linq;
using Yahv.Linq;
using Yahv.Usually;
using System;
using Yahv.Utils.Converters.Contents;
using Layers.Data;
using Yahv.Underly;

namespace Yahv.Erm.Services.Models.Origins
{

    /// <summary>
    /// 员工假期
    /// </summary>
    public class Vacation : IUnique
    {
        #region 属性

        public string ID { get; set; }

        public string StaffID { get; set; }

        public VacationType Type { get; set; }

        /// <summary>
        /// 有效期开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 有效期结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 剩余假期天数
        /// </summary>
        public decimal Lefts { get; set; }

        public decimal? Total { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        #endregion

        public Vacation()
        {
            this.CreateDate = this.UpdateDate = DateTime.Now;
        }

        #region 持久化

        /// <summary>
        /// 添加
        /// </summary>
        public void Enter()
        {
            using (var repository = new PvbErmReponsitory())
            {
                if (!repository.ReadTable<Vacations>().Any(t => t.ID == this.ID))
                {
                    this.ID = PKeySigner.Pick(PKeyType.Vacation);
                    repository.Insert(new Vacations
                    {
                        ID = this.ID,
                        StaffID = this.StaffID,
                        Type = (int)this.Type,
                        StartTime = this.StartTime,
                        EndTime = this.EndTime,
                        Lefts = this.Lefts,
                        CreateDate= this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Total = this.Total,
                    });
                }
                else
                {
                    repository.Update<Vacations>(new
                    {
                        this.StaffID,
                        Type = (int)this.Type,
                        this.StartTime,
                        this.EndTime,
                        this.Lefts,
                        this.Total,
                        UpdateDate = DateTime.Now,
                    }, item => item.ID == this.ID);
                }
            }
        }

        /// <summary>
        /// 废弃
        /// </summary>
        public void Abandon()
        {
            using (var repository = new PvbErmReponsitory())
            {
                repository.Delete<Vacations>(item => item.ID == this.ID);
            }
        }
        #endregion
    }
}