using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.WL.ConsoleToolApp
{
    public class DecListRepair
    {
        public DateTime DtStart { get; set; }
        public DateTime DtEnd { get; set; }
        public DecListRepair(DateTime dtStart, DateTime dtEnd)
        {
            this.DtStart = dtStart;
            this.DtEnd = dtEnd;
        }

        public void Repair()
        {
            string decHeadID = "";
            try
            {
                var decHeads = new Needs.Ccs.Services.Views.DecHeadsView().Where(t => t.CreateTime > this.DtStart && t.CreateTime < this.DtEnd).OrderByDescending(t => t.CreateTime).ToList();
                foreach (var decHead in decHeads)
                {
                    decHeadID = decHead.ID;
                    var remainDeci = decHead.isTwoStep ? 2 : 0;
                    var decLists = decHead.Lists;

                    using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                    {
                        var orderCus = new Ccs.Services.Views.Orders2View().Where(t => t.ID == decHead.OrderID).FirstOrDefault();
                        decimal realExchangeRate = orderCus.CustomsExchangeRate.Value;
                        var orderitems = new Needs.Ccs.Services.Views.OrdersView()[decHead.OrderID].Items.ToList();
                        foreach (var item in decLists)
                        {
                            string OdecListID = item.ID;

                            string decListID = "";
                            if (item.OrderItemID.Contains("XDT"))
                            {
                                decListID = item.OrderItemID.Replace("XDTOrder", "DecList");
                            }
                            else
                            {
                                decListID = item.OrderItemID.Replace("Order", "DecList");
                            }

                            decimal taxedPrice = Math.Round(item.DeclTotal * realExchangeRate, remainDeci, MidpointRounding.AwayFromZero);
                            decimal tariff = orderitems.Where(t => t.ID == item.OrderItemID).FirstOrDefault().ImportTax.Value.Value;
                            taxedPrice += tariff;
                            reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecLists>(new { DecListID = decListID, TaxedPrice = taxedPrice }, t => t.ID == OdecListID);

                            Console.WriteLine(decHeadID + ":" + OdecListID + "更新完成");
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                string test = decHeadID + ex.ToString();
            }

        }

        public void RepairNew()
        {
            string decHeadID = "";
            try
            {

                var decHeads = new Needs.Ccs.Services.Views.DecHeadsView().Where(t => t.CreateTime > this.DtStart && t.CreateTime < this.DtEnd).OrderByDescending(t => t.CreateTime).ToList();
                //var decHeads = new Needs.Ccs.Services.Views.DecHeadsView().Where(t =>t.ID== "XDTCDO202106010000001").ToList();
                //var decHeads = new Needs.Ccs.Services.Views.DecHeadsView().Where(t => decIDs.Contains(t.ID)).ToList();
                foreach (var decHead in decHeads)
                {
                    decHeadID = decHead.ID;
                    var remainDeci = decHead.isTwoStep ? 2 : 0;
                    var decLists = decHead.Lists;

                    using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                    {
                        var orderCus = new Ccs.Services.Views.Orders2View().Where(t => t.ID == decHead.OrderID).FirstOrDefault();
                        decimal realExchangeRate = orderCus.CustomsExchangeRate.Value;
                        var orderitems = new Needs.Ccs.Services.Views.OrdersView()[decHead.OrderID].Items.ToList();
                        foreach (var item in decLists)
                        {
                            //if (item.TaxedPrice == null)
                            //{
                            string OdecListID = item.ID;
                            decimal taxedPrice = Math.Round(item.DeclTotal * realExchangeRate, remainDeci, MidpointRounding.AwayFromZero);
                            decimal tariff = orderitems.Where(t => t.ID == item.OrderItemID).FirstOrDefault().ImportTax.Value.Value;
                            if (tariff != 0)
                            {
                                decimal declareQty = item.GQty;
                                decimal orderItemQty = orderitems.Where(t => t.ID == item.OrderItemID).FirstOrDefault().Quantity;
                                if (declareQty != orderItemQty)
                                {
                                    decimal rate = declareQty / orderItemQty;
                                    tariff = tariff * rate;
                                }                                
                            }
                            taxedPrice += tariff;
                            reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecLists>(new { TaxedPrice = taxedPrice }, t => t.ID == OdecListID);

                            Console.WriteLine(decHeadID + ":" + OdecListID + "更新完成");
                            //}
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                string test = decHeadID + ex.ToString();
                Console.WriteLine(test);
            }

        }

        public void UpdateInputID()
        {
            string decHeadID = "";
            try
            {
                var decHeads = new Needs.Ccs.Services.Views.DecHeadsView().Where(t => t.CreateTime > this.DtStart && t.CreateTime < this.DtEnd).OrderByDescending(t => t.CreateTime).ToList();
                //var decHeads = new Needs.Ccs.Services.Views.DecHeadsView().Where(t => t.ID == "XDTCDO202106010000001").ToList();
                foreach (var decHead in decHeads)
                {
                    decHeadID = decHead.ID;
                    var decLists = decHead.Lists;

                    using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                    {
                        var deliveriesTopViews = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Sz_Cfb_InView>().Where(item => item.LotNumber == decHead.VoyNo).ToList();
                        foreach (var item in decLists)
                        {
                            var InputInfo = deliveriesTopViews.Where(t => t.OrderItemID == item.OrderItemID && t.BoxCode.Contains(item.CaseNo.Trim())).FirstOrDefault();
                            if (InputInfo != null)
                            {
                                string InputID = InputInfo.InputID;
                                reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecLists>(new { InputID = InputID }, t => t.ID == item.ID);
                                Console.WriteLine(decHeadID + ":" + item.ID + "更新完成");
                            }
                            else
                            {
                                Console.WriteLine("空"+decHeadID + ":" + item.ID + "为空");
                            }                            
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string test = decHeadID + ex.ToString();
                Console.WriteLine(test);
            }
        }
    }
}
