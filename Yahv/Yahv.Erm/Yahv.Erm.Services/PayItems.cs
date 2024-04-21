using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls.PvbErm;
using Yahv.Erm.Services.Models.Origins;

namespace Yahv.Erm.Services
{
    public class PayWageItem
    {
        public string Name { get; set; }

        decimal _value;

        public decimal Value
        {
            get
            {
                decimal temp = 0;
                switch (Type)
                {
                    //普通列
                    case WageItemType.Normal:
                        return this._value;
                    //计算列
                    case WageItemType.Calc:
                        decimal result = Convert.ToDecimal(Formulas.Current[this.Name]
                            .Calc(father.Where(item => item.Name != this.Name).ToArray()));
                        result = Round(result, 2);

                        //保留整数,可以为负数（临时修改 以后优化）
                        if (Name == "奖金提成")
                        {
                            return Round(result, 0);
                        }

                        return result < 0 ? 0 : result;
                    //累计收入
                    case WageItemType.AccumIncome:
                        temp = GetAccumulated("工资合计", WageItemType.AccumIncome);
                        return temp < 0 ? 0 : temp;
                    //累计免税收入
                    case WageItemType.AccumFreeIncome:
                        return GetAccumFreeIncome(pastItems);
                    //累计专项扣除
                    case WageItemType.AccumSpecDeduction:
                        temp = GetAccumulated("养老,失业,工伤,医疗和生育,公积金", WageItemType.AccumSpecDeduction);
                        return temp < 0 ? 0 : temp;
                    //累计专项附加扣除
                    case WageItemType.AccumSpecAddDeduction:
                        temp = GetAccumulated("专项附加扣除", WageItemType.AccumSpecAddDeduction);
                        return temp < 0 ? 0 : temp;
                    //累计专项附加调整列
                    case WageItemType.AccumSpecAddAdjustments:
                        return GetAccumulated("累计专项附加调整", WageItemType.AccumSpecAddAdjustments);
                    //累计个税起征点调整
                    case WageItemType.AccumPitAdjustments:
                        return GetAccumulated("个税起征点调整", WageItemType.AccumPitAdjustments);
                    //累计预扣预缴应纳税所得额
                    case WageItemType.AccumPayTaxableIncome:
                        return GetAccumPayTaxableIncome(pastItems);
                    //累计已预扣预缴税额(累计个税)
                    case WageItemType.AccumPersonalIncomeTax:
                        return GetAccumPersonalIncomeTax();
                    //本月个税
                    case WageItemType.PersonalIncomeTax:
                        return CalcPersonalIncomeTax();
                    default:
                        throw new NotSupportedException("不支持指定的类型" + Type.ToString());
                }
            }
            set
            {
                this._value = value;
            }
        }

        public WageItemType Type { get; set; }

        internal IEnumerable<PayWageItem> father;

        /// <summary>
        /// 上一个月累计值
        /// </summary>
        internal IEnumerable<PastsItem> pastItems;

        /// <summary>
        /// 个税税率
        /// </summary>
        internal IEnumerable<PersonalRate> personalRates;

        /// <summary>
        /// 累计免税收入 是否固定
        /// </summary>
        /// <remarks>1-12月份全年都在同一个公司的人固定是60000; 中间调转过公司，新入职的按照每月5000累计</remarks>
        public bool IsFixed { get; set; }

        /// <summary>
        /// 工资月
        /// </summary>
        public string DateIndex { get; set; }

        #region 私有函数
        /// <summary>
        /// 累计免税收入
        /// </summary>
        /// <param name="pItems"></param>
        /// <returns></returns>
        private decimal GetAccumFreeIncome(IEnumerable<PastsItem> pItems)
        {
            decimal result = 5000;
            decimal fixedValue = 60000;

            //历史值为空，并且不是第一个月
            if ((pItems == null || !pItems.Any()) && this.DateIndex.Substring(4, 2) != "01")
            {
                return result;
            }

            if (this.IsFixed)
            {
                return fixedValue;
            }

            if (pItems.FirstOrDefault() != null)
            {
                result += pItems.FirstOrDefault(t => t.Type == WageItemType.AccumFreeIncome).Value;
            }

            return result;
        }

        /// <summary>
        /// 获取累计值
        /// </summary>
        /// <param name="wageItems">工资项具体值</param>
        /// <param name="pItems">上个月累计值</param>
        /// <param name="items">工资项字符串</param>
        /// <param name="type">工资项类型</param>
        /// <returns></returns>
        private decimal GetAccumulated(string items, WageItemType type)
        {
            decimal result = 0;

            if (father == null || !father.Any())
                return 0;

            if (!string.IsNullOrWhiteSpace(items))
            {
                //循环添加累计值
                foreach (var str in items.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                {
                    result += father.FirstOrDefault(item => item.Name == str) != null
                  ? father.FirstOrDefault(item => item.Name == str).Value : 0;
                }
            }

            //从累计表获取累计值
            if (pastItems.FirstOrDefault(t => t.Type == type) != null)
            {
                result += pastItems.FirstOrDefault(t => t.Type == type).Value;
            }

            result = Round(result, 2);
            return result;
        }

        /// <summary>
        /// 计算累计预扣预缴应纳税所得额
        /// </summary>
        /// <returns></returns>
        private decimal GetAccumPayTaxableIncome(IEnumerable<PastsItem> pItems)
        {
            decimal result = 0;

            //累计预扣预缴应纳税所得额 = 累计收入 - 累计免税收入 - 累计专项扣除 - 累计专项附加扣除- 累计专项附加调整 - 累计个税起征点调整 
            result = father.FirstOrDefault(t => t.Type == WageItemType.AccumIncome) != null
                ? father.FirstOrDefault(t => t.Type == WageItemType.AccumIncome).Value
                : 0;

            result = result - (father.FirstOrDefault(t => t.Type == WageItemType.AccumFreeIncome) != null
                ? father.FirstOrDefault(t => t.Type == WageItemType.AccumFreeIncome).Value
                : 0);

            result = result - (father.FirstOrDefault(t => t.Type == WageItemType.AccumSpecDeduction) != null
                ? father.FirstOrDefault(t => t.Type == WageItemType.AccumSpecDeduction).Value
                : 0);

            result = result - (father.FirstOrDefault(t => t.Type == WageItemType.AccumSpecAddDeduction) != null
                ? father.FirstOrDefault(t => t.Type == WageItemType.AccumSpecAddDeduction).Value
                : 0);

            result = result - (father.FirstOrDefault(t => t.Type == WageItemType.AccumSpecAddAdjustments) != null
                ? father.FirstOrDefault(t => t.Type == WageItemType.AccumSpecAddAdjustments).Value
                : 0);

            result = result - (father.FirstOrDefault(t => t.Type == WageItemType.AccumPitAdjustments) != null
                ? father.FirstOrDefault(t => t.Type == WageItemType.AccumPitAdjustments).Value
                : 0);

            result = Round(result, 2);
            return result < 0 ? 0 : result;
        }

        /// <summary>
        /// 计算当月个税
        /// </summary>
        /// <returns></returns>
        private decimal CalcPersonalIncomeTax()
        {
            decimal result = 0;

            var payTaxableIncome = father.FirstOrDefault(item => item.Type == WageItemType.AccumPayTaxableIncome) != null ? father.FirstOrDefault(item => item.Type == WageItemType.AccumPayTaxableIncome).Value : 0;       //累计预扣预缴应纳税所得额
            var accumPersonalIncomeTax = father.FirstOrDefault(item => item.Type == WageItemType.AccumPersonalIncomeTax) != null ? father.FirstOrDefault(item => item.Type == WageItemType.AccumPersonalIncomeTax).Value : 0;       //累计已预扣预缴税额
            decimal YKL = personalRates.FirstOrDefault(item => item.BeginAmount < payTaxableIncome && item.EndAmount >= payTaxableIncome).Rate;      //预扣率
            decimal SSKCS = personalRates.FirstOrDefault(item => item.BeginAmount < payTaxableIncome && item.EndAmount >= payTaxableIncome).Deduction;        //速算扣除数

            //本月个税 =（累计预扣预缴应纳税所得额 × 预扣率 - 速算扣除数) - 累计已预扣预缴税额
            result = (payTaxableIncome * YKL - SSKCS) - accumPersonalIncomeTax;

            result = Round(result, 2);
            return result < 0 ? 0 : result;
        }

        /// <summary>
        /// 获取累计已预扣预缴税额
        /// </summary>
        /// <returns></returns>
        private decimal GetAccumPersonalIncomeTax()
        {
            decimal result = 0;

            //从累计表获取累计值
            if (pastItems.FirstOrDefault(t => t.Type == WageItemType.AccumPersonalIncomeTax) != null)
            {
                result += pastItems.FirstOrDefault(t => t.Type == WageItemType.AccumPersonalIncomeTax).Value;
            }

            //需要把上一个月的本月个税累计到已预扣预缴税额
            if (pastItems.FirstOrDefault(t => t.Type == WageItemType.PersonalIncomeTax) != null)
            {
                result += pastItems.FirstOrDefault(t => t.Type == WageItemType.PersonalIncomeTax).Value;
            }

            return result;
        }

        /// <summary>
        /// 四舍五入
        /// </summary>
        /// <param name="value"></param>
        /// <param name="decimals"></param>
        /// <returns></returns>
        private decimal Round(decimal value, int decimals)
        {
            decimal result = value;

            if (value < 0)
            {
                result = Math.Abs(result);
            }

            result = Math.Round(result, decimals, MidpointRounding.AwayFromZero);

            if (value < 0)
            {
                result = 0 - result;
            }

            return result;
        }
        #endregion
    }


    public class PayWageItems : IEnumerable<PayWageItem>
    {
        List<PayWageItem> data;

        public PayWageItems(IEnumerable<PayWageItem> data, IEnumerable<PastsItem> pastsItem, IEnumerable<PersonalRate> personalRates, string dateIndex, bool isFixed)
        {
            this.data = new List<PayWageItem>(data.Select(item =>
            {
                item.father = this;
                item.pastItems = pastsItem;
                item.personalRates = personalRates;
                item.DateIndex = dateIndex;
                item.IsFixed = isFixed;
                return item;
            }));
        }

        public int Count { get { return this.data.Count; } }

        public IEnumerator<PayWageItem> GetEnumerator()
        {
            return this.data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
