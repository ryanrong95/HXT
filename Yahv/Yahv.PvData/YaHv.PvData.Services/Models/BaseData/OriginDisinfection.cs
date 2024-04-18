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
using Yahv.Utils.Converters.Contents;

namespace YaHv.PvData.Services.Models
{
    /// <summary>
    /// 产地消毒/检疫
    /// </summary>
    public class OriginDisinfection : IUnique
    {
        #region 属性

        string id;
        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID
        {
            get
            {
                //编码规则：品牌+制造商+封装+包装的MD5
                return this.id ?? string.Concat(this.Origin).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 原产地
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// 消毒/检疫开始日期
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 消毒/检疫截止日期
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
                if (!repository.ReadTable<Layers.Data.Sqls.PvData.OriginsDisinfection>().Any(t => t.ID == this.ID))
                {
                    repository.Insert(new Layers.Data.Sqls.PvData.OriginsDisinfection()
                    {
                        ID = this.ID,
                        Origin = this.Origin,
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
                    repository.Update<Layers.Data.Sqls.PvData.OriginsDisinfection>(new
                    {
                        Origin = this.Origin,
                        StartDate = this.StartDate,
                        EndDate = this.EndDate,
                        Status = (int)GeneralStatus.Normal,
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
                repository.Update<Layers.Data.Sqls.PvData.OriginsDisinfection>(new
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
