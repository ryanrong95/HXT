using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layer.Data.Sqls;
using Needs.Erp.Generic;
using Needs.Linq;
using Needs.Utils.Converters;
using Newtonsoft.Json.Linq;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Models;

namespace NtErp.Crm.Services.Views
{
    public class DistrictAlls : UniqueView<District, BvCrmReponsitory>, Needs.Underly.IFkoView<District>
    {
        /// <summary>
        /// 无参构造函数
        /// </summary>
        public DistrictAlls()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库实体</param>
        internal DistrictAlls(BvCrmReponsitory reponsitory) : base(reponsitory)
        {

        }

        /// <summary>
        /// 获取区域数据集合
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<District> GetIQueryable()
        {
            return from district in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Districts>()
                   select new District
                   {
                       ID = district.ID,
                       FatherID = district.FatherID,
                       Name = district.Name,
                       Level = district.Level,
                       CreateDate = district.CreateDate,
                       UpdateDate = district.UpdateDate,
                       Status = (Enums.Status)district.Status,
                   };
        }

        /// <summary>
        /// 区域人员绑定
        /// </summary>
        /// <param name="districtId">区域id</param>
        /// <param name="leader">组长</param>
        /// <param name="admin">组员</param>
        public void Binding(string districtId, AdminTop leader, AdminTop admin, int Type)
        {
            var district = this[districtId];
            using (BvCrmReponsitory reponsitory = new BvCrmReponsitory())
            {
                reponsitory.Insert(new Layer.Data.Sqls.BvCrm.MapsDistrict
                {
                    ID = string.Concat(district.ID, leader?.ID, admin.ID).MD5(),
                    DistrictID = district.ID,
                    Type = Type,
                    LeadID = leader.ID,
                    AdminID = admin.ID,
                });
            }
        }

        /// <summary>
        /// 删除该区域人员绑定
        /// </summary>
        /// <param name="districtId">区域id</param>
        public void DeleteBinding(string districtId, int Type)
        {
            var district = this[districtId];
            using (BvCrmReponsitory reponsitory = new BvCrmReponsitory())
            {
                reponsitory.Delete<Layer.Data.Sqls.BvCrm.MapsDistrict>(item => item.DistrictID == district.ID && item.Type == Type);
            }
        }
    }


    /// <summary>
    /// 区域树
    /// </summary>
    public class DistrictTree
    {
        //所有区域
        private District[] districts;
        private Layer.Data.Sqls.BvCrm.MapsDistrict[] maps;
        const string root = "1000";
        private string Id;

        //区域树集合
        public JArray Tree
        {
            get
            {
                return this.GetTree();
            }
        }

        /// <summary>
        /// 返回员工的id
        /// </summary>
        public string[] AdminDescendants
        {
            get
            {
                return this.GetDistrict();
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public DistrictTree(string adminid = null)
        {
            this.Id = adminid;
            using (BvCrmReponsitory reponsitory = new BvCrmReponsitory())
            {
                districts = new DistrictAlls(reponsitory).ToArray();
                maps = reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsDistrict>().ToArray();
            }
        }

        /// <summary>
        /// 获取区域树形集合
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private JArray GetTree(string id = null)
        {
            Func<District, bool> exp = t => t.FatherID == id;
            if (id == null)
            {
                exp = t => t.FatherID == root;
            }
            JArray arry = new JArray();
            var list = this.districts.Where(exp).ToArray();
            foreach (var item in list)
            {
                JObject obj = new JObject();
                obj.Add("id", item.ID);
                obj.Add("text", item.Name);

                var children = GetTree(item.ID);
                if (children != null && children.Count > 0)
                {
                    obj.Add("children", children);
                }
                arry.Add(obj);
            }

            return arry;
        }

        /// <summary>
        /// 根据角色判断区域类型
        /// </summary>
        /// <returns></returns>
        private string[] GetDistrict()
        {
            AdminTop admin = Extends.AdminExtends.GetTop(this.Id);
            if (admin == null)
            {
                return null;
            }
            string[] adminids = new string[0];
            if(admin.JobType == JobType.Sales || admin.JobType == JobType.Sales_PME)
            {
                adminids = adminids.Concat(GetDescendants(null, new string[] { this.Id }, DistrictType.Sales)).ToArray();
            }
            if(admin.JobType == JobType.FAE || admin.JobType == JobType.PME || admin.JobType == JobType.Sales_PME)
            {
                adminids = adminids.Concat(GetDescendants(null, new string[] { this.Id }, DistrictType.Market)).ToArray();
            }
            return adminids;
        }

        /// <summary>
        /// 迭代获取子区域的员工
        /// </summary>
        /// <param name="districts">区域集合</param>
        /// <param name="ids">员工集合</param>
        private string[] GetDescendants(District[] Mydistricts, string[] adminids, DistrictType type)
        {
            if (Mydistricts == null)
            {
                Mydistricts = (from district in districts
                               join map in maps on district.ID equals map.DistrictID
                               where adminids.Contains(map.LeadID)
                               where map.Type == (int)type
                               select district).Distinct().ToArray();
            }

            var distictIds = Mydistricts.Select(item => item.ID).ToArray();

            var admin_linq = this.maps.Where(item => distictIds.Contains(item.DistrictID) && item.Type == (int)type).Select(item => item.AdminID);

            adminids = adminids.Concat(admin_linq).ToArray();

            var district_linq = districts.Where(item => distictIds.Contains(item.FatherID)).ToArray();

            if (district_linq.Count() > 0)
            {
                adminids = adminids.Concat(GetDescendants(district_linq, adminids, type)).ToArray();
            }

            return adminids.Distinct().ToArray();
        }
    }
}
