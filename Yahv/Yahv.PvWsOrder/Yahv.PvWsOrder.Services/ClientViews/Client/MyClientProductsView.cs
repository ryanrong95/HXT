using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Services.Models;
using Yahv.Services.Views;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;

namespace Yahv.PvWsOrder.Services.ClientViews
{
    /// <summary>
    /// 产品视图
    /// </summary>
    public class MyClientProductsView : ClientProductsTopView<ScCustomReponsitory>
    {
        private string enterpriseid;

        /// <summary>
        /// 无参构造函数
        /// </summary>
        private MyClientProductsView()
        {

        }

        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="EnterpriseID">客户ID</param>
        public MyClientProductsView(string EnterpriseID)
        {
            this.enterpriseid = EnterpriseID;
        }

        /// <summary>
        /// 查询结果集
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<ClientProduct> GetIQueryable()
        {
            return base.GetIQueryable().Where(item => item.ClientID == this.enterpriseid && item.Status == GeneralStatus.Normal);
        }


        public void Enter(ClientProduct product)
        {
            int count = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.ClientProducts>().Count(item => item.ID == product.ID);
            if (count == 0)
            {
                Reponsitory.Insert(new Layers.Data.Sqls.PvWsOrder.ClientProducts
                {
                    ID = Guid.NewGuid().ToString(),
                    ClientID = product.ClientID,
                    Name = product.Name,
                    Model = product.Model,
                    Manufacturer = product.Manufacturer,
                    Batch = product.Batch,
                    Status = (int)GeneralStatus.Normal,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                });
            }
            else
            {
                Reponsitory.Update<Layers.Data.Sqls.PvWsOrder.ClientProducts>(new
                {
                    ClientID = product.ClientID,
                    Name = product.Name,
                    Model = product.Model,
                    Manufacturer = product.Manufacturer,
                    Batch = product.Batch,
                    Status = (int)product.Status,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                }, item => item.ID == product.ID);
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        public void Abandon(ClientProduct products)
        {
            Reponsitory.Update<Layers.Data.Sqls.PvWsOrder.ClientProducts>(new
            {
                Status = (int)GeneralStatus.Deleted
            }, item => item.ID == products.ID);
        }
    }
}
