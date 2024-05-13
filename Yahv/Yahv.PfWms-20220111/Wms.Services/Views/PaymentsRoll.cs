using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms.Services.Models;
using Yahv.Linq;
using Yahv.Services.Views;

namespace Wms.Services.Views
{
    public class PaymentsWayBillRoll : QueryView<Models.DataPaymentWaybill, PvWmsRepository>
    {

        public PaymentsWayBillRoll()
        {

        }

        public PaymentsWayBillRoll(PvWmsRepository reponsitory) : base(reponsitory)
        {
        }

        public PaymentsWayBillRoll(PvWmsRepository reponsitory, IQueryable<DataPaymentWaybill> iQueryable) : base(reponsitory, iQueryable)
        {
        }


        protected override IQueryable<Models.DataPaymentWaybill> GetIQueryable()
        {

            var clientView = new Yahv.Services.Views.WsClientsTopView<PvWmsRepository>(this.Reponsitory);

            return from waybill in new Yahv.Services.Views.WaybillsTopView<PvWmsRepository>(this.Reponsitory)
                   join client in clientView on waybill.EnterCode equals client.EnterCode
                   orderby waybill.CreateDate descending
                   select new Models.DataPaymentWaybill
                   {
                       WaybillID = waybill.ID,
                       EnterCode = waybill.EnterCode,
                       ClientName = client.Name,
                       CreateDate = waybill.CreateDate,
                       CarrierID = waybill.CarrierID,
                       Supplier = waybill.Supplier,
                       Condition = waybill.Condition,
                       ConsignorID = waybill.ConsignorID,
                       Code = waybill.Code,
                       Place = waybill.Consignor.Place,
                       WaybillType = waybill.Type,
                      
                   };
        }

        public PaymentsWayBillRoll SearchByPartNumber(string name)
        {
            //string name = "AD620";
            var _productView = new Yahv.Services.Views.ProductsTopView<PvWmsRepository>(this.Reponsitory);

            var waybillIDView = from product in _productView
                                join notice in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>() on product.ID equals notice.ProductID
                                where product.PartNumber.Contains(name)
                                select notice.WaybillID;

            var topN = waybillIDView.Distinct().OrderBy(item => item).Take(500);
            var iQuery = from waybill in this.IQueryable
                         where topN.Contains(waybill.WaybillID)
                         select waybill;

            return new PaymentsWayBillRoll(this.Reponsitory, iQuery);
        }



        public PaymentsWayBillRoll SearchByWareHouseID(string warehouseID)
        {
            var waybillIDView = from notice in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                                where notice.WareHouseID.Contains(warehouseID)
                                select notice.WaybillID;

            List<string> topN = new List<string>();

            topN.AddRange(waybillIDView.Distinct().OrderBy(item => item).Take(500));

            var iQuery = from waybill in this.IQueryable
                         where topN.Contains(waybill.WaybillID)
                         select waybill;

            var roll = new PaymentsWayBillRoll(this.Reponsitory, iQuery);

            return roll;
        }

        public PaymentsWayBillRoll SearchByWaybillD(string id)
        {
            var iQuery = from waybill in this.IQueryable
                         where waybill.WaybillID == id
                         select waybill;

            return new PaymentsWayBillRoll(this.Reponsitory, iQuery);
        }


    }

    public static class PaymentsWayBillRollExtend
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="iqueryable"></param>
        /// <param name="type">type:in 收入 ，out 支出</param>
        /// <returns></returns>
        public static DataPaymentWaybill ToFillData(this IQueryable<DataPaymentWaybill> iqueryable, string type,string otype)
        {
            var waybill = iqueryable.FirstOrDefault();
            if (waybill == null)
            {
                return null;
            }

            string[] orderids = null;

            string CarrierName = null;
            string ClientName = null;
            string PayeeName = null;
            string ThirdName = null;


            using (var rep = new PvWmsRepository())
            {

                var list = new List<PamentParams>();
                if (otype=="in")
                {
                    var linq = from notice in rep.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                               join input in rep.ReadTable<Layers.Data.Sqls.PvWms.Inputs>() on notice.InputID equals input.ID
                               where notice.WaybillID == waybill.WaybillID
                               select new PamentParams{OrderID= input.OrderID,ClientID= input.ClientID,PayeeID= input.PayeeID,ThirdID= input.ThirdID };
                    list.AddRange(linq);
                }
                else 
                if (otype == "out")
                {
                    var linq = from notice in rep.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                               join input in rep.ReadTable<Layers.Data.Sqls.PvWms.Inputs>() on notice.InputID equals input.ID
                               join output in rep.ReadTable<Layers.Data.Sqls.PvWms.Outputs>() on notice.OutputID equals output.ID
                               where notice.WaybillID == waybill.WaybillID
                               select new PamentParams { OrderID = output.OrderID, ClientID = input.ClientID, PayeeID = input.PayeeID, ThirdID = input.ThirdID };
                    list.AddRange(linq);
                }


                orderids = list.Select(item => item.OrderID).Distinct().ToArray();
                if (list.Count() > 0)
                {
                    var first = list.First();
                    waybill.ClientID = first.ClientID;
                    waybill.PayeeID = first.PayeeID;
                    waybill.ThirdID = first.ThirdID;
        
                    if (!string.IsNullOrEmpty(waybill.CarrierID))
                        CarrierName = new Yahv.Services.Views.CarriersTopView<PvbCrmReponsitory>(new PvbCrmReponsitory()).Where(item => item.ID == waybill.CarrierID).FirstOrDefault().Name;
                    if (!string.IsNullOrEmpty(waybill.ClientID))
                        ClientName = new Yahv.Services.Views.EnterprisesTopView<PvbCrmReponsitory>(new PvbCrmReponsitory()).Where(item => item.ID == waybill.ClientID).FirstOrDefault().Name;
                    if (!string.IsNullOrEmpty(waybill.PayeeID))
                        PayeeName = new Yahv.Services.Views.EnterprisesTopView<PvbCrmReponsitory>(new PvbCrmReponsitory()).Where(item => item.ID == waybill.PayeeID).FirstOrDefault().Name;
                    if (!string.IsNullOrEmpty(waybill.ThirdID))
                        ThirdName = new Yahv.Services.Views.EnterprisesTopView<PvbCrmReponsitory>(new PvbCrmReponsitory()).Where(item => item.ID == waybill.ThirdID).FirstOrDefault().Name;


                }
            }

            if (orderids.Length > 0)
            {
                var files = new Yahv.Services.Views.CenterFilesTopView().Where(item => orderids.Contains(item.WsOrderID)).ToArray();

                if (type == "in")
                {
                    var paymets = new VouchersStatisticsView<PvbCrmReponsitory>(new PvbCrmReponsitory()).Where(tem => tem.WaybillID == waybill.WaybillID && orderids.Contains(tem.OrderID));
                    
                    waybill.Payments = paymets.AsEnumerable().Select(item=> { item.Payer = ClientName; item.Payee = PayeeName;item.Files =files.Where(tem=>tem.PayID==item.ReceivableID).Select(tem => { tem.Url = Yahv.Services.Models.CenterFile.Web + tem.Url; return tem; }).ToArray(); return item;});
                    waybill.OrderID = orderids.First(); waybill.CarrierName = CarrierName; waybill.ClientName = ClientName; waybill.PayeeName = PayeeName; waybill.ThirdName = ThirdName;
               
                    return waybill;
                }

                if (type == "out")
                {
                    var paymets = new PaymentsStatisticsView<PvbCrmReponsitory>(new PvbCrmReponsitory()).Where(tem => tem.WaybillID == waybill.WaybillID && orderids.Contains(tem.OrderID));
                   
                    waybill.Payments = paymets.AsEnumerable().Select(item => { item.Payer = ThirdName; item.Payee = CarrierName; item.Files = files.Where(tem => tem.PayID == item.PayableID).Select(tem => { tem.Url = Yahv.Services.Models.CenterFile.Web + tem.Url; return tem; }).ToArray(); return item; });
                    waybill.OrderID = orderids.First(); waybill.CarrierName = CarrierName; waybill.ClientName = ClientName; waybill.PayeeName = PayeeName; waybill.ThirdName = ThirdName;

                    return waybill;
                }
            }
            else
            {
                return iqueryable.FirstOrDefault();
            }


            return null;
        }

        class  PamentParams
        {
            public string OrderID { get; set; }
            public string ClientID { get; set; }
            public string PayeeID { get; set; }
            public string ThirdID { get; set; }
        }

    }
}
