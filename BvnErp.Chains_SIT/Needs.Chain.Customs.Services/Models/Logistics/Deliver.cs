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
    /// 送货信息
    /// </summary>
    public class Deliver : IUnique, IPersist
    {
        private string id;
        public string ID
        {
            get
            {
                return this.id ?? string.Concat(this.Driver.ID, this.Vehicle.ID, this.Contact, this.Mobile, this.Address).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 司机信息
        /// </summary>
        public Driver Driver { get; set; }

        /// <summary>
        /// 车辆信息
        /// </summary>
        public Vehicle Vehicle { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public string Contact { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 送货地址
        /// </summary>
        public string Address { get; set; }

        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public Deliver()
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
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Delivers>().Count(item => item.ID == this.ID);
                if (count == 0)
                {
                    reponsitory.Insert(this.ToLinq());
                }
                else
                {
                    reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
                }
            }
        }
    }
}