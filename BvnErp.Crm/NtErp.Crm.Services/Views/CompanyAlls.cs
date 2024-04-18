using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layer.Data.Sqls;
using Needs.Erp.Generic;
using Needs.Linq;
using Needs.Utils.Converters;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Models;

namespace NtErp.Crm.Services.Views
{
    public class CompanyAlls : UniqueView<Company, BvCrmReponsitory>, Needs.Underly.IFkoView<Company>
    {
        /// <summary>
        /// 无参构造函数
        /// </summary>
        public CompanyAlls()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库实体</param>
        internal CompanyAlls(BvCrmReponsitory reponsitory) : base(reponsitory)
        {
        }

        /// <summary>
        /// 公司数据集合
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Company> GetIQueryable()
        {
            return from company in Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Companies>()
                   where company.Status != (int)Enums.Status.Delete 
                   select new Company
                   {
                       ID = company.ID,
                       Type = (CompanyType)company.Type,
                       Name = company.Name,
                       Code = company.Code,
                       Status = (Enums.Status)company.Status,
                       CreateDate = company.CreateDate,
                       UpdateDate = company.UpdateDate,
                       Summary = company.Summary
                   };
        }


        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="AdminID">人员</param>
        /// <param name="ManufactureID">品牌</param>
        public void Binding(string AdminID, string[] manufactures)
        {
            var admin = new AdminTopView(this.Reponsitory)[AdminID];
            this.DeleteBinding(admin);
            if (string.IsNullOrWhiteSpace(manufactures.First()))
            {
                return;
            }
            foreach (string manufactureID in manufactures)
            {
                var manufacture = this[manufactureID];
                this.Reponsitory.Insert(new Layer.Data.Sqls.BvCrm.MapsAdmin
                {
                    ID = string.Concat(admin.ID, manufacture.ID).MD5(),
                    AdminID = admin.ID,
                    ManufactureID = manufacture.ID,
                });
            }
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="AdminID">人员ID</param>
        public void DeleteBinding(AdminTop Admin)
        {
            this.Reponsitory.Delete<Layer.Data.Sqls.BvCrm.MapsAdmin>(item => item.AdminID == Admin.ID);
        }
    }
}
