using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;

namespace Yahv.Services.Models
{
    public class Transport : Yahv.Linq.IUnique
    {
        #region 属性
        string id;
        public string ID
        {
            get
            {
                return this.id ?? string.Join("", this.EnterpriseID, this.Type, this.CarNumber1).MD5();

            }
            set
            {
                this.id = value;
            }
        }
        public string EnterpriseID { set; get; }
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
        #endregion
        #region 持久化
        public void Enter()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                if (!repository.ReadTable<Layers.Data.Sqls.PvbCrm.Transports>().Any(item => item.ID == this.ID))
                {
                    repository.Insert(new Layers.Data.Sqls.PvbCrm.Transports
                    {
                        ID = this.ID,
                        EnterpriseID = this.EnterpriseID,
                        Type = (int)this.Type,
                        CarNumber1 = this.CarNumber1,
                        CarNumber2 = this.CarNumber2,
                        Weight = this.Weight,
                        Status = (int)this.Status,
                        Creator ="",
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                    });
                }
            }
        }
        #endregion
    }
}
