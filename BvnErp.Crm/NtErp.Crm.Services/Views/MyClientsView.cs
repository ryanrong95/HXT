using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Erp.Generic;
using NtErp.Crm.Services.Models;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Models.Generic;
using System.Linq.Expressions;

namespace NtErp.Crm.Services.Views
{
    public class MyClientsView : Generic.ClientClassicAlls
    {
        //人员对象
        AdminTop Admin;

        /// <summary>
        /// 无参构造函数
        /// </summary>
        private MyClientsView()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="admin">人员对象</param>
        public MyClientsView(IGenericAdmin admin)
        {
            this.Admin = Extends.AdminExtends.GetTop(admin.ID);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="admin">人员对象</param>
        public MyClientsView(AdminTop admin, BvCrmReponsitory reponsitory) : base(reponsitory)
        {
            this.Admin = admin;
        }

        /// <summary>
        /// 根据查询条件获取客户集合
        /// </summary>
        /// <param name="expression">查询条件</param>
        /// <param name="expressions">lambda表达式</param>
        /// <returns></returns>
        protected override IQueryable<ClientDossier> GetIQueryable(Expression<Func<ClientDossier, bool>> expression, params LambdaExpression[] expressions)
        {
            var clientdossiers = from ClientDossier in base.GetIQueryable(expression, expressions)
                                 where ClientDossier.Client.IsSafe == IsProtected.Yes
                                 select ClientDossier;

            if (Admin.JobType == JobType.Sales)
            {
                //获取所有员工
                var mystaffs = new MyStaffsView(this.Admin, Reponsitory).Select(item => item.ID).ToArray();

                clientdossiers = from maps in Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsClient>()
                                 join ClientDossier in clientdossiers on maps.ClientID equals ClientDossier.Client.ID
                                 where mystaffs.Contains(maps.AdminID)
                                 select ClientDossier;
            }
            if (Admin.JobType == JobType.FAE)
            {
                //获取所有员工
                var mystaffs = new MyStaffsView(this.Admin, Reponsitory).Select(item => item.ID).ToArray();

                clientdossiers = from mapadmin in Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsAdmin>()
                                 join maps in Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsClient>() on mapadmin.ManufactureID equals maps.ManufacturerID
                                 join ClientDossier in clientdossiers on maps.ClientID equals ClientDossier.Client.ID
                                 where mystaffs.Contains(mapadmin.AdminID)
                                 select ClientDossier;
            }

            return clientdossiers.Distinct().OrderByDescending(item => item.Client.UpdateDate);

            #region 需求变更前
            //if (Admin.JobType != JobType.TPM)
            //{
            //    //获取所有员工
            //    var mystaffs = new MyStaffsView(this.Admin, Reponsitory).Select(item => item.ID).ToArray();

            //    var linq1 = (from ClientDossier in clientdossiers
            //                 join maps in Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsClient>() on ClientDossier.Client.ID equals maps.ClientID
            //                 where mystaffs.Contains(maps.AdminID)
            //                 select ClientDossier).Distinct();

            //    if (Admin.JobType != JobType.Sales)
            //    {
            //        var linq2 = (from ClientDossier in clientdossiers
            //                     join maps in Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsClient>() on ClientDossier.Client.ID equals maps.ClientID
            //                     join manumap in Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsAdmin>() on maps.ManufacturerID equals manumap.ManufactureID
            //                     where mystaffs.Contains(manumap.AdminID)
            //                     select ClientDossier).Distinct();

            //        linq1 = linq1.Union(linq2);
            //    }

            //    return linq1;
            //}
            //else
            //{
            //    return clientdossiers;
            //}
            #endregion
        }


        /// <summary>
        /// 校验是否是当前用户
        /// </summary>
        /// <param name="ClientID"></param>
        /// <returns></returns>
        public bool IsMyclient(string ClientID)
        {
            if (this.Admin.JobType != JobType.PME && this.Admin.JobType != JobType.Sales_PME)
            {
                return true;
            }
            //获取所有员工
            var mystaffs = new MyStaffsView(this.Admin, Reponsitory).Select(item => item.ID).ToArray();
            //是否是当前客户的所有人
            var maps = Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsClient>().Where(item => item.ClientID == ClientID);

            if (maps.Count(item => mystaffs.Contains(item.AdminID)) > 0)
            {
                return true;
            }
            //是否管理当前客户所关联的品牌
            var manus = from mapsadmin in Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsAdmin>()
                        join map in maps on mapsadmin.ManufactureID equals map.ManufacturerID
                        where mystaffs.Contains(mapsadmin.AdminID)
                        select mapsadmin;

            if(manus.Count() > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expressions"></param>
        /// <returns></returns>
        public override string[] MapCondition(Expression<Func<Layer.Data.Sqls.BvCrm.MapsClient, bool>> expression,string[] clientids)
        {
            return base.MapCondition(expression, clientids);
        }
    }
}
