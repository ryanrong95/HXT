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
    /// 提货信息
    /// </summary>
    public class Consignee : IUnique, IPersist
    {
        private string id;
        public string ID
        {
            get
            {
                return this.id ?? string.Concat(this.Mobile, this.IDNumber).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 提货人名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 提货人电话
        /// </summary>
        public string Mobile { get; set; }

        public Enums.IDType IDType { get; set; }

        public string IDNumber { get; set; }

        public DateTime CreateDate { get; set; }

        public Consignee()
        {
            this.CreateDate = DateTime.Now;
        }

        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Consignees>().Count(item => item.ID == this.ID);
                if (count == 0)
                {
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.Consignees
                    {
                        ID = this.ID,
                        Name = this.Name,
                        Mobile = this.Mobile,
                        IDType = (int)this.IDType,
                        IDNumber = this.IDNumber,
                        CreateDate = this.CreateDate,
                    });
                }
                else
                {
                    reponsitory.Update(new Layer.Data.Sqls.ScCustoms.Consignees
                    {
                        ID = this.ID,
                        Name = this.Name,
                        Mobile = this.Mobile,
                        IDType = (int)this.IDType,
                        IDNumber = this.IDNumber,
                        CreateDate = this.CreateDate,
                    }, item => item.ID == this.ID);
                }
            }
        }
    }
}