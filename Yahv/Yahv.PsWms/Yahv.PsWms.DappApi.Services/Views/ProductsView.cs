using Layers.Data.Sqls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PsWms.DappApi.Services.Enums;
using Yahv.PsWms.DappApi.Services.Models;

namespace Yahv.PsWms.DappApi.Services.Views
{
    public class ProductsView : QueryView<Product, PsWmsRepository>
    {
        #region 构造函数

        public ProductsView()
        {
        }

        public ProductsView(PsWmsRepository reponsitory) : base(reponsitory)
        {
        }

        #endregion

        protected override IQueryable<Product> GetIQueryable()
        {
            var view = from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.Products>()
                       select new Product
                       {
                           ID = entity.ID,
                           Partnumber = entity.Partnumber,
                           Brand = entity.Brand,
                           Package = entity.Package,
                           DateCode = entity.DateCode,
                           Mpq = entity.Mpq,
                           Moq = entity.Moq,
                           CreateDate = entity.CreateDate,
                           ModifyDate = entity.ModifyDate,
                       };

            return view;
        }

        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter(JToken jtoken)
        {
            var product = new Layers.Data.Sqls.PsWms.Products
            {
                Partnumber = jtoken["Partnumber"].Value<string>(),
                Brand = jtoken["Brand"].Value<string>(),
                Package = jtoken["Package"].Value<string>(),
                DateCode = jtoken["DateCode"].Value<string>(),
                Mpq = jtoken["Mpq"]?.Value<int>(),
                Moq = jtoken["Moq"]?.Value<int>(),
            };

            product.ID = Layers.Data.PKeySigner.Pick(PKeyType.Product);
            product.CreateDate = DateTime.Now;
            product.ModifyDate = DateTime.Now;

            Reponsitory.Insert(product);

        }

        /// <summary>
        /// 去持久化
        /// </summary>
        public void Delete(string id)
        {
            Reponsitory.Delete<Layers.Data.Sqls.PsWms.Products>(t => t.ID == id);
        }
    }
}
