using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Linq.Generic;
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
    public class ExamProductView : Unique1Classics<ExamProduct, BvCrmReponsitory>
    {
        protected ExamProductView()
        {

        }

        protected override IQueryable<ExamProduct> GetIQueryable(Expression<Func<ExamProduct, bool>> expression, params LambdaExpression[] expressions)
        {
            var linq = from product in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.ProductItems>()
                       select new ExamProduct
                       {
                           ID = product.ID,
                           RefUnitPrice = product.RefUnitPrice,
                           RefQuantity = product.RefQuantity,
                           RefUnitQuantity = product.RefUnitQuantity,
                           Status = (ProductStatus)product.Status,
                           StandardID = product.StandardID,
                           FaeAdminID = product.FAEAdmin,
                           PMAdminID = product.PMAdmin,
                       };

            linq = linq.Where(expression);

            foreach (var express in expressions)
            {
                linq = linq.Where(express as Expression<Func<ExamProduct, bool>>);
            }

            return linq;
        }

        protected override IEnumerable<ExamProduct> OnReadShips(ExamProduct[] results)
        {
            var standardProductAlls = new StandardProductAlls(this.Reponsitory).ToArray();
            var admins = this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.AdminTopView>();

            var projects = (from maps in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsProject>()
                            join project in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Projects>() on maps.ProjectID equals project.ID
                            join admin in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.AdminTopView>() on project.AdminID equals admin.ID  //创建人
                            join client in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Clients>() on project.ClientID equals client.ID // 项目所属客户
                            select new
                            {
                                ProductItemID = maps.ProductItemID,
                                ProjectName = project.Name,
                                AdminName = admin.RealName,
                                ClientID = project.ClientID,
                                ClientName = client.Name,
                            }).ToArray();

            return from project in projects
                   join product in results on project.ProductItemID equals product.ID
                   join standard in standardProductAlls on product.StandardID equals standard.ID
                   join pmadmin in admins on product.PMAdminID equals pmadmin.ID into pmadmins
                   from _pmadmin in pmadmins.DefaultIfEmpty()
                   join faeadmin in admins on product.FaeAdminID equals faeadmin.ID into faeadmins
                   from _faeadmin in faeadmins.DefaultIfEmpty()
                   select new ExamProduct
                   {
                       ProjectName = project.ProjectName,
                       AdminName = project.AdminName,
                       ClientID = project.ClientID,
                       ClientName = project.ClientName,
                       ID = product.ID,
                       StandProductName = standard.Name,
                       ManufactureName = standard.Manufacturer.Name,
                       RefUnitPrice = product.RefUnitPrice,
                       RefQuantity = product.RefQuantity,
                       RefUnitQuantity = product.RefUnitQuantity,
                       Status = product.Status,
                       PMAdminID = product.PMAdminID,
                       PMAdminName = _pmadmin == null ? string.Empty : _pmadmin.RealName,
                       FaeAdminID = product.FaeAdminID,
                       FaeAdminName = _faeadmin == null ? string.Empty : _faeadmin.RealName,
                   };
        }
    }


    /// <summary>
    /// 考核产品
    /// </summary>
    public class ExamProduct : IUnique
    {
        public string ID
        {
            get; set;
        }

        public string ProjectName
        {
            get; set;
        }

        public string AdminName
        {
            get; set;
        }

        public string ClientName
        {
            get; set;
        }

        public string ClientID
        {
            get; set;
        }

        public string StandardID { get; set; }

        public string StandProductName
        {
            get; set;
        }

        public string ManufactureName
        {
            get; set;
        }

        /// <summary>
        /// 参考单价
        /// </summary>
        public decimal RefUnitPrice
        {
            get; set;
        }

        /// <summary>
        /// 参考用量
        /// </summary>
        public int RefQuantity
        {
            get; set;
        }

        /// <summary>
        /// 参考单位用量
        /// </summary>
        public int RefUnitQuantity
        {
            get; set;
        }

        /// <summary>
        /// Fae
        /// </summary>
        public string FaeAdminID
        {
            get; set;
        }

        public string FaeAdminName
        {
            get; set;
        }

        /// <summary>
        /// PM
        /// </summary>
        public string PMAdminID
        {
            get; set;
        }

        public string PMAdminName
        {
            get; set;
        }

        /// <summary>
        /// 状态
        /// </summary>
        public ProductStatus Status
        {
            get; set;
        }
    }
}
