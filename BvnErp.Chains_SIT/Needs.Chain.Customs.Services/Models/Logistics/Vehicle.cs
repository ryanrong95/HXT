using Needs.Linq;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 车辆信息
    /// </summary>
    [Serializable]
    public class Vehicle : IUnique, IPersist
    {
        string id;
        public string ID
        {
            get
            {
                return this.id ?? string.Concat(this.VehicleType, this.License).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        public  Carrier Carrier { get; set; }

        /// <summary>
        /// 车牌类型
        /// </summary>
        public Enums.VehicleType VehicleType { get; set; }

        /// <summary>
        /// 车牌号
        /// </summary>
        public string License { get; set; }

        /// <summary>
        /// 香港车牌号
        /// </summary>
        public string HKLicense { get; set; }

        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }
        /// <summary>
        /// 车重
        /// </summary>

        public string Weight { get; set; }

        /// <summary>
        /// 尺寸
        /// </summary>
        public string Size { get; set; }

        public Vehicle()
        {
            this.CreateDate = this.UpdateDate = DateTime.Now;
            this.Status = Enums.Status.Normal;
        }
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
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.Vehicles
                    {
                        ID = this.ID,
                        Type = (int)this.VehicleType,
                        CarrierID = this.Carrier.ID,
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
                    reponsitory.Update(new Layer.Data.Sqls.ScCustoms.Vehicles
                    {
                        ID = this.ID,
                        Type = (int)this.VehicleType,
                        CarrierID = this.Carrier.ID,
                        License = this.License,
                        HKLicense = string.IsNullOrEmpty(this.HKLicense)?"": this.HKLicense,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Weight = this.Weight,
                        Size = this.Size
                    }, item => item.ID == this.ID);
                }
                this.OnEnterSuccess();
            }
        }

        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;

        /// <summary>
        /// 删除
        /// </summary>
        public void Abandon()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Vehicles>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);
            }

            this.OnAbandonSuccess();
        }

        virtual protected void OnAbandonSuccess()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                //成功后触发事件
                this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        virtual public void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }
    }
}