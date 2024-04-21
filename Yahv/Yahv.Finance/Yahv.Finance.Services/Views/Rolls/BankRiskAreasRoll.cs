using Layers.Data.Sqls;
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
    public class BankRiskAreasRoll : QueryView<BankRiskArea, PvFinanceReponsitory>
    {
        public BankRiskAreasRoll()
        {
        }

        protected BankRiskAreasRoll(PvFinanceReponsitory reponsitory, IQueryable<BankRiskArea> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<BankRiskArea> GetIQueryable()
        {
            var bankRiskAreasOrigin = new BankRiskAreasOrigin(this.Reponsitory);

            return bankRiskAreasOrigin.Where(t => t.Status == GeneralStatus.Normal);
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<BankRiskArea> iquery = this.IQueryable.Cast<BankRiskArea>().OrderByDescending(item => item.CreateDate);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_myBankRiskArea = iquery.ToArray();

            var ienums_linq = from bankRiskArea in ienum_myBankRiskArea
                              select new BankRiskArea
                              {
                                  ID = bankRiskArea.ID,
                                  BankID = bankRiskArea.BankID,
                                  District = bankRiskArea.District,
                                  CreatorID = bankRiskArea.CreatorID,
                                  ModifierID = bankRiskArea.ModifierID,
                                  CreateDate = bankRiskArea.CreateDate,
                                  ModifyDate = bankRiskArea.ModifyDate,
                                  Status = bankRiskArea.Status,
                              };

            var results = ienums_linq.ToArray();

            Func<BankRiskArea, object> convert = item => new
            {
                BankRiskAreaID = item.ID,
                DistrictName = ((Origin)Enum.Parse(typeof(Origin), item.District)).GetDescription(),
            };

            if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                Func<dynamic, object> convertAgain = item => new
                {
                    BankRiskAreaID = item.BankRiskAreaID,
                    DistrictName = item.DistrictName,
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
        /// 根据 BankID 查询
        /// </summary>
        /// <param name="bankID"></param>
        /// <returns></returns>
        public BankRiskAreasRoll SearchByBankID(string bankID)
        {
            var linq = from query in this.IQueryable
                       where query.BankID == bankID
                       select query;

            var view = new BankRiskAreasRoll(this.Reponsitory, linq);
            return view;
        }

    }
}
