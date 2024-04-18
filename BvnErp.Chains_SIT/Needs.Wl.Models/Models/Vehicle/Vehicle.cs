using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 车辆信息
    /// </summary>
    [Serializable]
    public class Vehicle : IUnique, IPersist
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; } = string.Empty;

        /// <summary>
        /// 承运商 ID
        /// </summary>
        public string CarrierID { get; set; } = string.Empty;

        /// <summary>
        /// 车牌类型
        /// </summary>
        public Enums.VehicleType Type { get; set; }

        /// <summary>
        /// 车牌号
        /// </summary>
        public string License { get; set; } = string.Empty;

        /// <summary>
        /// 香港车牌号
        /// </summary>
        public string HKLicense { get; set; } = string.Empty;

        /// <summary>
        /// Status
        /// </summary>
        public Enums.Status Status { get; set; }

        /// <summary>
        /// CreateDate
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// UpdateDate
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// 车重
        /// </summary>
        public string Weight { get; set; } = string.Empty;

        /// <summary>
        /// 尺寸
        /// </summary>
        public string Size { get; set; } = string.Empty;


        public event SuccessHanlder AbandonSuccess;
        //public event ErrorHanlder AbandonError;
        public event SuccessHanlder EnterSuccess;
        //public event ErrorHanlder EnterError;
        
        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Vehicles>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    reponsitory.Insert<Layer.Data.Sqls.ScCustoms.Vehicles>(new Layer.Data.Sqls.ScCustoms.Vehicles
                    {
                        ID = this.ID,
                        CarrierID = this.CarrierID,
                        Type = (int)this.Type,
                        License = this.License,
                        HKLicense = this.HKLicense,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Weight = this.Weight,
                        Size = this.Size
                    });
                }
                else
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.Vehicles>(new
                    {
                        CarrierID = this.CarrierID,
                        Type = (int)this.Type,
                        License = this.License,
                        HKLicense = this.HKLicense,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Weight = this.Weight,
                        Size = this.Size
                    }, item => item.ID == this.ID);
                }
            }

            this.OnEnter();
        }

        virtual public void OnEnter()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        public void Abandon()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Vehicles>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);
            }

            this.OnAbandon();
        }

        virtual protected void OnAbandon()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                //成功后触发事件
                this.AbandonSuccess(this, new SuccessEventArgs(this));
            }
        }

    }
}
