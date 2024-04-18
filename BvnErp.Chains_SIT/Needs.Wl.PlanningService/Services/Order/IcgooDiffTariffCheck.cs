using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.PlanningService.Services.Order
{
    public  class IcgooDiffTariffCheck
    {
        public string OrderIDs { get; set; }

        public IcgooDiffTariffCheck(string orderIDs)
        {
            this.OrderIDs = orderIDs;
        }

        public void Check()
        {
            if (!string.IsNullOrEmpty(OrderIDs))
            {
                string[] IDs = this.OrderIDs.Split(',');
                string curMonth = DateTime.Now.ToString("yyyyMM");
                var exclusions = new ExcludeOriginTariffsView().Where(t => t.ExclusionPeriod == curMonth && t.Status == Status.Normal).ToList();

                using (Needs.Ccs.Services.Views.IFClassifyResultView view = new Needs.Ccs.Services.Views.IFClassifyResultView())
                {
                    foreach (var item in IDs)
                    {
                        var currentOrder = new Needs.Ccs.Services.Views.OrdersView()[item];
                        if (currentOrder != null)
                        {
                            foreach (var orderItem in currentOrder.Items)
                            {
                                string productUnionCode = orderItem.ProductUniqueCode;
                                decimal? classifyTariffRate = orderItem.ImportTax?.Rate;

                                var preClassifyInfo = view.Where(t => t.PreProductUnicode == productUnionCode && t.ClassifyStatus == ClassifyStatus.Done && t.CompanyType != CompanyTypeEnums.Inside).FirstOrDefault();
                                if (preClassifyInfo != null)
                                {
                                    decimal? preTariffRate = preClassifyInfo.TariffRate;
                                    if (classifyTariffRate != preTariffRate)
                                    {
                                        var excluInfo = exclusions.Where(t => t.HSCode == preClassifyInfo.HSCode && t.Origin == orderItem.Origin).FirstOrDefault();
                                        if (excluInfo == null)
                                        {
                                            Needs.Ccs.Services.Models.ApiNotice apiNotice = new Ccs.Services.Models.ApiNotice();
                                            apiNotice.ClientID = currentOrder.Client.ID;
                                            apiNotice.ItemID = orderItem.ID;
                                            apiNotice.PushType = PushType.TariffDiff;

                                            apiNotice.Enter();
                                        }                                       
                                    }
                                }                               
                            }
                        }
                    }
                }
            }              
        }
    }
}
