using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using Yahv.Usually;
using Yahv.Utils.Converters.Contents;
using YaHv.Csrm.Services.Extends;

namespace YaHv.Csrm.Services.Models.Origins
{
    public class Transport : Yahv.Linq.IUnique
    {
        public Transport()
        {
            this.CreateDate = this.UpdateDate = DateTime.Now;
            this.Status = GeneralStatus.Normal;
        }
        #region 属性
        string id;
        public string ID
        {
            get
            {
                return this.id ?? string.Join("", this.Enterprise.ID, this.Type, this.CarNumber1).MD5();

            }
            set
            {
                this.id = value;
            }
        }
        string MapsID
        {
            get { return "Transport_" + this.ID; }
        }
        public Enterprise Enterprise { set; get; }
        /// <summary>
        /// 车辆类型
        /// </summary>
        public VehicleType Type { set; get; }
        /// <summary>
        /// 车牌号
        /// </summary>
        public string CarNumber1 { set; get; }
        /// <summary>
        /// 临时车牌
        /// </summary>
        public string CarNumber2 { set; get; }
        /// <summary>
        /// 载重
        /// </summary>
        public string Weight { set; get; }
        /// <summary>
        /// 状态
        /// </summary>
        public GeneralStatus Status { set; get; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { set; get; }
        /// <summary>
        /// 编辑时间
        /// </summary>
        public DateTime UpdateDate { set; get; }
        public string CreatorID { set; get; }
        /// <summary>
        /// 添加人
        /// </summary>
        public Admin Creator {internal set; get; }
        /// <summary>
        /// 是否默认
        /// </summary>
        //public bool IsDefault { set; get; }
        #endregion


        #region 事件
        /// <summary>
        /// EnterSuccess
        /// </summary>
        public event SuccessHanlder EnterSuccess;
        /// <summary>
        /// AbandonSuccess
        /// </summary>
        public event SuccessHanlder AbandonSuccess;
        #endregion

        #region 持久化
        public void Enter()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                if (repository.ReadTable<Layers.Data.Sqls.PvbCrm.Transports>().Any(item => item.ID == this.ID))
                {
                    repository.Update<Layers.Data.Sqls.PvbCrm.Transports>(new
                    {
                        CarNumber2 = this.CarNumber2,
                        Weight = this.Weight,
                        Status = (int)this.Status,
                        UpdateDate = this.UpdateDate
                    }, item => item.ID == this.ID);
                }
                else
                {
                    repository.Insert(this.ToLinq());

                }
                if (this != null && this.EnterSuccess != null)
                {
                    this.EnterSuccess(this, new SuccessEventArgs(this));
                }
            }
        }
        public void Abandon()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                if (repository.ReadTable<Layers.Data.Sqls.PvbCrm.Transports>().Any(item => item.ID == this.ID))
                {
                    repository.Update<Layers.Data.Sqls.PvbCrm.Transports>(new
                    {
                        Status = (int)GeneralStatus.Deleted
                    }, item => item.ID == this.ID);
                }
                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this));
                }
            }
        }
        #endregion
    }
}
