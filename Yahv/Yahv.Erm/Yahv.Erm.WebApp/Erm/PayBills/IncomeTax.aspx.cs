using System;
using System.Linq;
using Yahv.Web.Erp;

namespace Yahv.Erm.WebApp.Erm.PayBills
{
    public partial class IncomeTax : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitData();
            }

        }

        private void InitData()
        {
            try
            {
                var dateIndex = Request.QueryString["dateIndex"];
                var staffId = Request.QueryString["staffId"];

                if (string.IsNullOrWhiteSpace(dateIndex) || string.IsNullOrWhiteSpace(staffId))
                {
                    return;
                }

                var payItems = Alls.Current.PayItems.Where(item => item.PayID == (staffId.Replace("Staff", "") + "-" + dateIndex)).ToList();
                var personalRates = Alls.Current.PersonalRates.ToList();

                var LJGS = payItems.FirstOrDefault(t => t.Name == "累计预扣预缴应纳税所得额")?.Value ?? 0;
                decimal YKL = personalRates.FirstOrDefault(item => item.BeginAmount < LJGS && item.EndAmount >= LJGS).Rate;      //预扣率
                decimal SSKCS = personalRates.FirstOrDefault(item => item.BeginAmount < LJGS && item.EndAmount >= LJGS).Deduction;        //速算扣除数

                this.Model = new
                {
                    DateIndex = dateIndex,
                    StaffName = payItems.FirstOrDefault()?.StaffName,
                    LJSR = payItems.FirstOrDefault(t => t.Name == "累计收入")?.Value ?? 0,     //累计收入
                    MSSR = payItems.FirstOrDefault(t => t.Name == "累计免税收入")?.Value ?? 0,     //累计免税收入
                    ZXKC = payItems.FirstOrDefault(t => t.Name == "累计专项扣除")?.Value ?? 0,     //累计专项扣除
                    ZXFJKC = payItems.FirstOrDefault(t => t.Name == "累计专项附加扣除")?.Value ?? 0,       //累计专项附加扣除
                    ZXFJTZ = payItems.FirstOrDefault(t => t.Name == "累计专项附加调整列")?.Value ?? 0,       //累计专项附加调整
                    GSQZDTZ = payItems.FirstOrDefault(t => t.Name == "累计个税起征点调整")?.Value ?? 0,      //累计个税起征点调整
                    BYGSLJ = payItems.FirstOrDefault(t => t.Name == "累计已预扣预缴税额")?.Value ?? 0,         //累计个税
                    YKL,      //预扣率
                    SSKCS,        //速算扣除数
                    LJGS,     //累计预扣预缴应纳税所得额
                    GS = payItems.FirstOrDefault(t => t.Name == "本月个税")?.Value ?? 0,       //本期应预扣预缴税额
                };
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}