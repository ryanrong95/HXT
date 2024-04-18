using Layer.Data.Sqls;
using Needs.Erp.Generic;
using NtErp.Crm.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Views
{
    public class MyManufactureView : CompanyAlls
    {
        //人员对象
        AdminTop Admin;

        /// <summary>
        /// 无参构造函数
        /// </summary>
        private MyManufactureView()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="admin">人员对象</param>
        public MyManufactureView(IGenericAdmin admin)
        {
            this.Admin = Extends.AdminExtends.GetTop(admin.ID);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="admin">人员</param>
        /// <param name="reponsitory">数据库实例</param>
        public MyManufactureView(AdminTop admin, BvCrmReponsitory reponsitory) : base(reponsitory)
        {
            this.Admin = admin;
        }

        /// <summary>
        /// 获取公司数据集合
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Company> GetIQueryable()
        {
            var linq = base.GetIQueryable().Where(item => item.Type == Enums.CompanyType.Manufacture);

            switch (Admin.JobType)
            {
                case Enums.JobType.Sales:
                case Enums.JobType.Sales_PME:
                case Enums.JobType.TPM:
                    return linq;
                case Enums.JobType.FAE:
                case Enums.JobType.PME:
                    {
                        var mystaffids = new MyStaffsView(this.Admin, Reponsitory).Select(item => item.ID).ToArray();

                        linq = from maps in Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsAdmin>()
                               join manufacture in base.GetIQueryable()
                               on maps.ManufactureID equals manufacture.ID
                               where mystaffids.Contains(maps.AdminID)
                               select manufacture;

                        return linq.Distinct();
                    }
                default:
                    return linq;
            }
        }
    }
}