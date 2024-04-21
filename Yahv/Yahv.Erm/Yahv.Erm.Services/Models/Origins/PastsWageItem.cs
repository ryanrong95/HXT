using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Usually;

namespace Yahv.Erm.Services.Models.Origins
{
    public class PastsWageItem : IUnique
    {
        #region 属性
        /// <summary>
        /// 主键
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 职员ID
        /// </summary>
        public string StaffID { get; set; }

        /// <summary>
        /// 工资项ID
        /// </summary>
        public string WageItemID { get; set; }

        public string WageItemName { get; set; }

        /// <summary>
        /// 工资默认值
        /// </summary>
        public decimal? DefaultValue { get; set; }

        /// <summary>
        /// 录入日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 录入人
        /// </summary>
        public string AdminID { get; set; }

        public string AdminName { get; set; }
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
        /// 添加
        /// </summary>
        public void Enter()
        {
            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                //添加
                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    repository.Insert(new Layers.Data.Sqls.PvbErm.Pasts_MapsWageItem()
                    {
                        ID = PKeySigner.Pick(PKeyType.Pasts_MapsWageItem),
                        StaffID = this.StaffID,
                        WageItemID = this.WageItemID,
                        DefaultValue = this.DefaultValue,
                        CreateDate = this.CreateDate,
                        AdminID = this.AdminID,
                    });
                }

                //操作成功
                if (this != null && EnterSuccess != null)
                    this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }

        #endregion
    }
}
