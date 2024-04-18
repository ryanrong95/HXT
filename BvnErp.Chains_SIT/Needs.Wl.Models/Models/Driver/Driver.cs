using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 司机
    /// </summary>
    [Serializable]
    public class Driver : IUnique, IPersist
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
        /// 名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 名称
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// 大陆手机
        /// </summary>
        public string Mobile { get; set; } = string.Empty;

        /// <summary>
        /// 驾驶证号码、证件号码
        /// </summary>
        public string License { get; set; } = string.Empty;

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
        /// 海关编号
        /// </summary>
        public string HSCode { get; set; }

        /// <summary>
        ///司机卡号
        /// </summary>
        public string DriverCardNo { get; set; }

        /// <summary>
        /// 香港手机号
        /// </summary>
        public string HKMobile { get; set; }

        /// <summary>
        /// 口岸电子编号
        /// </summary>
        public string PortElecNo { get; set; }

        /// <summary>
        /// 寮步密码
        /// </summary>
        public string LaoPaoCode { get; set; }
        /// <summary>
        /// 是否中港贸易
        /// </summary>
        public bool? IsChcd { get; set; } 
        public event SuccessHanlder AbandonSuccess;
        //public event ErrorHanlder AbandonError;
        public event SuccessHanlder EnterSuccess;
        //public event ErrorHanlder EnterError;


        public Driver()
        {
            this.IsChcd = false;
        }

        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Drivers>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    reponsitory.Insert<Layer.Data.Sqls.ScCustoms.Drivers>(new Layer.Data.Sqls.ScCustoms.Drivers()
                    {
                        ID = this.ID,
                        CarrierID = this.CarrierID,
                        Name = this.Name,
                        Code = this.Code,
                        Mobile = this.Mobile,
                        License = this.License,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        HSCode = this.HSCode,
                        DriverCardNo = this.DriverCardNo,
                        HKMobile = this.HKMobile,
                        PortElecNo = this.PortElecNo,
                        LaoPaoCode = this.LaoPaoCode,
                        IsChcd=this.IsChcd
                    });
                }
                else
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.Drivers>(new
                    {
                        CarrierID = this.CarrierID,
                        Name = this.Name,
                        Code = this.Code,
                        Mobile = this.Mobile,
                        License = this.License,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        HSCode = this.HSCode,
                        DriverCardNo = this.DriverCardNo,
                        HKMobile = this.HKMobile,
                        PortElecNo = this.PortElecNo,
                        LaoPaoCode = this.LaoPaoCode,
                        IsChcd = this.IsChcd
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
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Drivers>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);
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
