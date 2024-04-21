using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Views.Origins;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Usually;

namespace Yahv.Finance.Services.Views.Rolls
{
    public class GoldStoresRoll : QueryView<GoldStore, PvFinanceReponsitory>
    {
        #region 事件

        public event SuccessHanlder EnableSuccess;
        public event SuccessHanlder DisableSuccess;
        #endregion

        public GoldStoresRoll()
        {
        }

        protected GoldStoresRoll(PvFinanceReponsitory reponsitory, IQueryable<GoldStore> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<GoldStore> GetIQueryable()
        {
            return new GoldStoresOrigin(this.Reponsitory);
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<GoldStore> iquery = this.IQueryable.Cast<GoldStore>().OrderByDescending(item => item.CreateDate);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_myGoldStore = iquery.ToArray();

            //OwnerIDs
            var ownerIDs = ienum_myGoldStore.Select(item => item.OwnerID);

            //CreatorIDs
            var creatorIDs = ienum_myGoldStore.Select(item => item.CreatorID);

            #region 所属人姓名、创建人姓名

            var ownerAdminsTopView = new AdminsTopView(this.Reponsitory);

            var linq_owner = from owner in ownerAdminsTopView
                             where ownerIDs.Contains(owner.ID)
                             select new
                             {
                                 OwnerID = owner.ID,
                                 OwnerName = owner.RealName,
                             };

            var ienums_owner = linq_owner.ToArray();


            var creatorAdminsTopView = new AdminsTopView(this.Reponsitory);

            var linq_creator = from creator in creatorAdminsTopView
                               where creatorIDs.Contains(creator.ID)
                               select new
                               {
                                   CreatorID = creator.ID,
                                   CreatorName = creator.RealName,
                               };

            var ienums_creator = linq_creator.ToArray();

            #endregion

            var ienums_linq = from goldStore in ienum_myGoldStore
                              join owner in ienums_owner on goldStore.OwnerID equals owner.OwnerID into ienums_owner2
                              from owner in ienums_owner2.DefaultIfEmpty()
                              join creator in ienums_creator on goldStore.CreatorID equals creator.CreatorID into ienums_creator2
                              from creator in ienums_creator2.DefaultIfEmpty()
                              select new GoldStore
                              {
                                  ID = goldStore.ID,
                                  Name = goldStore.Name,
                                  Summary = goldStore.Summary,
                                  IsSpecial = goldStore.IsSpecial,
                                  OwnerID = goldStore.OwnerID,
                                  CreatorID = goldStore.CreatorID,
                                  ModifierID = goldStore.ModifierID,
                                  CreateDate = goldStore.CreateDate,
                                  ModifyDate = goldStore.ModifyDate,
                                  Status = goldStore.Status,

                                  OwnerName = owner != null ? owner.OwnerName : "",
                                  CreatorName = creator != null ? creator.CreatorName : "",
                              };

            var results = ienums_linq.ToArray();

            Func<GoldStore, object> convert = item => new
            {
                ID = item.ID,
                Name = item.Name,
                OwnerName = item.OwnerName,
                CreatorName = item.CreatorName,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                StatusName = item.Status.GetDescription(),
            };

            if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                Func<dynamic, object> convertAgain = item => new
                {

                };

                return results.Select(convert).Select(convertAgain).ToArray();
            }

            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = results.Select(convert).ToArray(),
            };
        }

        /// <summary>
        /// 根据金库名称查询
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GoldStoresRoll SearchByName(string name)
        {
            var linq = from query in this.IQueryable
                       where query.Name.Contains(name)
                       select query;

            var view = new GoldStoresRoll(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据状态查询
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public GoldStoresRoll SearchByStatus(GeneralStatus status)
        {
            var linq = from query in this.IQueryable
                       where query.Status == status
                       select query;

            var view = new GoldStoresRoll(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据是否特殊金库查询
        /// </summary>
        /// <param name="isSpecial"></param>
        /// <returns></returns>
        public GoldStoresRoll SearchByIsSpecial(bool isSpecial)
        {
            var linq = from query in this.IQueryable
                       where query.IsSpecial == isSpecial
                       select query;

            var view = new GoldStoresRoll(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 批量启用
        /// </summary>
        /// <param name="ids"></param>
        public void Enable(string[] ids)
        {
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                reponsitory.Update<Layers.Data.Sqls.PvFinance.GoldStores>(new
                {
                    ModifyDate = DateTime.Now,
                    Status = (int)GeneralStatus.Normal,
                }, item => ids.Contains(item.ID));

                EnableSuccess?.Invoke(ids, new SuccessEventArgs(ids));
            }
        }

        /// <summary>
        /// 批量禁用
        /// </summary>
        /// <param name="ids"></param>
        public void Disable(string[] ids)
        {
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                reponsitory.Update<Layers.Data.Sqls.PvFinance.GoldStores>(new
                {
                    ModifyDate = DateTime.Now,
                    Status = (int)GeneralStatus.Closed,
                }, item => ids.Contains(item.ID));

                this.DisableSuccess?.Invoke(ids, new SuccessEventArgs(ids));
            }
        }
    }
}
