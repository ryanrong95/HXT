using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wl.HistoryImport
{
    public class HistoryCreateOrder
    {

        public void AssemblyOrder(HistoryUseOnly only)
        {
            List<InsideOrderItem> models = new List<InsideOrderItem>();
            List<PackHistoryOnly> packs = new List<PackHistoryOnly>();
            var CustomsTariffsView = new Needs.Ccs.Services.Views.CustomsTariffsView();
            var CountriesView = new Needs.Ccs.Services.Views.BaseCountriesView();
            InsideOrderItem orderItem = new InsideOrderItem();

            

            using (YD_LogisticsDBEntities db = new YD_LogisticsDBEntities())
            {
                var query = from c in db.T_Order_Model
                            where c.OrderID == only.OrderID
                            select c;

                foreach(var item in query.ToList())
                {
                    orderItem = new InsideOrderItem();
                    orderItem.No = item.DisplayOrder.Value.ToString();
                    orderItem.PreProductID = "";
                    orderItem.CustomsCode = item.CustomsCode;
                    orderItem.ProductName = item.DeclareProductName;
                    orderItem.Elements = item.Elements;
                    orderItem.Quantity = item.Quantity; 
                    orderItem.Unit = "007";
                    orderItem.FirstLegalUnit = item.FirstLegalUnit!=null?item.FirstLegalUnit: CustomsTariffsView.Where(m => m.HSCode == item.CustomsCode).FirstOrDefault()?.Unit1;
                    orderItem.SecondLegalUnit = item.SecondLegalUnit;
                    string place = item.PlaceOfProduction;
                    orderItem.PlaceOfProduction = CountriesView.Where(code => code.EditionOneCode == place || code.Code == place||code.Name==place).FirstOrDefault() == null ? Icgoo.UnknownCountry : CountriesView.Where(code => code.EditionOneCode == place || code.Code == place || code.Name == place).FirstOrDefault().Code;
                    orderItem.UnitPrice = item.UnitPrice.Value;
                    orderItem.TotalDeclarePrice = item.TotalDeclarePrice;
                    orderItem.Currency = only.Currency;
                    orderItem.Model = item.Model;
                    orderItem.Brand = item.Brand;
                    orderItem.IsInspection = (bool)item.IsInspection;
                    orderItem.InspFee = item.InspectionFee==null?0:item.InspectionFee.Value;
                    orderItem.IsCCC = item.IsCCC==null?false:(item.IsCCC.Value==1||item.IsCCC.Value==3)?true:false;
                    orderItem.IsSysForbid = false;
                    orderItem.CIQCode = item.CIQCode!=null?item.CIQCode:"999";
                    orderItem.TariffRate = item.TariffRate.Value;
                    orderItem.VauleAddedRate = item.ValueAddedTaxRate.Value;
                    orderItem.TaxCode = item.ModelInfoClassificationValue;
                    orderItem.TaxName = item.InvoiceProductName;

                    //var T_Order_PackDetail = (from c in db.T_Order_PackDetail
                    //                          where c.ModelID == item.ID
                    //                          select c).FirstOrDefault();

                    //if (T_Order_PackDetail != null)
                    //{
                    //    orderItem.NetWeight = T_Order_PackDetail.NetWeight;
                    //    var grossweight = (T_Order_PackDetail.NetWeight/0.7m)<0.02m?0.02m: (T_Order_PackDetail.NetWeight / 0.7m);
                    //    orderItem.GrossWeight = Math.Round(grossweight, 2);
                    //    var T_Order_Pack = (from c in db.T_Order_Pack
                    //                        where c.ID == T_Order_PackDetail.PackID
                    //                        select c).FirstOrDefault();

                    //    orderItem.PackNo = T_Order_Pack.PackNo;
                        
                    //}

                    //orderItem.CaseWeight = Convert.ToDecimal(row["毛重"].ToString());
                    //orderItem.SupplierName = row["付款公司"].ToString();                                       
                    //orderItem.DeclareCompany = row["报关公司"].ToString();

                    models.Add(orderItem);


                }

                var orderPacks = (from a in db.T_Order_Pack
                                  where a.OrderID == only.OrderID
                                  orderby a.PackNo ascending
                                  select new PackHistoryOnly
                                  {
                                      ID = a.ID,
                                      GrossWeight = a.GrossWeight.Value,
                                      BoxIndex = a.PackNo,
                                      Quantity = a.Quantity
                                  }).ToList();

                foreach (PackHistoryOnly orderPack in orderPacks)
                {
                    var packModels = (from a in db.T_Order_PackDetail
                                      join b in db.T_Order_Model on a.ModelID equals b.ID
                                      where a.PackID == orderPack.ID
                                      orderby b.DisplayOrder ascending
                                      select new PackItemHistoryOnly()
                                      {
                                          PackID = a.PackID,                                         
                                          Model = b.Model,                                       
                                          NetWeight = a.NetWeight,                                         
                                          Quantity = a.Quantity,                                         
                                          PlaceOfProduction = a.PlaceOfProduction,
                                          UnitPrice = b.UnitPrice,
                                          Brand = b.Brand,
                                          ModelID = b.ID,
                                      }).ToList();

                    //箱号相同，型号相同，合并去重
                    var temp = packModels.GroupBy(p => new { PackID = p.PackID, Model = p.Model, UnitPrice = p.UnitPrice, Brand = p.Brand, PlaceOfProduction = p.PlaceOfProduction, ModelID = p.ModelID })
                              .Select(m => new PackItemHistoryOnly
                              {
                                  PackID = m.Key.PackID,
                                  Model = m.Key.Model,
                                  UnitPrice = m.Key.UnitPrice,
                                  Brand = m.Key.Brand,
                                  ModelID = m.Key.ModelID,
                                  PlaceOfProduction = m.Key.PlaceOfProduction,
                                  Quantity = m.Sum(q => q.Quantity),
                                  NetWeight = m.Sum(q => q.NetWeight),
                              }).ToList();

                    temp.ForEach(t =>
                    {
                        var bef = packModels.Where(c => c.ModelID == t.ModelID).FirstOrDefault();
                        t.ModelID = bef.ModelID;
                        t.NetWeight = Math.Round(t.NetWeight, 2, MidpointRounding.AwayFromZero)<0.01M?0.01M: Math.Round(t.NetWeight, 2, MidpointRounding.AwayFromZero);
                        t.Brand = bef.Brand;                     
                    });

                    orderPack.PackItems = temp.ToList();                   
                }
                packs = orderPacks;
            }

            CreateOrder create = new CreateOrder();
            create.Create(models, only, packs);
        }
    }
}
