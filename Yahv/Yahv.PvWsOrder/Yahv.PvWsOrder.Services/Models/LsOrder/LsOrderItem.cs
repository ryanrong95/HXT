using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvWsOrder.Services.Models
{
    public class LsOrderItem : Yahv.Services.Models.LsOrder.LsOrderItem
    {

        #region 保存修改的单价

        public void Enter()
        {
            SavePrice();
        }
        
        /// <summary>
        /// 保存修改的单价
        /// </summary>
        private void SavePrice()
        {
            using (PvLsOrderReponsitory repository = new PvLsOrderReponsitory())
            {
                var count = repository.ReadTable<Layers.Data.Sqls.PvLsOrder.OrderItems>()
                            .Count(item => item.ID == this.ID);
                if (count > 0)
                {
                    repository.Update<Layers.Data.Sqls.PvLsOrder.OrderItems>(new { UnitPrice = this.UnitPrice }, item => item.ID == this.ID);
                }
            }
        }
        #endregion
    }

   
}
