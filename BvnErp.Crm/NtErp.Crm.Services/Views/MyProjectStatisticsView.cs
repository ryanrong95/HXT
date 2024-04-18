using Needs.Erp.Generic;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Views
{
    public class MyProjectStatisticsView : ProjectStatisticsAlls
    {
        //人员对象
        AdminTop Admin;

        /// <summary>
        /// 无参构造函数
        /// </summary>
        public MyProjectStatisticsView()
        {

        }

        /// <summary>
        /// 无参构造函数
        /// </summary>
        public MyProjectStatisticsView(string adminID)
        {
            this.Admin = Extends.AdminExtends.GetTop(adminID);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="admin">人员对象</param>
        public MyProjectStatisticsView(IGenericAdmin admin)
        {
            this.Admin = Extends.AdminExtends.GetTop(admin.ID);
        }

        /// <summary>
        /// 获取我的销售机会统计数据
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<ProductItemExtend> GetIQueryable()
        {
            //获取所有员工
            //var mystaffids = new MyStaffsView(this.Admin, Reponsitory);

            //if (Admin.JobType != JobType.TPM)
            //{
            //    var linq1 = from maps in Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsClient>()
            //                join product in base.GetIQueryable() on maps.ClientID equals product.ProjectClientID
            //                join admin in mystaffids on maps.AdminID equals admin.ID
            //                select product;

            //    if (Admin.JobType != JobType.Sales)
            //    {
            //        var linq2 = (from maps in Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsAdmin>()
            //                     join product in base.GetIQueryable() on maps.ManufactureID equals product.standardProduct.Manufacturer.ID
            //                     join admin in mystaffids on maps.AdminID equals admin.ID
            //                     select product).Distinct();

            //        linq1 = linq1.Union(linq2);
            //    }

            //    return linq1;
            //}

            return base.GetIQueryable();
        }

        public IQueryable<ProductItemExtend> GetOwn()
        {
            var mystaffids = new MyStaffsView(this.Admin, Reponsitory);
            var linq1 = from maps in Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsClient>()
                        join product in base.GetIQueryable() on maps.ClientID equals product.ProjectClientID
                        join admin in mystaffids on maps.AdminID equals admin.ID
                        select product;

            return linq1;
        }
    }
}
