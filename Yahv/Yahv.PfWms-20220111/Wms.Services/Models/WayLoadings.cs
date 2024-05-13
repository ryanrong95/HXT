using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms.Services.Extends;
using Yahv.Linq;
using Yahv.Linq.Persistence;
using Yahv.Usually;

namespace Wms.Services.Models
{
    public class WayLoadings : IUnique, IPersisting
    {

        #region 事件
        //Enter成功
        public event SuccessHanlder WayLoadingSuccess;
        //Enter失败
        public event ErrorHanlder WayLoadingFailed;
        #endregion

        #region 属性

        /// <summary>
        /// 唯一码，同WaybillID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 提货时间
        /// </summary>
        public DateTime TakingDate { get; set; }

        /// <summary>
        /// 提货地址
        /// </summary>
        public string TakingAddress { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public string TakingContact { get; set; }

        /// <summary>
        /// 联系人电话
        /// </summary>
        public string TakingPhone { get; set; }

        /// <summary>
        /// 两地车牌1
        /// </summary>
        public string CarNumber1 { get; set; }

        /// <summary>
        /// 司机名字
        /// </summary>
        public string Driver { get; set; }

        /// <summary>
        /// 汽车荷载量
        /// </summary>
        public int Carload { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifyDate { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatorID { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        public string ModifierID { get; set; }
        #endregion

        #region 方法

        public void Enter()
        {
            try
            {
                using (var repository=new PvWmsRepository())
                {
                    this.CreateDate = DateTime.Now;
                    repository.Insert(this.ToLinq());
                }
                this.WayLoadingSuccess?.Invoke(this, new SuccessEventArgs(this));
            }
            catch 
            {
                this.WayLoadingFailed?.Invoke(this, new ErrorEventArgs("Failed!!"));
            }
        }

        public void Abandon()
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
