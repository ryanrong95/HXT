using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Views;
using Needs.Utils.Serializers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class DyjPayExchangeStatus
    {
        public void DYJStatusUpdate()
        {
            var param = "?key=" + DyjCwConfig.Key +
                "&sdate=" + DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd")+
                "&edate=" + DateTime.Now.ToString("yyyy-MM-dd") + "&id&skCorp&kpCorp&uid&kpType&state";


            try
            {
                //string test = request.Json();
                var api = System.Configuration.ConfigurationManager.AppSettings["DYJFeeApplyApiURL"];
                var get = Needs.Utils.Http.ApiHelper.Current.Get(api + DyjCwConfig.GetFeeApplyAList + param);
                //格式化json字符串
                var result = new Newtonsoft.Json.JsonSerializer().Deserialize(new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader(get)));
                //反序列化Response
                var response = Newtonsoft.Json.JsonConvert.DeserializeObject<DyjGetFeeApplyAListResponse>(result.ToString());

                foreach (var item in response.data)
                {

                    if (item.费用类型 == "汇兑")
                    {
                        var payexchange = new Views.AdminPayExchangeApplyView().FirstOrDefault(t => t.DyjID == item.编号 + ".0");

                        if (payexchange != null)
                        {
                            
                                //已审批，待付款
                                if (payexchange.PayExchangeApplyStatus == PayExchangeApplyStatus.Approvaled)
                                {
                                    if (item.状态 == "等待付款" || item.状态 == "部分付款")
                                    { 
                                        //相当于还没有结果，不需处理
                                    }

                                    if (item.状态 == "付款完成") {
                                        //付款完成，处理华芯通付款通知

                                        #region 处理付款通知

                                        var Notice = new PaymentNoticesView().FirstOrDefault(t=>t.PayExchangeApply.ID == payexchange.ID);
                                        
                                        //TODO
                                        //Notice.SeqNo = "0";
                                        //Notice.Poundage = null;
                                        //Notice.SeqNoPoundage = null;
                                        ////Notice.USDAmount = decimal.Parse(USDAmount);
                                        //Notice.PayDate = actualPayTime;//付款日期
                                        ////Notice.ExchangeRate = decimal.Parse(ExchangeRate);
                                        //Notice.PayType = PaymentType.TransferAccount;
                                        //Notice.FinanceVault = new FinanceVault { ID = FinanceVault };
                                        //Notice.FinanceAccount = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceAccounts.Where(t => t.ID == FinanceAccount).FirstOrDefault();
                                        //Notice.SetOperator(Needs.Underly.FkoFactory<Admin>.Create("Admin0000009365"));
                                        //Notice.Paid();

                                        #endregion

                                        //reponsitory.Update<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>(new
                                        //{
                                        //    UpdateDate = DateTime.Now,
                                        //    PayExchangeApplyStatus = PayExchangeApplyStatus.Completed
                                        //}, c => c.ID == payexchange.ID);

                                    

                                    if (item.状态 == "不能付款")
                                    {
                                        //reponsitory.Update<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>(new
                                        //{
                                        //    UpdateDate = DateTime.Now,
                                        //    PayExchangeApplyStatus = PayExchangeApplyStatus.Cancled
                                        //}, c => c.ID == payexchange.ID);

                                        //将付款通知改为 不能付款或失效
                                        payexchange.Log(Underly.FkoFactory<Admin>.Create("Admin0000009365"), "出纳[郝红梅]审批退回了付汇申请，退回原因：无法付款");

                                    }
                                }                               
                            }
                        }
                    }

                    if (item.费用类型 == "付款")
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                ex.Source += "DyjPayExchangeStatus";
                ex.CcsLog("同步大赢家状态错误");
            }
        }
    }
}

public class dyjPayExRequest
{
    public string sdate { get; set; }
    public string edate { get; set; }
    public string id { get; set; }
    public string skCorp { get; set; }
    public string kpCorp { get; set; }
    public string uid { get; set; }
    public string kpType { get; set; }
    public string state { get; set; }
    public string key { get; set; }
}

public class DyjGetFeeApplyAListResponse
{
    public string status { get; set; }

    public string message { get; set; }

    public bool isSuccess { get; set; }

    public bool isPage { get; set; }

    //public DyjPageInfo pageInfo { get; set; }

    public string list { get; set; }

    public List<dyjPayExRespond> data { get; set; }

    public string filelist { get; set; }

    public string taken { get; set; }
}

public class dyjPayExRespond
{
    public string 编号 { get; set; }
    public string 制单日期 { get; set; }
    public string 费用类型 { get; set; }
    public string 制单人 { get; set; }
    public string 付款公司 { get; set; }
    public string 币种 { get; set; }
    public string 收款单位 { get; set; }
    public string 申请金额 { get; set; }
    public string 外币金额 { get; set; }
    public string 状态 { get; set; }
    public string 公司 { get; set; }
    public string 部门 { get; set; }
    public string 员工 { get; set; }
    public string 摘要 { get; set; }
    public decimal 明细金额 { get; set; }
    public decimal 明细外币金额 { get; set; }
    public decimal 剩余金额 { get; set; }
    public decimal 剩余外币金额 { get; set; }
}


