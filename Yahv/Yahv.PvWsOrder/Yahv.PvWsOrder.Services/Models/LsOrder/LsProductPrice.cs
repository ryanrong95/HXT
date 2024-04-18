using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Services.Models.LsOrder;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.Models
{
    /// <summary>
    /// 库位租赁价格指导类
    /// </summary>
    public class LsProductPrice : LsProductPrices
    {
        public LsProductPrice()
        {
            Currency = Currency.CNY;
            CreateDate = DateTime.Now;
        }

        #region 持久化
        public void Enter()
        {
            using (PvLsOrderReponsitory Reponsitory = new PvLsOrderReponsitory())
            {
                //保存新指导价
                int count = Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.LsProductsPricesTopView>()
                            .Count(item => item.ID == this.ID);
                if (count == 0)
                {
                    Reponsitory.Insert(new Layers.Data.Sqls.PvLsOrder.LsProductPrices
                    {
                        ID = this.ID,
                        ProductID = this.ProductID,
                        Month = this.Month,
                        Currency = (int)this.Currency,
                        Price = this.Price,
                        CreateDate = this.CreateDate,
                        Creator = this.Creator,
                        Summary = this.Summary,
                    });
                }
                else  //更新
                {
                    Reponsitory.Update<Layers.Data.Sqls.PvLsOrder.LsProductPrices>(new
                    {
                        Price = this.Price,
                        Summary = this.Summary,
                        Creator = this.Creator,
                    }, item => item.ID == this.ID);
                }
            };
        }

        public void Delete()
        {
            using (PvLsOrderReponsitory Reponsitory = new PvLsOrderReponsitory())
            {
                int count = Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.LsProductsPricesTopView>()
                            .Count(item => item.ID == this.ID);
                if (count > 0)
                {
                    Reponsitory.Delete<Layers.Data.Sqls.PvLsOrder.LsProductPrices>(item => item.ID == this.ID);
                }
            }
        }

        #endregion
    }
}
