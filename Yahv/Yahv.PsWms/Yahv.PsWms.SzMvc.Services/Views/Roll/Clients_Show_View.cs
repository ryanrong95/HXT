using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Yahv.Linq;
using Yahv.Linq.Generic;
using Yahv.Utils.Converters.Contents;

namespace Yahv.PsWms.SzMvc.Services.Views.Roll
{

    public class Clients_Show_View : QueryRoll<ClientShow, ClientShow, PsOrderRepository>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public Clients_Show_View()
        {
        }

        protected override IQueryable<ClientShow> GetIQueryable(Expression<Func<ClientShow, bool>> expression, params LambdaExpression[] expressions)
        {
            var clients = new Origins.ClientsOrigin(this.Reponsitory);
            var siteusers = new Origins.SiteusersOrigin(this.Reponsitory);

            var linq = from entity in clients
                       join site in siteusers on entity.SiteuserID equals site.ID
                       select new ClientShow
                       {
                           ID = entity.ID,
                           SiteuserID = entity.SiteuserID,
                           Name = entity.Name,
                           CreateDate = entity.CreateDate,
                           ModifyDate = entity.ModifyDate,

                           Username = site.Username,
                           Password = site.Password,
                           LoginDate = site.LoginDate,
                       };

            foreach (var predicate in expressions)
            {
                linq = linq.Where(predicate as Expression<Func<ClientShow, bool>>);
            }
            return linq.Where(expression);
        }

        protected override IEnumerable<ClientShow> OnReadShips(ClientShow[] results)
        {
            var linq = from entity in results
                       select new ClientShow
                       {
                           ID = entity.ID,
                           SiteuserID = entity.SiteuserID,
                           Name = entity.Name,
                           CreateDate = entity.CreateDate,
                           ModifyDate = entity.ModifyDate,

                           Username = entity.Username,
                           Password = entity.Password,
                           LoginDate = entity.LoginDate,
                       };
            return linq;
        }
    }

    public class ClientShow : IUnique
    {
        public string ID { get; set; }

        public string SiteuserID { get; set; }

        public string Name { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime ModifyDate { get; set; }

        public DateTime? LoginDate { get; set; }
    }
}
