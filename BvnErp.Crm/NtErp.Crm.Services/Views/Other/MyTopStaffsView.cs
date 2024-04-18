//using Layer.Data.Sqls;
//using Needs.Erp.Generic;
//using Needs.Linq;
//using NtErp.Crm.Services.Enums;
//using NtErp.Crm.Services.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Text;
//using System.Threading.Tasks;

//namespace NtErp.Crm.Services.Views
//{
//    public class MyTopStaffsView : UniqueView<Admin, BvCrmReponsitory>
//    {
//        MyTopStaffsView()
//        {

//        }

//        string[] maps;
//        public MyTopStaffsView(IGenericAdmin admin)
//        {
//            var linq = from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsDistrict>()
//                       where entity.LeadID == admin.ID
//                       select entity.AdminID;
//            this.maps = linq.ToArray();
//        }

//        protected override IQueryable<Admin> GetIQueryable()
//        {
//            string[] arry;
//            using (var erp = new BvnErpReponsitory())
//            {
//                string search = "test";
//                var linq = from entity in erp.ReadTable<Layer.Data.Sqls.BvnErp.Admins>()
//                           where entity.UserName.Contains(search)
//                           select entity.ID;
//                arry = linq.Take(1000).ToArray();
//            }

//            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.AdminsProject>().Where(w=>w.AdminID=="1")
//                   where arry.Contains(entity.AdminID)
//                   select new Admin
//                   {
//                       ID = entity.AdminID
//                   };

//        }

//        class admin : iadmin
//        {
//            public string ID { get; set; }

//        }
//        interface iadmin
//        {

//            string ID { get; set; }
//        }
//        class adminProject
//        {
//            #region 属性
//            public string AdminID
//            {
//                get
//                {
//                    return this.AdminID;
//                }
//                set
//                {
//                    this.AdminID = value;
//                }
//            }

//            public JobType JobType
//            {
//                get
//                {
//                    return this.JobType;
//                }

//                set
//                {
//                    this.JobType = value;
//                }
//            }

//            public DateTime CreateDate
//            {
//                get
//                {
//                    return this.CreateDate;
//                }

//                set
//                {
//                    this.CreateDate = value;
//                }
//            }

//            public DateTime UpdateDate
//            {
//                get
//                {
//                    return this.UpdateDate;
//                }

//                set
//                {
//                    this.UpdateDate = value;
//                }
//            }

//            public string Summary
//            {
//                get
//                {
//                    return this.Summary;
//                }

//                set
//                {
//                    this.Summary = value;
//                }
//            }
//            #endregion
//        }

//        interface iadminProject
//        {
//            string AdminID { get; set; }

//            JobType JobType { get; set; }

//            DateTime CreateDate { get; set; }

//            DateTime UpdateDate { get; set; }

//            string Summary { get; set; }
//        }

//        class TopAdmin : iadmin, iadminProject
//        {
//            admin admin;
//            adminProject adminProject;
//            #region 属性
//            public string ID
//            {
//                get
//                {
//                    return this.admin.ID;
//                }
//                set
//                {
//                    this.admin.ID = value;
//                }
//            }
//            public string AdminID
//            {
//                get
//                {
//                    return this.AdminID;
//                }
//                set
//                {
//                    this.AdminID = value;
//                }
//            }

//            public JobType JobType
//            {
//                get
//                {
//                    return this.JobType;
//                }

//                set
//                {
//                    this.JobType = value;
//                }
//            }

//            public DateTime CreateDate
//            {
//                get
//                {
//                    return this.CreateDate;
//                }

//                set
//                {
//                    this.CreateDate = value;
//                }
//            }

//            public DateTime UpdateDate
//            {
//                get
//                {
//                    return this.UpdateDate;
//                }

//                set
//                {
//                    this.UpdateDate = value;
//                }
//            }

//            public string Summary
//            {
//                get
//                {
//                    return this.Summary;
//                }

//                set
//                {
//                    this.Summary = value;
//                }
//            }
//            #endregion
//        }


//        public void Where(Expression<Func<Layer.Data.Sqls.BvCrm.AdminsProject, bool>> predicate)
//        {

//        }
//    }
//}