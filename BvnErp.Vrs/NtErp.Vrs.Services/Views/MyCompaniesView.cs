using Needs.Erp.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Vrs.Services.Views
{
    public class MapsAdminVender
    {
        public string AdminID { set; get; }
        public string CompanyID { set; get; }
    }
    public class MyCompaniesView : Needs.Linq.QueryView<Views.MapsAdminVender, Layer.Data.Sqls.BvnVrsReponsitory>
    {
        IGenericAdmin admin;
        public MyCompaniesView(IGenericAdmin admin)
        {
            this.admin = admin;
        }

        protected override IQueryable<MapsAdminVender> GetIQueryable()
        {
            if (this.admin == null)
            {
                var linq = from maps in Reponsitory.ReadTable<Layer.Data.Sqls.BvnVrs.MapsAdminCompany>()
                           select new MapsAdminVender
                           {
                               AdminID = maps.AdminID,
                               CompanyID = maps.CompanyID
                           };
                return linq;
            }
            else
            {
                var linq = from maps in Reponsitory.ReadTable<Layer.Data.Sqls.BvnVrs.MapsAdminCompany>()
                           where maps.AdminID == this.admin.ID
                           select new MapsAdminVender
                           {
                               AdminID = maps.AdminID,
                               CompanyID = maps.CompanyID
                           };
                return linq;
            }
        }
        /// <summary>
        /// 为当前用户增加Vender
        /// </summary>
        /// <param name="id"></param>
        public void Bind(string id, string adminid)
        {
            using (var repository = new Layer.Data.Sqls.BvnVrsReponsitory())
            {
                repository.Delete<Layer.Data.Sqls.BvnVrs.MapsAdminCompany>(item => item.CompanyID == id);
                repository.Insert(new Layer.Data.Sqls.BvnVrs.MapsAdminCompany
                {
                    AdminID = adminid,
                    CompanyID = id
                });
            }
        }

        /// <summary>
        /// 为当前用户移除Vender
        /// </summary>
        /// <param name="id"></param>
        public void UnBind(string id, string adminid)
        {
            using (var repository = new Layer.Data.Sqls.BvnVrsReponsitory())
            {
                repository.Delete<Layer.Data.Sqls.BvnVrs.MapsAdminCompany>(item => item.AdminID == adminid && item.CompanyID == id);
            }
        }

    }
}
