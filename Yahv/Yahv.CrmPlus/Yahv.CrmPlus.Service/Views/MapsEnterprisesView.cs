using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models;
using Yahv.Linq;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Underly;
using Layers.Data;

namespace Yahv.CrmPlus.Service.Views
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// 应该放置在 Entity中，为逻辑对象（当前叫做：Rolls）
    /// </remarks>
    public class MyMapsEnterprise : IEntity
    {
        public string ID { get; set; }
        /// <summary>
        /// 商务关系
        /// </summary>
        public BusinessRelationType BusinessRelationType { get; set; }
        public string MainName { internal set; get; }
        public string SubName { internal set; get; }
        public string Creator { get; internal set; }
        public DateTime CreateDate { get; set; }
        public AuditStatus AuditStatus { get; set; }
    }

    public class MapsEnterprisesView : vDepthView<Models.Origins.MyMapsEnterprise, MyMapsEnterprise, PvdCrmReponsitory>
    {
        public MapsEnterprisesView()
        {
        }

        public MapsEnterprisesView(IQueryable<Models.Origins.MyMapsEnterprise> iQueryable) : base(iQueryable)
        {
        }

        protected MapsEnterprisesView(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected MapsEnterprisesView(PvdCrmReponsitory reponsitory, IQueryable<Models.Origins.MyMapsEnterprise> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<Models.Origins.MyMapsEnterprise> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.MapsEnterprise>()
                   select new Models.Origins.MyMapsEnterprise
                   {
                       ID = entity.ID,
                       MainID = entity.MainID,
                       SubID = entity.SubID,
                       AuditStatus = (AuditStatus)entity.Status,
                       BusinessRelationType = (BusinessRelationType)entity.Type,
                       CreateDate = entity.CreateDate,
                       CreatorID = entity.CreatorID,
                   };
        }

        protected override IEnumerable<MyMapsEnterprise> OnMyPage(IQueryable<Models.Origins.MyMapsEnterprise> iquery)
        {
            var data = iquery.ToArray();

            var enterprisesId = data.Select(item => item.MainID).Concat(data.Select(item => item.SubID)).Distinct();

            var linq_enterprise = from enterprise in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Enterprises>()
                                  where enterprisesId.Contains(enterprise.ID)
                                  select new
                                  {
                                      ID = enterprise.ID,
                                      RealName = enterprise.Name,
                                  };
            var enums_enterprises = linq_enterprise.ToArray();

            var adminsId = data.Select(item => item.CreatorID).Distinct();
            var linq_admin = from admin in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.AdminsTopView>()
                             where adminsId.Contains(admin.ID)
                             select new
                             {
                                 ID = admin.ID,
                                 RealName = admin.RealName,
                             };

            var enums_admin = linq_admin.ToArray();

            return from map in data
                   join mainEnterprise in enums_enterprises on map.MainID equals mainEnterprise.ID
                   join subEnterprise in enums_enterprises on map.SubID equals subEnterprise.ID
                   //创建人
                   join admin in enums_admin on map.CreatorID equals admin.ID into creatAdmin
                   from creator in creatAdmin.DefaultIfEmpty()
                   select new MyMapsEnterprise
                   {
                       ID = map.ID,
                       MainName = mainEnterprise.RealName,
                       SubName = subEnterprise.RealName,
                       AuditStatus = (AuditStatus)map.AuditStatus,
                       BusinessRelationType = (BusinessRelationType)map.BusinessRelationType,
                       CreateDate = map.CreateDate,
                       Creator = creator.RealName
                   };
        }

        /// <summary>
        /// 关联关系
        /// </summary>
        /// <param name="mainID">主要ID</param>
        /// <param name="subID">从属ID</param>
        /// <param name="type">关系类型</param>
        static public void Binding(string mainID, string subID, BusinessRelationType type)
        {
            using (var reponsitory = new MapsEnterprisesView().Reponsitory)
            {
                var exist = reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.MapsEnterprise>().
                    Where(x => x.MainID == mainID && x.SubID == subID && x.Type == (int)type);
                if (exist.Any())
                {
                    return;
                }

                string id = PKeySigner.Pick(PKeyType.MapsEnterprise);
                reponsitory.Insert(new Layers.Data.Sqls.PvdCrm.MapsEnterprise()
                {
                    ID = id,
                    Type = (int)type,
                    MainID = mainID,
                    SubID = subID,
                    Status = (int)AuditStatus.Waiting,
                    CreateDate = DateTime.Now
                });
            }
        }
    }
}
