using System;
using System.Collections.Generic;
using System.Linq;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Views.Origins;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Finance.Services.Views.Rolls
{
    public class ChargeApplyItemsRoll : QueryView<ChargeApplyItem, PvFinanceReponsitory>
    {
        public ChargeApplyItemsRoll()
        {
        }

        public ChargeApplyItemsRoll(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected ChargeApplyItemsRoll(PvFinanceReponsitory reponsitory, IQueryable<ChargeApplyItem> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<ChargeApplyItem> GetIQueryable()
        {
            return new ChargeApplyItemsOrigin(this.Reponsitory);
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<ChargeApplyItem> iquery = this.IQueryable.Cast<ChargeApplyItem>().OrderByDescending(item => item.CreateDate);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_myChargeApplyItem = iquery.ToArray();

            //CostCatalogs
            var costCatalogs = ienum_myChargeApplyItem.Select(t => t.AccountCatalogID).Distinct().ToArray();

            #region 付款类型

            var accountCatalogsOrigin = new AccountCatalogsOrigin(this.Reponsitory);

            var linq_accountCatalog = from accountCatalog in accountCatalogsOrigin
                                      where costCatalogs.Contains(accountCatalog.ID)
                                      select new
                                      {
                                          AccountCatalogID = accountCatalog.ID,
                                          AccountCatalogName = accountCatalog.Name,
                                      };

            var ienums_accountCatalog = linq_accountCatalog.ToArray();

            #endregion

            var ienums_linq = from ChargeApplyItem in ienum_myChargeApplyItem
                              join accountCatalog in ienums_accountCatalog on ChargeApplyItem.AccountCatalogID equals accountCatalog.AccountCatalogID into ienums_accountCatalog2
                              from accountCatalog in ienums_accountCatalog2.DefaultIfEmpty()
                              select new ChargeApplyItem
                              {
                                  ID = ChargeApplyItem.ID,
                                  ApplyID = ChargeApplyItem.ApplyID,
                                  IsPaid = ChargeApplyItem.IsPaid,
                                  ExpectedTime = ChargeApplyItem.ExpectedTime,
                                  AccountCatalogID = ChargeApplyItem.AccountCatalogID,
                                  Price = ChargeApplyItem.Price,
                                  Summary = ChargeApplyItem.Summary,
                                  FlowID = ChargeApplyItem.FlowID,
                                  CallBackUrl = ChargeApplyItem.CallBackUrl,
                                  CallBackID = ChargeApplyItem.CallBackID,
                                  CreateDate = ChargeApplyItem.CreateDate,
                                  ModifyDate = ChargeApplyItem.ModifyDate,
                                  Status = ChargeApplyItem.Status,

                                  AccountCatalogName = accountCatalog != null ? accountCatalog.AccountCatalogName : "",
                              };

            var results = ienums_linq.ToArray();

            Func<ChargeApplyItem, object> convert = item => new
            {
                ChargeApplyItemID = item.ID,
                ChargeApplyID = item.ApplyID,
                AccountCatalogID = item.AccountCatalogID,
                AccountCatalogName = item.AccountCatalogName,
                Price = item.Price,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
            };

            if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                Func<dynamic, object> convertAgain = item => new
                {
                    ChargeApplyItemID = item.ChargeApplyItemID,
                    ChargeApplyID = item.ChargeApplyID,
                    AccountCatalogID = item.AccountCatalogID,
                    AccountCatalogName = item.AccountCatalogName,
                    Price = item.Price,
                    CreateDate = item.CreateDate,
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
        /// 根据 CostApplyID 查询
        /// </summary>
        /// <param name="costApplyID"></param>
        /// <returns></returns>
        public ChargeApplyItemsRoll SearchByCostApplyID(string chargeApplyID)
        {
            var linq = from query in this.IQueryable
                       where query.ApplyID == chargeApplyID
                       select query;

            var view = new ChargeApplyItemsRoll(this.Reponsitory, linq);
            return view;
        }

        public void Refresh(string applyID, ChargeApplyItem[] items)
        {
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create(false))
            {
                reponsitory.Delete<Layers.Data.Sqls.PvFinance.ChargeApplyItems>(item => item.ApplyID == applyID);

                var now = DateTime.Now;
                Layers.Data.Sqls.PvFinance.ChargeApplyItems[] itemsdata = items.Select(t => new Layers.Data.Sqls.PvFinance.ChargeApplyItems
                {
                    ID = t.ID,
                    ApplyID = t.ApplyID,
                    IsPaid = t.IsPaid,
                    ExpectedTime = t.ExpectedTime,
                    AccountCatalogID = t.AccountCatalogID,
                    Price = t.Price,
                    Summary = t.Summary,
                    FlowID = t.FlowID,
                    CallBackUrl = t.CallBackUrl,
                    CallBackID = t.CallBackID,
                    CreateDate = now,
                    ModifyDate = now,
                    Status = (int)GeneralStatus.Normal,
                }).ToArray();

                reponsitory.Insert(itemsdata);

                reponsitory.Submit();
            }
        }

        /// <summary>
        /// 添加或者更新
        /// </summary>
        /// <param name="applyID"></param>
        /// <param name="items"></param>
        public void InsertOrUpdate(string applyID, IEnumerable<ChargeApplyItem> items)
        {
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create(false))
            {
                var itemsdata = items.Where(item => string.IsNullOrEmpty(item.ID)).Select(item => new Layers.Data.Sqls.PvFinance.ChargeApplyItems
                {
                    ID = item.ID ?? PKeySigner.Pick(PKeyType.ChargeApplyItem),
                    ApplyID = applyID,
                    IsPaid = item.IsPaid,
                    ExpectedTime = item.ExpectedTime,
                    AccountCatalogID = item.AccountCatalogID,
                    Price = item.Price,
                    Summary = item.Summary ?? null,
                    FlowID = item.FlowID,
                    CallBackUrl = item.CallBackUrl,
                    CallBackID = item.CallBackID,
                    CreateDate = DateTime.Now,
                    ModifyDate = DateTime.Now,
                    Status = (int)item.Status,
                }).ToArray();

                var itemsUpdate = items.Where(item => !string.IsNullOrEmpty(item.ID));
                if (itemsUpdate != null && itemsUpdate.Any())
                {
                    foreach (var item in itemsUpdate)
                    {
                        reponsitory.Update<Layers.Data.Sqls.PvFinance.ChargeApplyItems>(new
                        {
                            ApplyID = applyID,
                            IsPaid = item.IsPaid,
                            AccountCatalogID = item.AccountCatalogID,
                            Price = item.Price,
                            Summary = item.Summary,
                            FlowID = item.FlowID,
                            ModifyDate = DateTime.Now,
                            Status = (int)item.Status,
                        }, c => c.ID == item.ID);
                    }
                }

                reponsitory.Insert(itemsdata);
                reponsitory.Submit();
            }
        }

        /// <summary>
        /// 删除申请项
        /// </summary>
        /// <param name="id"></param>
        public void Abandon(params string[] id)
        {
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                string flowId = null;
                reponsitory.Update<Layers.Data.Sqls.PvFinance.ChargeApplyItems>(new
                {
                    Status = (int)ApplyItemStauts.Paying,
                    FlowID = flowId,
                }, c => id.Contains(c.ID));
            }
        }
    }
}