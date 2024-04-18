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
    /// 司机
    /// </summary>
    public class Driver : IUnique, IPersist
    {
        string id;
        public string ID
        {
            get
            {
                return this.id ?? string.Concat(this.Name, this.License).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 大陆手机
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 驾驶证号码
        /// </summary>
        public string License { get; set; }

        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

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
        /// 承运商
        /// </summary>
        public Carrier Carrier { get; set; }
        /// <summary>
        /// 是否中港贸易
        /// </summary>
        public bool? IsChcd { get; set; }

        public Driver()
        {
            this.CreateDate = this.UpdateDate = DateTime.Now;
            this.Status = Enums.Status.Normal;
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
                    reponsitory.Insert(this.ToLinq());
                }
                else
                {
                    reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
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
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Drivers>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);
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