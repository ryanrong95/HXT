using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvWsOrder.Services.Models.Adopt
{
    public class ScCustomsCheck : IOrderCheck
    {
        public bool isVaildOrder(string orderID)
        {
            bool isVaild = false;
            try
            {
                using (foricScCustomsReponsitory reponsitory = new foricScCustomsReponsitory())
                {
                    int count = reponsitory.ReadTable<Layers.Data.Sqls.foricScCustoms.Orders>().Where(t => t.ID == orderID||t.MainOrderId==orderID).Count();
                    if (count > 0)
                    {
                        isVaild = true;
                    }
                }
            }
            catch(Exception ex)
            {

            }
            return isVaild;
        }

        public void UpdateFee(string orderID, decimal orderFee, string adminID)
        {
            try
            {
                if (orderFee != 0)
                {
                    var apisetting = new WlAdminApiSetting();
                    var apiurl = ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.AddOrderPremiums;
                    var result = Yahv.Utils.Http.ApiHelper.Current.Get(apiurl, new
                    {
                        orderID = orderID,
                        orderFee = orderFee,
                        adminID = adminID
                    });
                }               
            }
            catch (Exception ex)
            {

            }
        }
    }
}
