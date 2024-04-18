using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class ChargeStandardListForMaintainView : QueryView<ChargeStandardListForMaintainView.ChargeStandardModel, ScCustomsReponsitory>
    {
        public ChargeStandardListForMaintainView()
        {
        }

        protected ChargeStandardListForMaintainView(ScCustomsReponsitory reponsitory, IQueryable<ChargeStandardModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<ChargeStandardModel> GetIQueryable()
        {
            var decChargeStandards = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecChargeStandards>();

            var iQuery = from decChargeStandard in decChargeStandards
                         where decChargeStandard.Status == (int)Enums.Status.Normal
                         select new ChargeStandardModel
                         {
                             ID = decChargeStandard.ID,
                             FatherID = decChargeStandard.FatherID,
                             EnumValue = decChargeStandard.EnumValue,
                             SerialNo = decChargeStandard.SerialNo,
                             Type = decChargeStandard.Type != null ? (Enums.ChargeStandardType?)decChargeStandard.Type : null,
                             IsMenuLeaf = decChargeStandard.IsMenuLeaf,
                             Name = decChargeStandard.Name,
                             Unit1 = decChargeStandard.Unit1,
                             FixedCount1 = decChargeStandard.FixedCount1,
                             Unit2 = decChargeStandard.Unit2,
                             FixedCount2 = decChargeStandard.FixedCount2,
                             Price = decChargeStandard.Price,
                             Currency = decChargeStandard.Currency,
                             Remark1 = decChargeStandard.Remark1,
                             Remark2 = decChargeStandard.Remark2,
                             Status = (Enums.Status)decChargeStandard.Status,
                             CreateDate = decChargeStandard.CreateDate,
                             UpdateDate = decChargeStandard.UpdateDate,
                             Summary = decChargeStandard.Summary,
                         };
            return iQuery;
        }

        /// <summary>
        /// 转为一个 obj
        /// </summary>
        /// <returns></returns>
        public ChargeStandardModel ToOneObject()
        {
            IQueryable<ChargeStandardModel> iquery = this.IQueryable.Cast<ChargeStandardModel>();

            //获取数据
            var ienum_myDecChargeStandards = iquery.ToList();

            //币种中文赋值
            for (int i = 0; i < ienum_myDecChargeStandards.Count; i++)
            {
                if (!string.IsNullOrEmpty(ienum_myDecChargeStandards[i].Currency))
                {
                    Enums.ClientCurrency currency = (Enums.ClientCurrency)(Enum.Parse(typeof(Enums.ClientCurrency), ienum_myDecChargeStandards[i].Currency));
                    ienum_myDecChargeStandards[i].CurrencyCN = currency.GetDescription();
                }
            }

            //给原先顶部的节点赋值 FatherID
            string tempID = Guid.NewGuid().ToString("N");
            for (int i = 0; i < ienum_myDecChargeStandards.Count; i++)
            {
                if (string.IsNullOrEmpty(ienum_myDecChargeStandards[i].FatherID))
                {
                    ienum_myDecChargeStandards[i].FatherID = tempID;
                }
            }

            //不断向下找到子节点, 直到找不到
            var tempTop = new ChargeStandardModel { ID = tempID, };
            tempTop.SetAll(ienum_myDecChargeStandards.ToArray());
            tempTop.SetChildren(1);

            return tempTop;
        }

        /// <summary>
        /// 平铺
        /// </summary>
        /// <returns></returns>
        public ChargeStandardModel[] ToTile()
        {
            List<ChargeStandardModel> tiledModels = new List<ChargeStandardModel>();

            var topStandard = this.ToOneObject();

            tiledModels.AddInto(topStandard);

            return tiledModels.ToArray();
        }



        public class ChargeStandardModel
        {
            #region 属性

            public string ID { get; set; }

            public string FatherID { get; set; }

            public int? EnumValue { get; set; }

            public int? SerialNo { get; set; }

            public Enums.ChargeStandardType? Type { get; set; }

            public bool? IsMenuLeaf { get; set; }

            public string Name { get; set; }

            public string Unit1 { get; set; }

            public decimal? FixedCount1 { get; set; }

            public string Unit2 { get; set; }

            public decimal? FixedCount2 { get; set; }

            public decimal? Price { get; set; }

            public string Currency { get; set; }

            public string CurrencyCN { get; set; }

            public string Remark1 { get; set; }

            public string Remark2 { get; set; }

            public Enums.Status Status { get; set; }

            public DateTime CreateDate { get; set; }

            public DateTime UpdateDate { get; set; }

            public string Summary { get; set; }

            #endregion

            public ChargeStandardModel[] All { get; set; }

            public ChargeStandardModel[] Children { get; set; }

            public int Level { get; set; }

            public void SetAll(ChargeStandardModel[] all)
            {
                this.All = all;
            }

            public void SetLevel(int level)
            {
                this.Level = level;
            }

            public void SetChildren(int level)
            {
                if (this.All != null && this.All.Where(t => t.FatherID == this.ID).Any())
                {
                    this.Children = this.All
                        .Where(t => t.FatherID == this.ID)
                        .Select(selector)
                        .OrderBy(t => t.SerialNo).ToArray();

                    foreach (var child in this.Children)
                    {
                        child.SetAll(this.All);
                        child.SetLevel(level);
                        child.SetChildren(level + 1);
                        child.SetAll(null);
                    }
                }
                else
                {
                    this.Children = null;
                }
            }

            public ChargeStandardModel[] GetChildren()
            {
                return this.Children;
            }

            public static Func<ChargeStandardModel, ChargeStandardModel> selector = item => new ChargeStandardModel
            {
                ID = item.ID,
                FatherID = item.FatherID,
                EnumValue = item.EnumValue,
                SerialNo = item.SerialNo,
                Type = item.Type,
                IsMenuLeaf = item.IsMenuLeaf,
                Name = item.Name,
                Unit1 = item.Unit1,
                FixedCount1 = item.FixedCount1,
                Unit2 = item.Unit2,
                FixedCount2 = item.FixedCount2,
                Price = item.Price,
                Currency = item.Currency,
                CurrencyCN = item.CurrencyCN,
                Remark1 = item.Remark1,
                Remark2 = item.Remark2,
                Status = item.Status,
                CreateDate = item.CreateDate,
                UpdateDate = item.UpdateDate,
                Summary = item.Summary,
            };
        }
    }

    public static class ChargeStandardExtend
    {
        public static void AddInto(
            this List<ChargeStandardListForMaintainView.ChargeStandardModel> list,
            ChargeStandardListForMaintainView.ChargeStandardModel one)
        {
            list.Add(one);
            if (one.Children != null)
            {
                foreach (var child in one.Children)
                {
                    list.AddInto(child);
                }
            }
        }
    }
}
