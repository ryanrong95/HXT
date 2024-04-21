using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Views.Origins;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Finance.Services.Views.Rolls
{
    public class CostApplyItemsRoll : QueryView<CostApplyItem, PvFinanceReponsitory>
    {
        public CostApplyItemsRoll()
        {
        }

        protected CostApplyItemsRoll(PvFinanceReponsitory reponsitory, IQueryable<CostApplyItem> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<CostApplyItem> GetIQueryable()
        {
            return new CostApplyItemsOrigin(this.Reponsitory);
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<CostApplyItem> iquery = this.IQueryable.Cast<CostApplyItem>().OrderByDescending(item => item.CreateDate);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_myCostApplyItem = iquery.ToArray();

            //CostCatalogs
            var accountCatalogs = ienum_myCostApplyItem.Select(t => t.AccountCatalogID).Distinct().ToArray();

            #region 付款类型

            var accountCatalogsOrigin = new AccountCatalogsOrigin(this.Reponsitory);

            var linq_accountCatalog = from accountCatalog in accountCatalogsOrigin
                                      where accountCatalogs.Contains(accountCatalog.ID)
                                      select new
                                      {
                                          AccountCatalogID = accountCatalog.ID,
                                          AccountCatalogName = accountCatalog.Name,
                                      };

            var ienums_accountCatalog = linq_accountCatalog.ToArray();

            #endregion

            var ienums_linq = from costApplyItem in ienum_myCostApplyItem
                              join accountCatalog in ienums_accountCatalog on costApplyItem.AccountCatalogID equals accountCatalog.AccountCatalogID into ienums_accountCatalog2
                              from accountCatalog in ienums_accountCatalog2.DefaultIfEmpty()
                              select new CostApplyItem
                              {
                                  ID = costApplyItem.ID,
                                  ApplyID = costApplyItem.ApplyID,
                                  IsPaid = costApplyItem.IsPaid,
                                  ExpectedTime = costApplyItem.ExpectedTime,
                                  AccountCatalogID = costApplyItem.AccountCatalogID,
                                  Price = costApplyItem.Price,
                                  Summary = costApplyItem.Summary,
                                  FlowID = costApplyItem.FlowID,
                                  CallBackUrl = costApplyItem.CallBackUrl,
                                  CallBackID = costApplyItem.CallBackID,
                                  CreateDate = costApplyItem.CreateDate,
                                  ModifyDate = costApplyItem.ModifyDate,
                                  Status = costApplyItem.Status,

                                  AccountCatalogName = accountCatalog != null ? accountCatalog.AccountCatalogName : "",
                              };

            var results = ienums_linq.ToArray();

            Func<CostApplyItem, object> convert = item => new
            {
                CostApplyItemID = item.ID,
                CostApplyID = item.ApplyID,
                AccountCatalogID = item.AccountCatalogID,
                AccountCatalogName = item.AccountCatalogName,
                Price = item.Price,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
            };

            if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                Func<dynamic, object> convertAgain = item => new
                {
                    CostApplyItemID = item.CostApplyItemID,
                    CostApplyID = item.CostApplyID,
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
        public CostApplyItemsRoll SearchByCostApplyID(string costApplyID)
        {
            var linq = from query in this.IQueryable
                       where query.ApplyID == costApplyID
                       select query;

            var view = new CostApplyItemsRoll(this.Reponsitory, linq);
            return view;
        }

        public void Refresh(string costApplyID, CostApplyItem[] items)
        {
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create(false))
            {
                reponsitory.Delete<Layers.Data.Sqls.PvFinance.CostApplyItems>(item => item.ApplyID == costApplyID);

                Layers.Data.Sqls.PvFinance.CostApplyItems[] itemsdata = items.Select(t => new Layers.Data.Sqls.PvFinance.CostApplyItems
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
                    CreateDate = DateTime.Now,
                    ModifyDate = DateTime.Now,
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
        public void InsertOrUpdate(string applyID, IEnumerable<CostApplyItem> items)
        {
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create(false))
            {
                var itemsdata = items.Where(item => string.IsNullOrEmpty(item.ID)).Select(item => new Layers.Data.Sqls.PvFinance.CostApplyItems
                {
                    ID = item.ID ?? PKeySigner.Pick(PKeyType.CostApplyItem),
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
                        reponsitory.Update<Layers.Data.Sqls.PvFinance.CostApplyItems>(new
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
                reponsitory.Update<Layers.Data.Sqls.PvFinance.CostApplyItems>(new
                {
                    Status = (int)ApplyItemStauts.Paying,
                    FlowID = flowId,
                }, c => id.Contains(c.ID));
            }
        }
    }
}
