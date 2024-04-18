using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Views
{
    public class StaffAlls : UniqueView<Staff, BvCrmReponsitory>
    {
        /// <summary>
        /// 无参构造函数
        /// </summary>
        public StaffAlls()
        {

        }

        /// <summary>
        /// 获取员工数据集合
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Staff> GetIQueryable()
        {
            return from adminTop in Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.AdminTopView>()
                   join structure in Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsDistrict>()
                   on adminTop.ID equals structure.AdminID
                   select new Staff
                   {
                       ID = adminTop.ID,
                       UserName = adminTop.UserName,
                       RealName = adminTop.RealName,
                       FatherID = structure.LeadID
                   };
        }
    }


    public class Staff : IUnique
    {
        public string ID { get; set; }

        public string UserName { get; set; }

        public string RealName { get; set; }

        public string FatherID { get; set; }

    }
}
