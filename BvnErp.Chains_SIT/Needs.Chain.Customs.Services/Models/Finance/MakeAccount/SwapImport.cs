using Needs.Ccs.Services.ApiSettings;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{ 
    public  class SwapImport
    {
        public List<string> SwapIDs { get; set; }
        public SwapImportModel DataSwap { get; set; }
        public SwapImport()
        {
            string requestId = Needs.Overall.PKeySigner.Pick(PKeyType.XDTRecPre);
            this.DataSwap = new SwapImportModel();
            DataSwap.归属模板编号 = MakeAccountSetting.SwapImport_归属模板编号;
            DataSwap.归属方案编号 = MakeAccountSetting.SwapImport_归属方案编号;
            DataSwap.归属账套 = MakeAccountSetting.归属账套;
            DataSwap.源文件原始名称 = requestId;
            DataSwap.源文件内容 = new List<Voucher_SFD_XDT_FHK_XGGS_YHHH>();

            this.SwapIDs = new List<string>();
        }

        public void GenData(string ContractNo, string EntryID, string BankName, string StartDate, string EndDate,List<string> SelectedIDs)
        {
            var view = new Views.SwapNoticeReportView();
            var viewMain = view.GetSwapNoticeMain();

            var viewDetail = view.Where(t=>t.SwapDate>=Convert.ToDateTime(StartDate)&&t.SwapDate<Convert.ToDateTime(EndDate).AddDays(1)).AsQueryable();

            //查询条件
            if (!string.IsNullOrEmpty(BankName))
            {
                BankName = BankName.Trim();
                viewMain = viewMain.Where(t => t.BankName == BankName);
                viewDetail = viewDetail.Where(t => t.SwapBank == BankName);
            }
            if (!string.IsNullOrEmpty(StartDate))
            {
                DateTime start = Convert.ToDateTime(StartDate);
                viewMain = viewMain.Where(t => t.SwapDate >= start);
                viewDetail = viewDetail.Where(t => t.SwapDate >= start);
            }
            if (!string.IsNullOrEmpty(EndDate))
            {
                DateTime end = Convert.ToDateTime(EndDate).AddDays(1);
                viewMain = viewMain.Where(t => t.SwapDate < end);
                viewDetail = viewDetail.Where(t => t.SwapDate < end);
            }
            if (!string.IsNullOrEmpty(ContractNo))
            {
                ContractNo = ContractNo.Trim();
                viewDetail = viewDetail.Where(t => t.ContractNo == ContractNo);
            }
            if (!string.IsNullOrEmpty(EntryID))
            {
                EntryID = EntryID.Trim();
                viewDetail = viewDetail.Where(t => t.EntryID == EntryID);
            }

            //主表执行查询
            var mainList = viewMain.ToArray();

            //明细表执行查询
            var detailList = view.GetNoticeDetailWithDecPrice(viewDetail.ToArray());

            //换汇统计表: 明细表的分类汇总
            var detailSum = (from detail in detailList
                             group detail by new { detail.SwapNoticeID, detail.DDate, detail.DDateRate, detail.ClientName, detail.SupplierName, detail.Currency, detail.SwapDate, detail.SwapBank, detail.SwapBankMiddle, detail.SwapRate } into detail_group
                             select new SwapNoticeCollect
                             {
                                 SwapNoticeID = detail_group.Key.SwapNoticeID,
                                 DDate = detail_group.Key.DDate,
                                 DDateRate = detail_group.Key.DDateRate,
                                 ClientName = detail_group.Key.ClientName,
                                 SupplierName = detail_group.Key.SupplierName,
                                 Currency = detail_group.Key.Currency,
                                 DeclPrice = detail_group.Sum(t => t.DeclPrice),
                                 DeclarePrice = detail_group.Sum(t => t.DeclarePrice),
                                 SwapPrice = detail_group.Sum(t => t.SwapPrice),
                                 SwapDate = detail_group.Key.SwapDate,
                                 SwapBank = detail_group.Key.SwapBank,
                                 SwapBankMiddle = detail_group.Key.SwapBankMiddle,
                                 SwapRate = detail_group.Key.SwapRate,

                                 DeclarePriceRMB = detail_group.Sum(t => t.DeclarePrice) * detail_group.Key.DDateRate,
                                 DeclPriceRMB = detail_group.Sum(t => t.DeclPrice) * detail_group.Key.DDateRate,
                                 ScalePrice = detail_group.Sum(t => t.ScalePrice),
                                 OtherPrice = detail_group.Sum(t => t.OtherPrice),
                                 ScaleOtherPrice = detail_group.Sum(t => t.ScaleOtherPrice),
                                 DeclExchangeSpread = detail_group.Sum(t => t.DeclExchangeSpread),
                                 OtherExchangeSpread = detail_group.Sum(t => t.OtherExchangeSpread)
                             }).ToList();

            //K3做账银行名称转换
            //K3账户名词修改
            foreach (var m in mainList)
            {
                //K3账户名词修改
                switch (m.BankName)
                {
                    case ("星展银行"):
                        m.BankName = "星展银行（中国）有限公司深圳分行";
                        break;
                    case ("农业银行"):
                        m.BankName = "中国农业银行股份有限公司深圳免税大厦支行";
                        m.SwapBankMiddle = "中国农业银行深圳免税大厦支行美金账户";
                        break;
                    case ("宁波银行"):
                        m.BankName = "宁波银行深圳分行";
                        m.SwapBankMiddle = "宁波银行深圳分行美金账户";
                        break;
                    case ("华夏银行"):
                        m.BankName = "芯达通-华夏银行人民币账户";
                        break;
                    default:
                        break;
                }
            }

            var mainData = mainList;
            if (SelectedIDs.Count > 0)
            {
                mainData = mainList.Where(t => SelectedIDs.Contains(t.SwapNoticeID)).ToArray();
                this.SwapIDs = SelectedIDs;
            }
            else
            {
                this.SwapIDs = mainList.Select(t => t.SwapNoticeID).ToList();
            }

            //解决误差问题
            foreach (var main in mainData)
            {
                var sumData = detailSum.Where(t => t.SwapNoticeID == main.SwapNoticeID).ToList();
                var diff = main.SwapAmountRMB - sumData.Sum(t => t.DeclExchangeSpread + t.OtherExchangeSpread + t.DDateRateScalePrice + t.DDateRateScaleOtherPrice);
                detailSum.Where(t => t.SwapNoticeID == main.SwapNoticeID).FirstOrDefault().OtherExchangeSpread = detailSum.Where(t => t.SwapNoticeID == main.SwapNoticeID).FirstOrDefault().OtherExchangeSpread + diff;
            }

            foreach (var r in mainData)
            {
                Voucher_SFD_XDT_FHK_XGGS_YHHH m = new Voucher_SFD_XDT_FHK_XGGS_YHHH();
                m.换汇ID = r.SwapNoticeID;
                m.境外发货人 = r.ConsignorCode;
                m.换汇美金 = r.SwapAmount;
                m.换汇汇率 = r.SwapRate;
                m.换汇金额RMB = r.SwapAmountRMB;
                m.换汇银行 = r.BankName;
                m.过渡银行 = r.SwapBankMiddle;
                m.换汇日期 = r.SwapDate==null?"": r.SwapDate.ToString("yyyy-MM-dd");
                m.标识 = r.SwapNoticeID;
                m.币别 = "";
                m.换汇统计 = new List<Voucher_SFD_XDT_FHK_XGGS_YHHH_Item>();

                var detailSumData = detailSum.Where(t => t.SwapNoticeID == r.SwapNoticeID).ToList();
                foreach(var d in detailSumData)
                {
                    Voucher_SFD_XDT_FHK_XGGS_YHHH_Item item = new Voucher_SFD_XDT_FHK_XGGS_YHHH_Item();
                    item.币别 = d.Currency;
                    item.客户名称 = d.ClientName;
                    item.境外发货人 = d.SupplierName;
                    item.对应比例的委托金额 = d.ScalePrice;
                    item.对应比例的运保杂 = d.ScaleOtherPrice;
                    item.报关当天汇率 = d.DDateRate;
                    item.客户委托金额汇差 = d.DeclExchangeSpread;
                    item.芯达通运保杂汇差 = d.OtherExchangeSpread;
                    item.对应比例的委托金额RMB报关 = d.DDateRateScalePrice;
                    item.对应比例的运保杂金额RMB报关 = d.DDateRateScaleOtherPrice;

                    m.换汇统计.Add(item);
                }

                if (m.换汇统计.FirstOrDefault() != null)
                {
                    m.币别 = m.换汇统计.FirstOrDefault().币别;
                }

                DataSwap.源文件内容.Add(m);
            }
        }

        public bool Make(string ContractNo, string EntryID, string BankName, string StartDate, string EndDate, List<string> SelectedIDs)
        {
            try
            {
                GenData(ContractNo, EntryID, BankName, StartDate, EndDate, SelectedIDs);
                var flag = true;

                var mk = new MakeAccountHandler();

                string requestid = DataSwap.源文件原始名称;
                //报关进口-全额开票
                if (DataSwap.源文件内容.Count() > 0)
                {
                    var jResult = mk.PostToK3(this.DataSwap);
                    using (Layer.Data.Sqls.ScCustomsReponsitory responsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                    {
                        string swapIDs = string.Join(",", SwapIDs.ToArray());
                        //记录日志
                        responsitory.Insert(new Layer.Data.Sqls.ScCustoms.Logs
                        {
                            ID = ChainsGuid.NewGuidUp(),
                            Name = "生成凭证",
                            MainID = "换汇",
                            AdminID = "XDTAdmin",
                            Summary = swapIDs + " " + jResult.Json(),
                            Json = DataSwap.Json(),
                            CreateDate = DateTime.Now,
                        });

                        ////成功，更新apiNotice
                        if (jResult.success && jResult.data)
                        {
                            responsitory.Update<Layer.Data.Sqls.ScCustoms.SwapNotices>(
                                    new
                                    {
                                        SwapCreSta = true,
                                        RequestID = requestid
                                    }, item => SwapIDs.Contains(item.ID));

                            flag &= true;
                        }
                        else
                        {
                            flag &= false;
                        }
                    }
                }

                return flag;
            }
            catch (Exception ex)
            {
                string swapIDs = string.Join(",", SwapIDs.ToArray());
                ex.CcsLog("换汇：" + swapIDs);
                return false;
            }
        }
    }

    public class SwapImportModel
    {
        public string 归属模板编号 { get; set; }
        public string 归属方案编号 { get; set; }
        public string 归属账套 { get; set; }
        public string 源文件原始名称 { get; set; }
        public List<Voucher_SFD_XDT_FHK_XGGS_YHHH> 源文件内容 { get; set; }
    }

    public class Voucher_SFD_XDT_FHK_XGGS_YHHH
    {
        public string 标识 { get; set; }
        public string 换汇ID { get; set; }
        public string 境外发货人 { get; set; }
        public decimal 换汇美金 { get; set; }
        public decimal 换汇汇率 { get; set; }
        public decimal 换汇金额RMB { get; set; }
        public string 换汇银行 { get; set; }
        public string 过渡银行 { get; set; }
        public string 换汇日期 { get; set; }
        public string 币别 { get; set; }
        public List<Voucher_SFD_XDT_FHK_XGGS_YHHH_Item> 换汇统计 { get; set; }
    }

    public class Voucher_SFD_XDT_FHK_XGGS_YHHH_Item
    {
        public string 币别 { get; set; }
        public string 客户名称 { get; set; }
        public string 境外发货人 { get; set; }
        public decimal 对应比例的委托金额 { get; set; }
        public decimal 对应比例的运保杂 { get; set; }
        public decimal 报关当天汇率 { get; set; }
        public decimal 客户委托金额汇差 { get; set; }
        public decimal 芯达通运保杂汇差 { get; set; }
        /// <summary>
        /// 本位币
        /// </summary>
        public decimal 对应比例的委托金额RMB报关 { get; set; }
        /// <summary>
        /// 本位币
        /// </summary>
        public decimal 对应比例的运保杂金额RMB报关 { get; set; }
    }
}
