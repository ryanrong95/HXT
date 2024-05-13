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
    public class WayChcd : IUnique, IPersisting
    {

        #region 事件
        //Enter成功
        public event SuccessHanlder WayChcdSuccess;
        //Enter失败
        public event ErrorHanlder WaychcdFailed;
        #endregion

        #region 属性

        /// <summary>
        /// 同WaybillID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 货物运输批次号
        /// </summary>
        public string LotNumber { get; set; }

        /// <summary>
        /// 两地车牌1
        /// </summary>
        public string CarNumber1 { get; set; }

        /// <summary>
        /// 两地车牌2
        /// </summary>
        public string CarNumber2 { get; set; }

        /// <summary>
        /// 汽车荷载量
        /// </summary>
        public int Carload { get; set; }

        /// <summary>
        /// 是否只有一辆车
        /// </summary>
        public bool IsOnevehicle { get; set; }

        /// <summary>
        /// 司机名字
        /// </summary>
        public string Driver { get; set; }

        /// <summary>
        /// 计划拉货时间
        /// </summary>
        public DateTime PlanDate { get; set; }

        /// <summary>
        /// 实际起运时间
        /// </summary>
        public DateTime DepartDate { get; set; }

        /// <summary>
        /// 总数量，报关部特殊要求
        /// </summary>
        public int TotalQuantity { get; set; }

        #endregion

        public void Enter()
        {
            try
            {
                using (var repository = new PvWmsRepository())
                {
                    repository.Insert(this.ToLinq());
                }
                this.WayChcdSuccess?.Invoke(this, new SuccessEventArgs(this));
            }
            catch 
            {
                this.WaychcdFailed?.Invoke(this, new ErrorEventArgs("Failed!!"));
            }
            
        }

        public void Abandon()
        {
            throw new NotImplementedException();
        }
    }
}
