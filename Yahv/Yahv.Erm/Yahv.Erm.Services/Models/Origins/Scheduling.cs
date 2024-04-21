using System;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Erm.Services.Models.Origins
{
    /// <summary>
    /// 班别
    /// </summary>
    public class Scheduling : IUnique
    {
        #region 属性
        public string ID { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 岗位ID
        /// </summary>
        public string PostionID { get; set; }

        /// <summary>
        /// 上午开始时间
        /// </summary>
        public TimeSpan? AmStartTime { get; set; }

        /// <summary>
        /// 上午结束时间
        /// </summary>
        public TimeSpan? AmEndTime { get; set; }

        /// <summary>
        /// 下午开始时间
        /// </summary>
        public TimeSpan PmStartTime { get; set; }

        /// <summary>
        /// 下午结束时间
        /// </summary>
        public TimeSpan PmEndTime { get; set; }

        /// <summary>
        /// 判断迟到早退的阈值  以分钟为单位
        /// </summary>
        public int DomainValue { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 主班别（即可以分配给员工的）
        /// </summary>
        public bool IsMain { get; set; }
        #endregion

        #region 持久化

        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvbErmReponsitory>.Create())
            {
                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    this.ID = PKeySigner.Pick(PKeyType.Scheing);

                    reponsitory.Insert(new Layers.Data.Sqls.PvbErm.Schedulings()
                    {
                        ID = this.ID,
                        Name = this.Name,
                        PostionID = this.PostionID,
                        AmEndTime = this.AmEndTime,
                        AmStartTime = this.AmStartTime,
                        DomainValue = this.DomainValue,
                        PmEndTime = this.PmEndTime,
                        PmStartTime = this.PmStartTime,
                        Summary = this.Summary,
                        IsMain = this.IsMain,
                    });
                }
                else
                {
                    reponsitory.Update<Layers.Data.Sqls.PvbErm.Schedulings>(new
                    {
                        Name = this.Name,
                        PostionID = this.PostionID,
                        AmEndTime = this.AmEndTime,
                        AmStartTime = this.AmStartTime,
                        DomainValue = this.DomainValue,
                        PmEndTime = this.PmEndTime,
                        PmStartTime = this.PmStartTime,
                        Summary = this.Summary,
                        IsMain = this.IsMain,
                    }, item => item.ID == this.ID);
                }
            }
        }

        public void Abandon()
        {
            using (var reponsitory = LinqFactory<PvbErmReponsitory>.Create())
            {

                reponsitory.Delete<Layers.Data.Sqls.PvbErm.Schedulings>(item => item.ID == this.ID);
            }
        }
        #endregion
    }
}