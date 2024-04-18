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
    public class AdminClassicsAlls : Needs.Linq.Generic.Query1Classics<AdminDossier, BvCrmReponsitory>
    {
        internal AdminClassicsAlls()
        {

        }

        internal AdminClassicsAlls(BvCrmReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<AdminDossier> GetIQueryable(Expression<Func<AdminDossier, bool>> expression, params LambdaExpression[] expressions)
        {
            var admins = new AdminTopView(this.Reponsitory);

            var linq = admins.AsQueryable();

            foreach (var predicate in expressions)
            {
                linq = linq.Where(predicate as Expression<Func<Models.AdminTop, bool>>);
            }

            var admindossier = from admin in admins
                               select new AdminDossier
                               {
                                   Admin = admin,
                               };

            return admindossier.Where(expression).OrderByDescending(item => item.Admin.UpdateDate);
        }

        protected override IEnumerable<AdminDossier> OnReadShips(AdminDossier[] results)
        {
            CompanyAlls companies = new CompanyAlls(this.Reponsitory);

            var manu_linq = from maps in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsAdmin>().AsEnumerable()
                            join manufacture in companies on maps.ManufactureID equals manufacture.ID
                            select new
                            {
                                maps.AdminID,
                                manufacture,
                            };

            return from admindossier in results
                   join manu in manu_linq on admindossier.Admin.ID equals manu.AdminID into manufactures
                   select new AdminDossier
                   {
                       Admin = admindossier.Admin,
                       Manufactures = manufactures.Select(item => item.manufacture).ToArray(),
                   };
        }
    }
}
