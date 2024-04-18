using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Layer.Data.Sqls;
using NtErp.Crm.Services.Models.Generic;

namespace NtErp.Crm.Services.Views.Generic
{
    public class ClientClassicAlls : Needs.Linq.Generic.Query1Classics<ClientDossier, BvCrmReponsitory>
    {
        public ClientClassicAlls()
        {

        }

        protected ClientClassicAlls(BvCrmReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<ClientDossier> GetIQueryable(Expression<Func<ClientDossier, bool>> expression, params LambdaExpression[] expressions)
        {
            var clients = new ClientAlls(this.Reponsitory);

            IQueryable<Models.Client> linq = clients;

            foreach (var predicate in expressions)
            {
                linq = linq.Where(predicate as Expression<Func<Models.Client, bool>>);
            }

            var clientdossier = from client in linq
                                select new ClientDossier
                                {
                                    Client = client,
                                };

            return clientdossier.Where(expression);
        }

        protected override IEnumerable<ClientDossier> OnReadShips(ClientDossier[] results)
        {
            AdminTopView admins = new AdminTopView(this.Reponsitory);
            IndustryAlls industries = new IndustryAlls(this.Reponsitory);
            CompanyAlls companies = new CompanyAlls(this.Reponsitory);

            var ids = results.Select(item => item.Client.ID).ToArray();

            var admin_linq = from maps in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsClient>().AsEnumerable()
                             join admin in admins on maps.AdminID equals admin.ID
                             where ids.Contains(maps.ClientID)
                             select new
                             {
                                 maps.ClientID,
                                 admin,
                             };

            var industry_linq = from maps in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsClient>().AsEnumerable()
                                join industry in industries on maps.IndustryID equals industry.ID
                                where ids.Contains(maps.ClientID)
                                select new
                                {
                                    maps.ClientID,
                                    industry,
                                };


            var Manufacture_linq = from maps in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsClient>().AsEnumerable()
                                   join manufacture in companies on maps.ManufacturerID equals manufacture.ID
                                   where ids.Contains(maps.ClientID)
                                   select new
                                   {
                                       maps.ClientID,
                                       manufacture,
                                   };

            return from clientdossier in results
                   join admin in admin_linq on clientdossier.Client.ID equals admin.ClientID into _admins
                   join industry in industry_linq on clientdossier.Client.ID equals industry.ClientID into _industries
                   join manufacture in Manufacture_linq on clientdossier.Client.ID equals manufacture.ClientID into _manufactures
                   select new ClientDossier
                   {
                       Client = clientdossier.Client,
                       Admins = _admins.Select(item => item.admin).ToArray(),
                       Industries = _industries.Select(item => item.industry).ToArray(),
                       Manufactures = _manufactures.Select(item => item.manufacture).ToArray(),
                   };
        }


        /// <summary>
        /// 关联查询条件
        /// </summary>
        /// <param name="expressions"></param>
        /// <returns></returns>
        virtual public string[] MapCondition(Expression<Func<Layer.Data.Sqls.BvCrm.MapsClient, bool>> expression, string[] clientids)
        {
            var linq = this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsClient>().Where(expression).ToArray();

            if (clientids != null)
            {
                linq = linq.Where(item => clientids.Contains(item.ClientID)).ToArray();
            }

            return linq.Select(item => item.ClientID).ToArray();
        }
    }
}
