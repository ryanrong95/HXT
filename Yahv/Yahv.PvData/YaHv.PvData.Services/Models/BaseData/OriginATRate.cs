using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Usually;

namespace YaHv.PvData.Services.Models
{
    /// <summary>
    /// 产地加征税率
    /// </summary>
    public class OriginATRate : IUnique
    {
        #region 属性

        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 税则ID
        /// </summary>
        public string TariffID { get; set; }

        /// <summary>
        /// 海关税则
        /// </summary>
        public Tariff Tariff { get; set; }

        /// <summary>
        /// 原产地
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// 加征税率
        /// </summary>
        public decimal Rate { get; set; }

        /// <summary>
        /// 加征起始日期
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 加征截止日期
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 数据状态：正常、删除
        /// </summary>
        public GeneralStatus Status { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifyDate { get; set; }

        #endregion

        #region 事件

        public event SuccessHanlder EnterSuccess;
        public event SuccessHanlder AbandonSuccess;

        #endregion

        #region 持久化

        public void Enter()
        {
            using (var repository = LinqFactory<PvDataReponsitory>.Create())
            {
                //添加
                if (!repository.ReadTable<Layers.Data.Sqls.PvData.OriginsATRate>().Any(t => t.ID == this.ID))
                {
                    repository.Insert(new Layers.Data.Sqls.PvData.OriginsATRate()
                    {
                        ID = Utils.GuidUtil.NewGuidUp(),
                        TariffID = this.TariffID,
                        Origin = this.Origin,
                        Rate = this.Rate,
                        StartDate = this.StartDate,
                        EndDate = this.EndDate,
                        Status = (int)GeneralStatus.Normal,
                        CreateDate = DateTime.Now,
                        ModifyDate = DateTime.Now
                    });
                }
                //修改
                else
                {
                    repository.Update<Layers.Data.Sqls.PvData.OriginsATRate>(new
                    {
                        TariffID = this.TariffID,
                        Origin = this.Origin,
                        Rate = this.Rate,
                        StartDate = this.StartDate,
                        EndDate = this.EndDate,
                        ModifyDate = DateTime.Now
                    }, a => a.ID == this.ID);
                }
            }

            if (this != null && this.EnterSuccess != null)
            {
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }

        public void Abandon()
        {
            using (var repository = LinqFactory<PvDataReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvData.OriginsATRate>(new
                {
                    Status = (int)GeneralStatus.Deleted,
                    ModifyDate = DateTime.Now
                }, a => a.ID == this.ID);
            }

            if (this != null && this.AbandonSuccess != null)
            {
                this.AbandonSuccess(this, new SuccessEventArgs(this));
            }
        }

        #endregion
    }
}
