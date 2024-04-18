using Layer.Data.Sqls.ScCustoms;
using Needs.Ccs.Services.Hanlders;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 库存库
    /// </summary>
    public sealed class HKStoreStorage : StoreStorage
    {
        public HKStoreStorage() : base()
        {

        }

        /// <summary>
        /// 产品上架
        /// 分拣 完成上架
        /// </summary>
        public void InStore()
        {
            base.Enter();
        }

        /// <summary>
        /// 产品下架
        /// 出库 完成下架
        /// </summary>
        /// <param name="sorting">出库明细项</param>
        public void OutStore(ExitNoticeItem item)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //更新库存数据
                this.Quantity -= item.Quantity;
                if (this.Quantity != 0M)
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.StoreStorages>(new
                    {
                        Quantity = this.Quantity,
                        UpdateDate = DateTime.Now,
                    }, t => t.ID == this.ID);
                }
                else
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.StoreStorages>(new
                    {
                        Status = Enums.Status.Delete
                    }, t => t.ID == this.ID);
                }
            }
        }

        /// <summary>
        /// 产品下架
        /// 拣货 完成下架
        /// </summary>
        public void OutStore()
        {

        }
    }
}