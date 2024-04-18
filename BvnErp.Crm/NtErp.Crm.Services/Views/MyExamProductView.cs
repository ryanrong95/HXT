using NtErp.Crm.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Views
{
    public class MyExamProductView : ExamProductView
    {
        AdminTop admin;

        private MyExamProductView()
        {

        }

        public MyExamProductView(string AdminID)
        {
            this.admin = Extends.AdminExtends.GetTop(AdminID);
        }

        protected override IQueryable<ExamProduct> GetIQueryable(Expression<Func<ExamProduct, bool>> expression, params LambdaExpression[] expressions)
        {
            var linq = base.GetIQueryable(expression, expressions);
            if (this.admin == null)
            {
                return linq;
            }
            if(admin.JobType == Enums.JobType.Sales)
            {
                var mapsclient = this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsClient>().Where(item => item.AdminID == this.admin.ID);

                linq = from maps in mapsclient
                       join project in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Projects>() on maps.ClientID equals project.ClientID
                       join mapsproject in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsProject>() on project.ID equals mapsproject.ProjectID
                       join product in linq on mapsproject.ProductItemID equals product.ID
                       select product;
            }
            else
            {
                var mapsadmin = this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsAdmin>().Where(item => item.AdminID == this.admin.ID);

                linq = from maps in mapsadmin
                       join standard in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.StandardProducts>()
                       on maps.ManufactureID equals standard.ManufacturerID
                       join product in linq on standard.ID equals product.StandardID
                       select product;
            }

            return linq;
        }
    }
}
