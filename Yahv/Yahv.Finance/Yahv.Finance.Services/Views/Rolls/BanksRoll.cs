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

namespace Yahv.Finance.Services.Views.Rolls
{
    public class BanksRoll : QueryView<Bank, PvFinanceReponsitory>
    {
        public BanksRoll()
        {
        }

        protected BanksRoll(PvFinanceReponsitory reponsitory, IQueryable<Bank> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<Bank> GetIQueryable()
        {
            var banksOrigin = new BanksOrigin(this.Reponsitory);

            return banksOrigin;
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<Bank> iquery = this.IQueryable.Cast<Bank>().OrderByDescending(item => item.CreateDate);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_myBank = iquery.ToArray();

            //CreatorIDs
            var creatorIDs = ienum_myBank.Select(item => item.CreatorID);

            #region 创建人姓名

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

            var ienums_linq = from bank in ienum_myBank
                              join creator in ienums_creator on bank.CreatorID equals creator.CreatorID into ienums_creator2
                              from creator in ienums_creator2.DefaultIfEmpty()
                              select new Bank
                              {
                                  ID = bank.ID,
                                  Name = bank.Name,
                                  EnglishName = bank.EnglishName,
                                  CostSummay = bank.CostSummay,
                                  IsAccountCost = bank.IsAccountCost,
                                  AccountCost = bank.AccountCost,
                                  CreatorID = bank.CreatorID,
                                  ModifierID = bank.ModifierID,
                                  CreateDate = bank.CreateDate,
                                  ModifyDate = bank.ModifyDate,
                                  Status = bank.Status,

                                  CreatorName = creator != null ? creator.CreatorName : "",
                              };

            var results = ienums_linq.ToArray();

            Func<Bank, object> convert = item => new
            {
                BankID = item.ID,
                Name = item.Name,
                EnglishName = item.EnglishName,
                IsAccountCostDes = (item.IsAccountCost != null && item.IsAccountCost == true) ? "是" : "否",
                AccountCost = item.AccountCost != null ? Convert.ToString(item.AccountCost) : "",
                CreatorName = item.CreatorName,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                StatusName = item.Status == GeneralStatus.Normal ? "正常" : "停用",
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
        /// 根据银行名称查询
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public BanksRoll SearchByName(string name)
        {
            var linq = from query in this.IQueryable
                       where query.Name.Contains(name)
                       select query;

            var view = new BanksRoll(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据是否有账户管理费查询
        /// </summary>
        /// <param name="isAccountCost"></param>
        /// <returns></returns>
        public BanksRoll SearchByIsAccountCost(bool isAccountCost)
        {
            var linq = from query in this.IQueryable
                       where query.IsAccountCost == isAccountCost
                       select query;

            var view = new BanksRoll(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据状态查询
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public BanksRoll SearchByStatus(GeneralStatus status)
        {
            var linq = from query in this.IQueryable
                       where query.Status == status
                       select query;

            var view = new BanksRoll(this.Reponsitory, linq);
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
                reponsitory.Update<Layers.Data.Sqls.PvFinance.Banks>(new
                {
                    ModifyDate = DateTime.Now,
                    Status = (int)GeneralStatus.Normal,
                }, item => ids.Contains(item.ID));
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
                reponsitory.Update<Layers.Data.Sqls.PvFinance.Banks>(new
                {
                    ModifyDate = DateTime.Now,
                    Status = (int)GeneralStatus.Closed,
                }, item => ids.Contains(item.ID));
            }
        }

    }
}
