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
    /// 海关卡控
    /// </summary>
    public class CustomsControl : IUnique
    {
        #region 属性

        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 海关卡控类型：卡控型号、卡控海关编码
        /// </summary>
        public CustomsControlType Type { get; set; }

        /// <summary>
        /// 海关编码
        /// </summary>
        public string HSCode { get; set; }

        /// <summary>
        /// 报关品名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string PartNumber { get; set; }

        /// <summary>
        /// 正常、删除
        /// </summary>
        public GeneralStatus Status { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 更新时间
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
                this.ID = Layers.Data.PKeySigner.Pick(PKeyType.CustomsControl);
                repository.Insert(new Layers.Data.Sqls.PvData.CustomsControls()
                {
                    ID = this.ID,
                    Type = (int)this.Type,
                    HSCode = this.HSCode,
                    Name = this.Name,
                    PartNumber = this.PartNumber,
                    Status = (int)GeneralStatus.Normal,
                    CreateDate = DateTime.Now,
                    ModifyDate = DateTime.Now
                });
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
                repository.Update<Layers.Data.Sqls.PvData.CustomsControls>(new
                {
                    Status = (int)GeneralStatus.Deleted,
                    ModifyDate = DateTime.Now
                }, cc => cc.ID == this.ID);
            }

            if (this != null && this.AbandonSuccess != null)
            {
                this.AbandonSuccess(this, new SuccessEventArgs(this));
            }
        }

        #endregion
    }
}
