using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using Needs.Ccs.Services.Interfaces;
using Needs.Ccs.Services.Views;
using Needs.Linq;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq.SqlClient;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 代理订单
    /// </summary>
    public class IcgooOrder : Order
    {
        public string ConnectionString { get; set; }
        public List<PvOrderItems> EnterSpeed()
        {
            List<PvOrderItems> pvOrderItems = new List<PvOrderItems>();
            int StatusCode = (int)Enums.Status.Normal;
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                conn.Open();
                string OrderID = this.ID;

                /***** 开始一个事务 *****/
                //SqlTransaction tran = conn.BeginTransaction();

                #region 新增主订单
                SqlCommand sqlMainOrder = new SqlCommand("MainOrderPro", conn);
                sqlMainOrder.CommandType = CommandType.StoredProcedure;

                sqlMainOrder.Parameters.Add(new SqlParameter("@ID", SqlDbType.VarChar, 50));
                sqlMainOrder.Parameters.Add(new SqlParameter("@Type", SqlDbType.Int));
                sqlMainOrder.Parameters.Add(new SqlParameter("@AdminID", SqlDbType.VarChar, 50));
                sqlMainOrder.Parameters.Add(new SqlParameter("@UserID", SqlDbType.VarChar, 50));
                sqlMainOrder.Parameters.Add(new SqlParameter("@ClientID", SqlDbType.VarChar, 50));
                sqlMainOrder.Parameters.Add(new SqlParameter("@MainOrderStatus", SqlDbType.Int));
                sqlMainOrder.Parameters.Add(new SqlParameter("@PaymentStatus", SqlDbType.Int));
                sqlMainOrder.Parameters.Add(new SqlParameter("@PayExchangeStatus", SqlDbType.Int));
                sqlMainOrder.Parameters.Add(new SqlParameter("@Status", SqlDbType.Int));
                sqlMainOrder.Parameters.Add(new SqlParameter("@CreateDate", SqlDbType.DateTime));
                sqlMainOrder.Parameters.Add(new SqlParameter("@UpdateDate", SqlDbType.DateTime));
                sqlMainOrder.Parameters.Add(new SqlParameter("@Summary", SqlDbType.NChar, 500));

                sqlMainOrder.Parameters["@ID"].Value = this.MainOrderID;
                sqlMainOrder.Parameters["@Type"].Value = (int)this.Type;
                sqlMainOrder.Parameters["@AdminID"].Value = this.AdminID;
                sqlMainOrder.Parameters["@UserID"].Value = this.UserID;
                sqlMainOrder.Parameters["@ClientID"].Value = this.Client.ID;
                sqlMainOrder.Parameters["@MainOrderStatus"].Value = null;
                sqlMainOrder.Parameters["@PaymentStatus"].Value = null;
                sqlMainOrder.Parameters["@PayExchangeStatus"].Value = null;
                sqlMainOrder.Parameters["@Status"].Value = (int)this.Status;
                sqlMainOrder.Parameters["@CreateDate"].Value = this.CreateDate;
                sqlMainOrder.Parameters["@UpdateDate"].Value = this.UpdateDate;
                sqlMainOrder.Parameters["@Summary"].Value = "";

                sqlMainOrder.ExecuteNonQuery();
                #endregion

                #region 新增订单
                SqlCommand sqlCmd = new SqlCommand("OrderPro", conn);
                sqlCmd.CommandType = CommandType.StoredProcedure;

                sqlCmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.VarChar, 50));
                sqlCmd.Parameters.Add(new SqlParameter("@Type", SqlDbType.Int));
                sqlCmd.Parameters.Add(new SqlParameter("@AdminID", SqlDbType.VarChar, 50));
                sqlCmd.Parameters.Add(new SqlParameter("@UserID", SqlDbType.VarChar, 50));
                sqlCmd.Parameters.Add(new SqlParameter("@ClientID", SqlDbType.VarChar, 50));
                sqlCmd.Parameters.Add(new SqlParameter("@ClientAgreementID", SqlDbType.VarChar, 50));
                sqlCmd.Parameters.Add(new SqlParameter("@Currency", SqlDbType.VarChar, 50));
                sqlCmd.Parameters.Add(new SqlParameter("@CustomsExchangeRate", SqlDbType.Decimal));
                sqlCmd.Parameters.Add(new SqlParameter("@RealExchangeRate", SqlDbType.Decimal));
                sqlCmd.Parameters.Add(new SqlParameter("@IsFullVehicle", SqlDbType.Bit));
                sqlCmd.Parameters.Add(new SqlParameter("@IsLoan", SqlDbType.Bit));
                sqlCmd.Parameters.Add(new SqlParameter("@PackNo", SqlDbType.Int));
                sqlCmd.Parameters.Add(new SqlParameter("@WarpType", SqlDbType.VarChar, 2));
                sqlCmd.Parameters.Add(new SqlParameter("@DeclarePrice", SqlDbType.Decimal));
                sqlCmd.Parameters.Add(new SqlParameter("@PaidExchangeAmount", SqlDbType.Decimal));
                sqlCmd.Parameters.Add(new SqlParameter("@InvoiceStatus", SqlDbType.Int));
                sqlCmd.Parameters.Add(new SqlParameter("@IsHangUp", SqlDbType.Bit));
                sqlCmd.Parameters.Add(new SqlParameter("@OrderStatus", SqlDbType.Int));
                sqlCmd.Parameters.Add(new SqlParameter("@Status", SqlDbType.Int));
                sqlCmd.Parameters.Add(new SqlParameter("@CreateDate", SqlDbType.DateTime));
                sqlCmd.Parameters.Add(new SqlParameter("@UpdateDate", SqlDbType.DateTime));
                sqlCmd.Parameters.Add(new SqlParameter("@Summary", SqlDbType.NChar, 500));
                sqlCmd.Parameters.Add(new SqlParameter("@MainOrderID", SqlDbType.VarChar, 50));
                sqlCmd.Parameters.Add(new SqlParameter("@OrderBillType", SqlDbType.Int));

                sqlCmd.Parameters["@ID"].Value = OrderID;
                sqlCmd.Parameters["@Type"].Value = (int)this.Type;
                sqlCmd.Parameters["@AdminID"].Value = this.AdminID;
                sqlCmd.Parameters["@UserID"].Value = this.UserID;
                sqlCmd.Parameters["@ClientID"].Value = this.Client.ID;
                sqlCmd.Parameters["@ClientAgreementID"].Value = this.ClientAgreement.ID;
                sqlCmd.Parameters["@Currency"].Value = this.Currency;
                sqlCmd.Parameters["@CustomsExchangeRate"].Value = this.CustomsExchangeRate;
                sqlCmd.Parameters["@RealExchangeRate"].Value = this.RealExchangeRate;
                sqlCmd.Parameters["@IsFullVehicle"].Value = this.IsFullVehicle;
                sqlCmd.Parameters["@IsLoan"].Value = this.IsLoan;
                sqlCmd.Parameters["@PackNo"].Value = this.PackNo;
                sqlCmd.Parameters["@WarpType"].Value = this.WarpType;
                sqlCmd.Parameters["@DeclarePrice"].Value = this.DeclarePrice;
                sqlCmd.Parameters["@PaidExchangeAmount"].Value = this.PaidExchangeAmount;
                sqlCmd.Parameters["@InvoiceStatus"].Value = this.InvoiceStatus;
                sqlCmd.Parameters["@IsHangUp"].Value = this.IsHangUp;
                sqlCmd.Parameters["@OrderStatus"].Value = (int)this.OrderStatus;
                sqlCmd.Parameters["@Status"].Value = (int)this.Status;
                sqlCmd.Parameters["@CreateDate"].Value = this.CreateDate;
                sqlCmd.Parameters["@UpdateDate"].Value = this.UpdateDate;
                sqlCmd.Parameters["@Summary"].Value = "";
                sqlCmd.Parameters["@MainOrderID"].Value = this.MainOrderID;
                sqlCmd.Parameters["@OrderBillType"].Value = this.OrderBillType;

                //sqlCmd.Transaction = tran;

                sqlCmd.ExecuteNonQuery();
                #endregion

                #region 保存香港交货信息
                SqlCommand sqlHK = new SqlCommand("OrderConsigneesPro", conn);
                sqlHK.CommandType = CommandType.StoredProcedure;

                sqlHK.Parameters.Add(new SqlParameter("@ID", SqlDbType.VarChar, 50));
                sqlHK.Parameters.Add(new SqlParameter("@OrderID", SqlDbType.VarChar, 50));
                sqlHK.Parameters.Add(new SqlParameter("@ClientSupplierID", SqlDbType.VarChar, 50));
                sqlHK.Parameters.Add(new SqlParameter("@Type", SqlDbType.Int));
                sqlHK.Parameters.Add(new SqlParameter("@Contact", SqlDbType.VarChar, 150));
                sqlHK.Parameters.Add(new SqlParameter("@Mobile", SqlDbType.VarChar, 50));
                sqlHK.Parameters.Add(new SqlParameter("@Tel", SqlDbType.VarChar, 50));
                sqlHK.Parameters.Add(new SqlParameter("@Address", SqlDbType.VarChar, 250));
                sqlHK.Parameters.Add(new SqlParameter("@PickUpTime", SqlDbType.DateTime));
                sqlHK.Parameters.Add(new SqlParameter("@WayBillNo", SqlDbType.VarChar, 50));
                sqlHK.Parameters.Add(new SqlParameter("@Status", SqlDbType.Int));
                sqlHK.Parameters.Add(new SqlParameter("@CreateDate", SqlDbType.DateTime));
                sqlHK.Parameters.Add(new SqlParameter("@UpdateDate", SqlDbType.DateTime));
                sqlHK.Parameters.Add(new SqlParameter("@Summary", SqlDbType.NChar, 500));

                sqlHK.Parameters["@ID"].Value = this.OrderConsignee.ID;
                sqlHK.Parameters["@OrderID"].Value = OrderID;
                sqlHK.Parameters["@ClientSupplierID"].Value = this.OrderConsignee.ClientSupplier.ID;
                sqlHK.Parameters["@Type"].Value = (int)this.OrderConsignee.Type;
                sqlHK.Parameters["@Contact"].Value = this.OrderConsignee.Contact;
                sqlHK.Parameters["@Mobile"].Value = this.OrderConsignee.Mobile;
                sqlHK.Parameters["@Tel"].Value = this.OrderConsignee.Tel;
                sqlHK.Parameters["@Address"].Value = this.OrderConsignee.Address;
                sqlHK.Parameters["@PickUpTime"].Value = this.OrderConsignee.PickUpTime;
                sqlHK.Parameters["@WayBillNo"].Value = this.OrderConsignee.WayBillNo;
                sqlHK.Parameters["@Status"].Value = (int)this.OrderConsignee.Status;
                sqlHK.Parameters["@CreateDate"].Value = this.CreateDate;
                sqlHK.Parameters["@UpdateDate"].Value = this.UpdateDate;
                sqlHK.Parameters["@Summary"].Value = "";

                //sqlHK.Transaction = tran;
                sqlHK.ExecuteNonQuery();
                #endregion

                #region 保存国内交货信息
                if (this.OrderConsignor.Contact != null)
                {
                    SqlCommand sqlDomestic = new SqlCommand("OrderConsignorsPro", conn);
                    sqlDomestic.CommandType = CommandType.StoredProcedure;

                    sqlDomestic.Parameters.Add(new SqlParameter("@ID", SqlDbType.VarChar, 50));
                    sqlDomestic.Parameters.Add(new SqlParameter("@OrderID", SqlDbType.VarChar, 50));
                    sqlDomestic.Parameters.Add(new SqlParameter("@Type", SqlDbType.Int));
                    sqlDomestic.Parameters.Add(new SqlParameter("@Name", SqlDbType.VarChar, 200));
                    sqlDomestic.Parameters.Add(new SqlParameter("@Contact", SqlDbType.VarChar, 150));
                    sqlDomestic.Parameters.Add(new SqlParameter("@Mobile", SqlDbType.VarChar, 50));
                    sqlDomestic.Parameters.Add(new SqlParameter("@Tel", SqlDbType.VarChar, 50));
                    sqlDomestic.Parameters.Add(new SqlParameter("@Address", SqlDbType.VarChar, 250));
                    sqlDomestic.Parameters.Add(new SqlParameter("@IDType", SqlDbType.VarChar, 50));
                    sqlDomestic.Parameters.Add(new SqlParameter("@IDNumber", SqlDbType.VarChar, 50));
                    sqlDomestic.Parameters.Add(new SqlParameter("@Status", SqlDbType.Int));
                    sqlDomestic.Parameters.Add(new SqlParameter("@CreateDate", SqlDbType.DateTime));
                    sqlDomestic.Parameters.Add(new SqlParameter("@UpdateDate", SqlDbType.DateTime));
                    sqlDomestic.Parameters.Add(new SqlParameter("@Summary", SqlDbType.NChar, 500));

                    sqlDomestic.Parameters["@ID"].Value = this.OrderConsignor.ID;
                    sqlDomestic.Parameters["@OrderID"].Value = OrderID;
                    sqlDomestic.Parameters["@Type"].Value = (int)this.OrderConsignor.Type;
                    sqlDomestic.Parameters["@Name"].Value = this.OrderConsignor.Name;
                    sqlDomestic.Parameters["@Contact"].Value = this.OrderConsignor.Contact;
                    sqlDomestic.Parameters["@Mobile"].Value = this.OrderConsignor.Mobile;
                    sqlDomestic.Parameters["@Tel"].Value = this.OrderConsignor.Tel;
                    sqlDomestic.Parameters["@Address"].Value = this.OrderConsignor.Address;
                    sqlDomestic.Parameters["@IDType"].Value = this.OrderConsignor.IDType;
                    sqlDomestic.Parameters["@IDNumber"].Value = this.OrderConsignor.IDNumber;
                    sqlDomestic.Parameters["@Status"].Value = (int)this.OrderConsignor.Status;
                    sqlDomestic.Parameters["@CreateDate"].Value = this.CreateDate;
                    sqlDomestic.Parameters["@UpdateDate"].Value = this.UpdateDate;
                    sqlDomestic.Parameters["@Summary"].Value = "";

                    //sqlDomestic.Transaction = tran;
                    sqlDomestic.ExecuteNonQuery();
                }
                #endregion

                #region 付汇供应商
                foreach (var supplier in this.PayExchangeSuppliers)
                {
                    SqlCommand sqlDomestic = new SqlCommand("OrderPayExchangeSuppliersPro", conn);
                    sqlDomestic.CommandType = CommandType.StoredProcedure;

                    sqlDomestic.Parameters.Add(new SqlParameter("@ID", SqlDbType.VarChar, 50));
                    sqlDomestic.Parameters.Add(new SqlParameter("@OrderID", SqlDbType.VarChar, 50));
                    sqlDomestic.Parameters.Add(new SqlParameter("@ClientSupplierID", SqlDbType.VarChar, 50));
                    sqlDomestic.Parameters.Add(new SqlParameter("@Status", SqlDbType.Int));
                    sqlDomestic.Parameters.Add(new SqlParameter("@CreateDate", SqlDbType.DateTime));
                    sqlDomestic.Parameters.Add(new SqlParameter("@UpdateDate", SqlDbType.DateTime));
                    sqlDomestic.Parameters.Add(new SqlParameter("@Summary", SqlDbType.NChar, 500));

                    sqlDomestic.Parameters["@ID"].Value = supplier.ID;
                    sqlDomestic.Parameters["@OrderID"].Value = OrderID;
                    sqlDomestic.Parameters["@ClientSupplierID"].Value = supplier.ClientSupplier.ID;
                    sqlDomestic.Parameters["@Status"].Value = supplier.Status;
                    sqlDomestic.Parameters["@CreateDate"].Value = DateTime.Now;
                    sqlDomestic.Parameters["@UpdateDate"].Value = DateTime.Now;
                    sqlDomestic.Parameters["@Summary"].Value = "";

                    //sqlDomestic.Transaction = tran;
                    sqlDomestic.ExecuteNonQuery();
                }
                #endregion

                #region 保存订单项
                #region OrderItems
                DataTable dtitem = new DataTable();
                dtitem.Columns.Add("ID");
                dtitem.Columns.Add("OrderID");
                dtitem.Columns.Add("Origin");
                dtitem.Columns.Add("Quantity");
                dtitem.Columns.Add("DeclaredQuantity");
                dtitem.Columns.Add("Unit");
                dtitem.Columns.Add("UnitPrice");
                dtitem.Columns.Add("TotalPrice");
                dtitem.Columns.Add("GrossWeight");
                dtitem.Columns.Add("IsSampllingCheck");
                dtitem.Columns.Add("ClassifyStatus");
                dtitem.Columns.Add("ProductUniqueCode");
                dtitem.Columns.Add("Status");
                dtitem.Columns.Add("CreateDate");
                dtitem.Columns.Add("UpdateDate");
                dtitem.Columns.Add("Summary");
                dtitem.Columns.Add("Name");
                dtitem.Columns.Add("Model");
                dtitem.Columns.Add("Manufacturer");
                dtitem.Columns.Add("Batch");

                #endregion

                #region OrderItemCategories
                DataTable dtCategory = new DataTable();
                dtCategory.Columns.Add("ID");
                dtCategory.Columns.Add("OrderItemID");
                dtCategory.Columns.Add("Type");
                dtCategory.Columns.Add("TaxCode");
                dtCategory.Columns.Add("TaxName");
                dtCategory.Columns.Add("HSCode");
                dtCategory.Columns.Add("Name");
                dtCategory.Columns.Add("Elements");
                dtCategory.Columns.Add("Unit1");
                dtCategory.Columns.Add("Unit2");
                dtCategory.Columns.Add("Qty1");
                dtCategory.Columns.Add("Qty2");
                dtCategory.Columns.Add("CIQCode");
                dtCategory.Columns.Add("ClassifyFirstOperator");
                dtCategory.Columns.Add("ClassifySecondOperator");
                dtCategory.Columns.Add("Status");
                dtCategory.Columns.Add("CreateDate");
                dtCategory.Columns.Add("UpdateDate");
                dtCategory.Columns.Add("Summary");
                #endregion

                #region OrderItemTaxes
                DataTable dtTax = new DataTable();
                dtTax.Columns.Add("ID");
                dtTax.Columns.Add("OrderItemID");
                dtTax.Columns.Add("Type");
                dtTax.Columns.Add("Rate");
                dtTax.Columns.Add("ReceiptRate");
                dtTax.Columns.Add("Value");
                dtTax.Columns.Add("Status");
                dtTax.Columns.Add("CreateDate");
                dtTax.Columns.Add("UpdateDate");
                dtTax.Columns.Add("Summary");
                #endregion

                #region OrderPremiums
                DataTable dtPremiums = new DataTable();
                dtPremiums.Columns.Add("ID");
                dtPremiums.Columns.Add("OrderID");
                dtPremiums.Columns.Add("OrderItemID");
                dtPremiums.Columns.Add("Type");
                dtPremiums.Columns.Add("Name");
                dtPremiums.Columns.Add("Count");
                dtPremiums.Columns.Add("UnitPrice");
                dtPremiums.Columns.Add("Currency");
                dtPremiums.Columns.Add("Rate");
                dtPremiums.Columns.Add("AdminID");
                dtPremiums.Columns.Add("Status");
                dtPremiums.Columns.Add("CreateDate");
                dtPremiums.Columns.Add("UpdateDate");
                dtPremiums.Columns.Add("Summary");
                #endregion


                Dictionary<string, bool> specialType = new Dictionary<string, bool>();
                bool IsQuarantine = false, IsInsp = false, IsHighValue = false,IsCCC = false;
                PvOrderItems pvOrderItem;
                foreach (var classify in this.ClassifyProducts)
                {
                    pvOrderItem = new PvOrderItems();
                    #region 订单项
                    DataRow dr = dtitem.NewRow();
                    var prefix = System.Configuration.ConfigurationManager.AppSettings["Purchaser"];
                    string OrderItemID = prefix + Needs.Overall.PKeySigner.Pick(PKeyType.OrderItem);
                    dr[0] = OrderItemID;
                    dr[1] = OrderID;
                    dr[2] = classify.Origin;
                    dr[3] = classify.Quantity;
                    dr[4] = classify.DeclaredQuantity;
                    dr[5] = classify.Unit;
                    dr[6] = classify.UnitPrice;
                    dr[7] = classify.TotalPrice;
                    dr[8] = classify.GrossWeight;
                    dr[9] = classify.IsSampllingCheck;
                    dr[10] = (int)classify.ClassifyStatus;
                    dr[11] = classify.ProductUniqueCode;
                    dr[12] = (int)classify.Status;
                    dr[13] = DateTime.Now;
                    dr[14] = DateTime.Now;
                    dr[15] = null;
                    dr[16] = classify.Name;
                    dr[17] = classify.Model;
                    dr[18] = classify.Manufacturer;
                    dr[19] = classify.Batch;

                    dtitem.Rows.Add(dr);
                    #endregion

                    #region 接口信息
                    pvOrderItem.ID = OrderItemID;
                    pvOrderItem.TinyOrderID = OrderID;
                    pvOrderItem.Model = classify.Model;
                    pvOrderItem.Brand = classify.Manufacturer;
                    pvOrderItem.Origin = classify.Origin;
                    pvOrderItem.UnitPrice = classify.UnitPrice;
                    pvOrderItem.TotalPrice = classify.TotalPrice;
                    pvOrderItem.Unit = classify.Unit;
                    pvOrderItem.Quantity = classify.Quantity;
                    pvOrderItem.Currency = this.Currency;
                    pvOrderItem.ProductUnicode = classify.ProductUniqueCode;
                    pvOrderItem.GrossWeight = classify.GrossWeight;
                    pvOrderItem.ClassifyStatus = classify.ClassifyStatus;
                    pvOrderItem.ClassifyTime = classify.UpdateDate;
                    pvOrderItem.DateCode = classify.Batch;
                    if (!string.IsNullOrEmpty(classify.CaseNo))
                    {
                        pvOrderItem.PackNo = classify.CaseNo;
                    }
                    #endregion

                    if (classify.ClassifyStatus == ClassifyStatus.Done || classify.ClassifyStatus == ClassifyStatus.First)
                    {
                        #region 增加归类信息
                        DataRow drCategory = dtCategory.NewRow();
                        drCategory[0] = string.Concat(dr[0]).MD5();
                        drCategory[1] = dr[0];
                        drCategory[2] = (int)classify.Category.Type;
                        drCategory[3] = classify.Category.TaxCode;
                        drCategory[4] = classify.Category.TaxName;
                        drCategory[5] = classify.Category.HSCode;
                        drCategory[6] = classify.Category.Name;
                        drCategory[7] = classify.Category.Elements;
                        drCategory[8] = classify.Category.Unit1;
                        drCategory[9] = classify.Category.Unit2;
                        drCategory[10] = classify.Category.Qty1;
                        drCategory[11] = classify.Category.Qty2;
                        drCategory[12] = classify.Category.CIQCode;
                        drCategory[13] = classify.Category.ClassifyFirstOperator == null ? null : classify.Category.ClassifyFirstOperator.ID;
                        drCategory[14] = classify.Category.ClassifySecondOperator == null ? null : classify.Category.ClassifySecondOperator.ID;
                        drCategory[15] = (int)classify.Category.Status;
                        drCategory[16] = this.CreateDate;
                        drCategory[17] = this.UpdateDate;
                        drCategory[18] = "";

                        dtCategory.Rows.Add(drCategory);
                        #endregion

                        #region 关税
                        DataRow drTariff = dtTax.NewRow();
                        drTariff[0] = string.Concat(dr[0], CustomsRateType.ImportTax).MD5();
                        drTariff[1] = dr[0];
                        drTariff[2] = (int)CustomsRateType.ImportTax;
                        drTariff[3] = classify.ImportTax.Rate;
                        drTariff[4] = classify.ImportTax.Rate;
                        drTariff[5] = classify.ImportTax.Value;
                        drTariff[6] = StatusCode;
                        drTariff[7] = this.CreateDate;
                        drTariff[8] = this.UpdateDate;
                        drTariff[9] = "";

                        dtTax.Rows.Add(drTariff);
                        #endregion

                        #region 增值税
                        DataRow drValueAdded = dtTax.NewRow();
                        drValueAdded[0] = string.Concat(dr[0], CustomsRateType.AddedValueTax).MD5();
                        drValueAdded[1] = dr[0];
                        drValueAdded[2] = (int)CustomsRateType.AddedValueTax;
                        drValueAdded[3] = classify.AddedValueTax.Rate;
                        drValueAdded[4] = classify.AddedValueTax.Rate;
                        drValueAdded[5] = classify.AddedValueTax.Value;
                        drValueAdded[6] = StatusCode;
                        drValueAdded[7] = this.CreateDate;
                        drValueAdded[8] = this.UpdateDate;
                        drValueAdded[9] = "";

                        dtTax.Rows.Add(drValueAdded);
                        #endregion

                        #region 消费税
                        DataRow drExciseTax = dtTax.NewRow();
                        drExciseTax[0] = string.Concat(dr[0], CustomsRateType.ConsumeTax).MD5();
                        drExciseTax[1] = dr[0];
                        drExciseTax[2] = (int)CustomsRateType.ConsumeTax;
                        drExciseTax[3] = classify.ExciseTax?.Rate ?? 0m;
                        drExciseTax[4] = classify.ExciseTax?.Rate ?? 0m;
                        drExciseTax[5] = classify.ExciseTax?.Value;
                        drExciseTax[6] = StatusCode;
                        drExciseTax[7] = this.CreateDate;
                        drExciseTax[8] = this.UpdateDate;
                        drExciseTax[9] = "";

                        dtTax.Rows.Add(drExciseTax);
                        #endregion

                        #region 商检费
                        if (classify.IsInsp)
                        {
                            DataRow drFee = dtPremiums.NewRow();
                            drFee[0] = Needs.Overall.PKeySigner.Pick(PKeyType.OrderPremium);
                            drFee[1] = this.ID;
                            drFee[2] = dr[0];
                            drFee[3] = (int)Enums.OrderPremiumType.InspectionFee;
                            drFee[4] = null;
                            drFee[5] = 1;
                            drFee[6] = classify.InspectionFee;
                            drFee[7] = MultiEnumUtils.ToCode<Enums.Currency>(Enums.Currency.CNY);
                            drFee[8] = 1;
                            drFee[9] = this.AdminID;
                            drFee[10] = StatusCode;
                            drFee[11] = this.CreateDate;
                            drFee[12] = this.UpdateDate;
                            drFee[13] = null;

                            dtPremiums.Rows.Add(drFee);
                        }
                        #endregion

                        #region 税则归类记录
                        SqlCommand sqlProCategory = new SqlCommand("ProductCategoriesPro", conn);
                        sqlProCategory.CommandType = CommandType.StoredProcedure;

                        sqlProCategory.Parameters.Add(new SqlParameter("@ID", SqlDbType.VarChar, 50));
                        sqlProCategory.Parameters.Add(new SqlParameter("@Model", SqlDbType.VarChar, 50));
                        sqlProCategory.Parameters.Add(new SqlParameter("@Name", SqlDbType.VarChar, 150));
                        sqlProCategory.Parameters.Add(new SqlParameter("@HSCode", SqlDbType.VarChar, 50));
                        sqlProCategory.Parameters.Add(new SqlParameter("@Elements", SqlDbType.VarChar, 500));
                        sqlProCategory.Parameters.Add(new SqlParameter("@CreateDate", SqlDbType.DateTime));
                        sqlProCategory.Parameters.Add(new SqlParameter("@TariffRate", SqlDbType.VarChar, 50));
                        sqlProCategory.Parameters.Add(new SqlParameter("@AddedValueRate", SqlDbType.VarChar, 150));
                        sqlProCategory.Parameters.Add(new SqlParameter("@UnitPrice", SqlDbType.VarChar, 50));
                        sqlProCategory.Parameters.Add(new SqlParameter("@InspectionFee", SqlDbType.VarChar, 500));
                        sqlProCategory.Parameters.Add(new SqlParameter("@Quantity", SqlDbType.Decimal));
                        sqlProCategory.Parameters.Add(new SqlParameter("@AdminID", SqlDbType.VarChar, 50));

                        sqlProCategory.Parameters["@ID"].Value = string.Concat(classify.Category.Name, classify.Model, classify.Category.HSCode).MD5();
                        sqlProCategory.Parameters["@Model"].Value = classify.Model;
                        sqlProCategory.Parameters["@Name"].Value = classify.Category.Name;
                        sqlProCategory.Parameters["@HSCode"].Value = classify.Category.HSCode;
                        sqlProCategory.Parameters["@Elements"].Value = classify.Category.Elements;
                        sqlProCategory.Parameters["@CreateDate"].Value = DateTime.Now;
                        sqlProCategory.Parameters["@TariffRate"].Value = classify.ImportTax.Rate;
                        sqlProCategory.Parameters["@AddedValueRate"].Value = classify.AddedValueTax.Rate;
                        sqlProCategory.Parameters["@UnitPrice"].Value = classify.UnitPrice;
                        sqlProCategory.Parameters["@InspectionFee"].Value = classify.IsInsp ? (decimal?)classify.InspectionFee : null;
                        sqlProCategory.Parameters["@Quantity"].Value = classify.Quantity;
                        sqlProCategory.Parameters["@AdminID"].Value = Icgoo.DefaultCreator;

                        sqlProCategory.ExecuteNonQuery();

                        #endregion

                        #region 税务归类记录
                        SqlCommand sqlTaxCategory = new SqlCommand("ProductTaxCategoriesPro", conn);
                        sqlTaxCategory.CommandType = CommandType.StoredProcedure;

                        sqlTaxCategory.Parameters.Add(new SqlParameter("@ID", SqlDbType.VarChar, 50));
                        sqlTaxCategory.Parameters.Add(new SqlParameter("@Name", SqlDbType.VarChar, 150));
                        sqlTaxCategory.Parameters.Add(new SqlParameter("@Model", SqlDbType.VarChar, 150));
                        sqlTaxCategory.Parameters.Add(new SqlParameter("@TaxCode", SqlDbType.VarChar, 50));
                        sqlTaxCategory.Parameters.Add(new SqlParameter("@TaxName", SqlDbType.VarChar, 50));
                        sqlTaxCategory.Parameters.Add(new SqlParameter("@CreateTime", SqlDbType.DateTime));

                        sqlTaxCategory.Parameters["@ID"].Value = string.Concat(classify.Category.Name, classify.Category.TaxCode, classify.Category.TaxName).MD5();
                        sqlTaxCategory.Parameters["@Name"].Value = classify.Category.Name;
                        sqlTaxCategory.Parameters["@Model"].Value = classify.Model;
                        sqlTaxCategory.Parameters["@TaxCode"].Value = classify.Category.TaxCode;
                        sqlTaxCategory.Parameters["@TaxName"].Value = classify.Category.TaxName;
                        sqlTaxCategory.Parameters["@CreateTime"].Value = this.CreateDate;

                        sqlTaxCategory.ExecuteNonQuery();

                        #endregion

                        if ((classify.Category.Type & ItemCategoryType.Quarantine) > 0)
                        {
                            IsQuarantine = true;
                        }
                        if ((classify.Category.Type & ItemCategoryType.Inspection) > 0)
                        {
                            IsInsp = true;
                        }
                        if ((classify.Category.Type & ItemCategoryType.HighValue) > 0)
                        {
                            IsHighValue = true;
                        }
                        if ((classify.Category.Type & ItemCategoryType.CCC) > 0)
                        {
                            IsCCC = true;
                        }

                        #region 接口信息
                        pvOrderItem.ProductName = classify.Category.Name;
                        pvOrderItem.CustomsCode = classify.Category.HSCode;
                        pvOrderItem.CIQCode = classify.Category.CIQCode;
                        pvOrderItem.Elements = classify.Category.Elements;
                        pvOrderItem.TaxCode = classify.Category.TaxCode;
                        pvOrderItem.TaxName = classify.Category.TaxName;
                        pvOrderItem.TariffRate = classify.ImportTax.Rate;
                        pvOrderItem.ValueAddedRate = classify.AddedValueTax.Rate;
                        pvOrderItem.ExciseTaxRate = classify.ExciseTax.Rate;
                        pvOrderItem.FirstLegalUnit = classify.Category.Unit1;
                        pvOrderItem.SecondLegalUnit = classify.Category.Unit2;
                        pvOrderItem.CCC = (classify.Category.Type & ItemCategoryType.CCC) > 0 ? true : false;
                        pvOrderItem.Embargo = (classify.Category.Type & ItemCategoryType.Forbid) > 0 ? true : false;
                        pvOrderItem.HkControl = (classify.Category.Type & ItemCategoryType.HKForbid) > 0 ? true : false;
                        pvOrderItem.Coo = (classify.Category.Type & ItemCategoryType.OriginProof) > 0 ? true : false;
                        pvOrderItem.CIQ = (classify.Category.Type & ItemCategoryType.Inspection) > 0 ? true : false;
                        pvOrderItem.CIQprice = classify.InspectionFee;
                        pvOrderItem.IsHighPrice = (classify.Category.Type & ItemCategoryType.HighValue) > 0 ? true : false;
                        pvOrderItem.IsDisinfected = (classify.Category.Type & ItemCategoryType.Quarantine) > 0 ? true : false;
                        pvOrderItem.ClassifySecondAdminID = classify.Category.ClassifySecondOperator == null ? null : classify.Category.ClassifySecondOperator.ID;
                        #endregion
                    }

                    pvOrderItems.Add(pvOrderItem);
                }

                specialType.Add("IsQuarantine", IsQuarantine);
                specialType.Add("IsInsp", IsInsp);
                specialType.Add("IsHighValue", IsHighValue);
                specialType.Add("IsCCC", IsCCC);

                #region 商检，检疫
                foreach (KeyValuePair<string, bool> special in specialType)
                {
                    if (special.Value)
                    {
                        SqlCommand sqlSpecial = new SqlCommand("OrderVoyagesPro", conn);
                        sqlSpecial.CommandType = CommandType.StoredProcedure;

                        sqlSpecial.Parameters.Add(new SqlParameter("@ID", SqlDbType.VarChar, 50));
                        sqlSpecial.Parameters.Add(new SqlParameter("@OrderID", SqlDbType.VarChar, 50));
                        sqlSpecial.Parameters.Add(new SqlParameter("@Type", SqlDbType.Int));
                        sqlSpecial.Parameters.Add(new SqlParameter("@Status", SqlDbType.Int));
                        sqlSpecial.Parameters.Add(new SqlParameter("@CreateDate", SqlDbType.DateTime));
                        sqlSpecial.Parameters.Add(new SqlParameter("@UpdateDate", SqlDbType.DateTime));
                        sqlSpecial.Parameters.Add(new SqlParameter("@Summary", SqlDbType.NChar, 500));


                        sqlSpecial.Parameters["@OrderID"].Value = OrderID;
                        sqlSpecial.Parameters["@Status"].Value = Status.Normal;
                        sqlSpecial.Parameters["@CreateDate"].Value = this.CreateDate;
                        sqlSpecial.Parameters["@UpdateDate"].Value = this.UpdateDate;
                        sqlSpecial.Parameters["@Summary"].Value = "";

                        string typespecial = special.Key;

                        switch (typespecial)
                        {
                            case "IsQuarantine":
                                sqlSpecial.Parameters["@ID"].Value = string.Concat(OrderID, OrderSpecialType.Quarantine).MD5();
                                sqlSpecial.Parameters["@Type"].Value = OrderSpecialType.Quarantine;
                                break;

                            case "IsInsp":
                                sqlSpecial.Parameters["@ID"].Value = string.Concat(OrderID, OrderSpecialType.Inspection).MD5();
                                sqlSpecial.Parameters["@Type"].Value = OrderSpecialType.Inspection;
                                break;

                            case "IsHighValue":
                                sqlSpecial.Parameters["@ID"].Value = string.Concat(OrderID, OrderSpecialType.HighValue).MD5();
                                sqlSpecial.Parameters["@Type"].Value = OrderSpecialType.HighValue;
                                break;

                            case "IsCCC":
                                sqlSpecial.Parameters["@ID"].Value = string.Concat(OrderID, OrderSpecialType.CCC).MD5();
                                sqlSpecial.Parameters["@Type"].Value = OrderSpecialType.CCC;
                                break;
                        }

                        sqlSpecial.ExecuteNonQuery();
                    }
                }
                #endregion
              
                SqlBulkCopy bulkCopy = new SqlBulkCopy(conn);
                bulkCopy.DestinationTableName = "OrderItems";
                bulkCopy.BatchSize = dtitem.Rows.Count;
                bulkCopy.WriteToServer(dtitem);

                if (dtCategory.Rows.Count > 0)
                {
                    //SqlBulkCopy bulkCategory = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, tran);
                    SqlBulkCopy bulkCategory = new SqlBulkCopy(conn);
                    bulkCategory.DestinationTableName = "OrderItemCategories";
                    bulkCategory.BatchSize = dtCategory.Rows.Count;
                    bulkCategory.WriteToServer(dtCategory);
                }

                if (dtTax.Rows.Count > 0)
                {
                    //SqlBulkCopy bulkTax = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, tran);
                    SqlBulkCopy bulkTax = new SqlBulkCopy(conn);
                    bulkTax.DestinationTableName = "OrderItemTaxes";
                    bulkTax.BatchSize = dtTax.Rows.Count;
                    bulkTax.WriteToServer(dtTax);
                }

                if (dtPremiums.Rows.Count > 0)
                {
                    //SqlBulkCopy bulkPremiums = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, tran);
                    SqlBulkCopy bulkPremiums = new SqlBulkCopy(conn);
                    bulkPremiums.DestinationTableName = "OrderPremiums";
                    bulkPremiums.BatchSize = dtPremiums.Rows.Count;
                    bulkPremiums.WriteToServer(dtPremiums);
                }
                
                #endregion

                //try
                //{
                //    tran.Commit();
                //}
                //catch (Exception ex)
                //{
                //    tran.Rollback();
                //}

                conn.Close();
            }
            return pvOrderItems;

        }
        public void GenerateBillSpeed()
        {
            var con = System.Configuration.ConfigurationManager.ConnectionStrings["foricScCustomsConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(con))
            {
                conn.Open();
                var order = this;

                //获取海关汇率和实时汇率
                decimal customsExchangeRate, realExchangeRate;
                if (order.CustomsExchangeRate != null && order.RealExchangeRate != null)
                {
                    customsExchangeRate = order.CustomsExchangeRate.Value;
                    realExchangeRate = order.RealExchangeRate.Value;
                }
                else
                {
                    if (order.Currency == MultiEnumUtils.ToCode<Enums.Currency>(Enums.Currency.CNY))   //  "CNY")
                    {
                        customsExchangeRate = 1;
                        realExchangeRate = 1;
                    }
                    else
                    {
                        var customsExchange = new Views.CustomExchangeRatesView(this.Currency).ToRate();
                        var realExchange = new Views.RealTimeExchangeRatesView(this.Currency).ToRate();

                        if (customsExchange == null || string.IsNullOrEmpty(customsExchange.ID))
                        {
                            throw new Exception("请检查交易币种的海关汇率是否正确配置！");
                        }
                        if (realExchange == null || string.IsNullOrEmpty(realExchange.ID))
                        {
                            throw new Exception("请检查交易币种的实时汇率是否正确配置！");
                        }

                        customsExchangeRate = customsExchange.Rate;
                        realExchangeRate = realExchange.Rate;
                    }
                }

                //代理费汇率类型
                decimal agentRate = 0;
                switch (order.ClientAgreement.AgencyFeeClause.ExchangeRateType)
                {
                    case Enums.ExchangeRateType.RealTime:
                        agentRate = realExchangeRate;
                        break;
                    case Enums.ExchangeRateType.Custom:
                        agentRate = customsExchangeRate;
                        break;
                    case Enums.ExchangeRateType.Agreed:
                        agentRate = order.ClientAgreement.AgencyFeeClause.ExchangeRateValue.HasValue ? order.ClientAgreement.AgencyFeeClause.ExchangeRateValue.Value : 0;
                        break;
                    default:
                        break;
                }

                //计算代理费
                var totalAgency = order.DeclarePrice * agentRate * order.ClientAgreement.AgencyRate;
                var minAgencyFee = order.ClientAgreement.MinAgencyFee;
                var agencyFee = totalAgency < minAgencyFee ? minAgencyFee : totalAgency.ToRound(4);
                var agency = order.Premiums.Where(t => t.Type == Enums.OrderPremiumType.AgencyFee).FirstOrDefault() ?? new OrderPremium();
                agency.OrderID = order.ID;
                agency.Type = Enums.OrderPremiumType.AgencyFee;
                agency.Admin = order.Client.Merchandiser;
                agency.Count = 1;
                agency.UnitPrice = agencyFee;
                agency.Currency = MultiEnumUtils.ToCode<Enums.Currency>(Enums.Currency.CNY);
                agency.Rate = 1;
                if (agency.ID == null)
                {
                    agency.ID = Needs.Overall.PKeySigner.Pick(PKeyType.OrderPremium);
                }

                #region 更新代理费
                SqlCommand sqlFee = new SqlCommand("OrderPremiumsPro", conn);
                sqlFee.CommandType = CommandType.StoredProcedure;

                sqlFee.Parameters.Add(new SqlParameter("@ID", SqlDbType.VarChar, 50));
                sqlFee.Parameters.Add(new SqlParameter("@OrderID", SqlDbType.VarChar, 50));
                sqlFee.Parameters.Add(new SqlParameter("@Type", SqlDbType.Int));
                sqlFee.Parameters.Add(new SqlParameter("@Name", SqlDbType.VarChar, 150));
                sqlFee.Parameters.Add(new SqlParameter("@Count", SqlDbType.Int));
                sqlFee.Parameters.Add(new SqlParameter("@UnitPrice", SqlDbType.Decimal));
                sqlFee.Parameters.Add(new SqlParameter("@Currency", SqlDbType.VarChar, 50));
                sqlFee.Parameters.Add(new SqlParameter("@Rate", SqlDbType.Decimal));
                sqlFee.Parameters.Add(new SqlParameter("@AdminID", SqlDbType.VarChar, 50));
                sqlFee.Parameters.Add(new SqlParameter("@Status", SqlDbType.Int));
                sqlFee.Parameters.Add(new SqlParameter("@CreateDate", SqlDbType.DateTime));
                sqlFee.Parameters.Add(new SqlParameter("@UpdateDate", SqlDbType.DateTime));
                sqlFee.Parameters.Add(new SqlParameter("@Summary", SqlDbType.VarChar, 300));

                sqlFee.Parameters["@ID"].Value = agency.ID;
                sqlFee.Parameters["@OrderID"].Value = agency.OrderID;
                sqlFee.Parameters["@Type"].Value = (int)agency.Type;
                sqlFee.Parameters["@Name"].Value = agency.Name;
                sqlFee.Parameters["@Count"].Value = agency.Count;
                sqlFee.Parameters["@UnitPrice"].Value = agency.UnitPrice;
                sqlFee.Parameters["@Currency"].Value = agency.Currency;
                sqlFee.Parameters["@Rate"].Value = agency.Rate;
                sqlFee.Parameters["@AdminID"].Value = agency.Admin.ID;
                sqlFee.Parameters["@Status"].Value = (int)agency.Status;
                sqlFee.Parameters["@CreateDate"].Value = this.CreateDate;
                sqlFee.Parameters["@UpdateDate"].Value = this.UpdateDate;
                sqlFee.Parameters["@Summary"].Value = null;

                sqlFee.ExecuteNonQuery();
                #endregion

                foreach (var item in order.Items)
                {
                    //2020-09-09 更新
                    //完税价格计算公式：Round(Round(报关总价 * 运保杂, 2) * 海关汇率, 0)
                    //2021-02-05 更新，不管一步申报还是两步申报，报关总价都是取两位小数
                    //完税价格计算公式：Round(Round(报关总价 * 运保杂, 2) * 海关汇率, 2)
                    var topPrice = (item.TotalPrice * ConstConfig.TransPremiumInsurance).ToRound(2);
                    var total = (topPrice * customsExchangeRate).ToRound(2);
                    var importTaxValue = (total * item.ImportTax.Rate).ToRound(2);
                    item.ImportTax.Value = importTaxValue;
                    #region 更新关税
                    SqlCommand sqlTariff = new SqlCommand("OrderItemTaxesUpdatePro", conn);
                    sqlTariff.CommandType = CommandType.StoredProcedure;

                    sqlTariff.Parameters.Add(new SqlParameter("@ID", SqlDbType.VarChar, 50));
                    sqlTariff.Parameters.Add(new SqlParameter("@OrderItemID", SqlDbType.VarChar, 50));
                    sqlTariff.Parameters.Add(new SqlParameter("@Type", SqlDbType.Int));
                    sqlTariff.Parameters.Add(new SqlParameter("@Rate", SqlDbType.Decimal));
                    sqlTariff.Parameters.Add(new SqlParameter("@Value", SqlDbType.Decimal));
                    sqlTariff.Parameters.Add(new SqlParameter("@Status", SqlDbType.Int));
                    sqlTariff.Parameters.Add(new SqlParameter("@UpdateDate", SqlDbType.DateTime));
                    sqlTariff.Parameters.Add(new SqlParameter("@Summary", SqlDbType.VarChar, 300));

                    sqlTariff.Parameters["@ID"].Value = item.ImportTax.ID;
                    sqlTariff.Parameters["@OrderItemID"].Value = item.ImportTax.OrderItemID;
                    sqlTariff.Parameters["@Type"].Value = (int)item.ImportTax.Type;
                    sqlTariff.Parameters["@Rate"].Value = item.ImportTax.Rate;
                    sqlTariff.Parameters["@Value"].Value = item.ImportTax.Value;
                    sqlTariff.Parameters["@Status"].Value = (int)item.ImportTax.Status;
                    sqlTariff.Parameters["@UpdateDate"].Value = this.UpdateDate;
                    sqlTariff.Parameters["@Summary"].Value = item.ImportTax.Summary;

                    sqlTariff.ExecuteNonQuery();
                    #endregion

                    #region 更新消费税
                    if (item.ExciseTax != null)
                    {
                        //消费税计算公式：Round((完税价格＋关税)÷(1－消费税税率)×消费税税率, 2）
                        item.ExciseTax.Value = ((total + importTaxValue) / (1 - item.ExciseTax.Rate) * item.ExciseTax.Rate).ToRound(2);

                        SqlCommand sqlExciseTax = new SqlCommand("OrderItemTaxesUpdatePro", conn);
                        sqlExciseTax.CommandType = CommandType.StoredProcedure;

                        sqlExciseTax.Parameters.Add(new SqlParameter("@ID", SqlDbType.VarChar, 50));
                        sqlExciseTax.Parameters.Add(new SqlParameter("@OrderItemID", SqlDbType.VarChar, 50));
                        sqlExciseTax.Parameters.Add(new SqlParameter("@Type", SqlDbType.Int));
                        sqlExciseTax.Parameters.Add(new SqlParameter("@Rate", SqlDbType.Decimal));
                        sqlExciseTax.Parameters.Add(new SqlParameter("@Value", SqlDbType.Decimal));
                        sqlExciseTax.Parameters.Add(new SqlParameter("@Status", SqlDbType.Int));
                        sqlExciseTax.Parameters.Add(new SqlParameter("@UpdateDate", SqlDbType.DateTime));
                        sqlExciseTax.Parameters.Add(new SqlParameter("@Summary", SqlDbType.VarChar, 300));

                        sqlExciseTax.Parameters["@ID"].Value = item.ExciseTax.ID;
                        sqlExciseTax.Parameters["@OrderItemID"].Value = item.ExciseTax.OrderItemID;
                        sqlExciseTax.Parameters["@Type"].Value = (int)item.ExciseTax.Type;
                        sqlExciseTax.Parameters["@Rate"].Value = item.ExciseTax.Rate;
                        sqlExciseTax.Parameters["@Value"].Value = item.ExciseTax.Value;
                        sqlExciseTax.Parameters["@Status"].Value = (int)item.ExciseTax.Status;
                        sqlExciseTax.Parameters["@UpdateDate"].Value = this.UpdateDate;
                        sqlExciseTax.Parameters["@Summary"].Value = item.ExciseTax.Summary;

                        sqlExciseTax.ExecuteNonQuery();
                    }
                    #endregion

                    var exciseTaxRate = item.ExciseTax?.Rate ?? 0m;
                    //增值税计算公式：Round(((完税价 + 关税) + (完税价 + 关税) / (1-消费税税率) * 消费税税率) * 增值税率, 2)
                    item.AddedValueTax.Value = (((total + importTaxValue) + (total + importTaxValue) / (1 - exciseTaxRate) * exciseTaxRate) * item.AddedValueTax.Rate).ToRound(2);
                    #region 更新增值税
                    SqlCommand sqlAddedValue = new SqlCommand("OrderItemTaxesUpdatePro", conn);
                    sqlAddedValue.CommandType = CommandType.StoredProcedure;

                    sqlAddedValue.Parameters.Add(new SqlParameter("@ID", SqlDbType.VarChar, 50));
                    sqlAddedValue.Parameters.Add(new SqlParameter("@OrderItemID", SqlDbType.VarChar, 50));
                    sqlAddedValue.Parameters.Add(new SqlParameter("@Type", SqlDbType.Int));
                    sqlAddedValue.Parameters.Add(new SqlParameter("@Rate", SqlDbType.Decimal));
                    sqlAddedValue.Parameters.Add(new SqlParameter("@Value", SqlDbType.Decimal));
                    sqlAddedValue.Parameters.Add(new SqlParameter("@Status", SqlDbType.Int));
                    sqlAddedValue.Parameters.Add(new SqlParameter("@UpdateDate", SqlDbType.DateTime));
                    sqlAddedValue.Parameters.Add(new SqlParameter("@Summary", SqlDbType.VarChar, 300));

                    sqlAddedValue.Parameters["@ID"].Value = item.AddedValueTax.ID;
                    sqlAddedValue.Parameters["@OrderItemID"].Value = item.AddedValueTax.OrderItemID;
                    sqlAddedValue.Parameters["@Type"].Value = (int)item.AddedValueTax.Type;
                    sqlAddedValue.Parameters["@Rate"].Value = item.AddedValueTax.Rate;
                    sqlAddedValue.Parameters["@Value"].Value = item.AddedValueTax.Value;
                    sqlAddedValue.Parameters["@Status"].Value = (int)item.AddedValueTax.Status;
                    sqlAddedValue.Parameters["@UpdateDate"].Value = this.UpdateDate;
                    sqlAddedValue.Parameters["@Summary"].Value = item.AddedValueTax.Summary;

                    sqlAddedValue.ExecuteNonQuery();
                    #endregion
                }

                //若后面出现数量产品价格会变化，则重新计算订单总价格
                //order.DeclarePrice = order.Items.Sum(t => t.TotalPrice);

                SqlCommand sqlRate = new SqlCommand("OrderRateUpdatePro", conn);
                sqlRate.CommandType = CommandType.StoredProcedure;

                sqlRate.Parameters.Add(new SqlParameter("@ID", SqlDbType.VarChar, 50));
                sqlRate.Parameters.Add(new SqlParameter("@CustomsExchangeRate", SqlDbType.Decimal));
                sqlRate.Parameters.Add(new SqlParameter("@RealExchangeRate", SqlDbType.Decimal));

                sqlRate.Parameters["@ID"].Value = order.ID;
                sqlRate.Parameters["@CustomsExchangeRate"].Value = customsExchangeRate;
                sqlRate.Parameters["@RealExchangeRate"].Value = realExchangeRate;

                sqlRate.ExecuteNonQuery();

                //此处调用一下 Yahv 的 dll ，使用 Task 执行, 并记录异常日志 Begin

                PaymentToYahv paymentToYahv = new PaymentToYahv(agency, Enums.OrderBillType.Normal, order.ClientAgreement);
                paymentToYahv.Execute();

                //此处调用了一下 Yahv 的 dll ，其中用了 Task, 并会记录异常日志 End

            }

            this.OnOrderBillGenerated();


        }
        public void IcgooToEntryNoticeSpeed(bool isPickUp = true)
        {
            try
            {
                var con = System.Configuration.ConfigurationManager.ConnectionStrings["foricScCustomsConnectionString"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(con))
                {
                    conn.Open();
                    #region 新增EntryNotice
                    string entryNoticeID = Needs.Overall.PKeySigner.Pick(PKeyType.EntryNotice);
                    SqlCommand sqlCmd = new SqlCommand("EntryNotice", conn);
                    sqlCmd.CommandType = CommandType.StoredProcedure;

                    sqlCmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.VarChar, 50));
                    sqlCmd.Parameters.Add(new SqlParameter("@OrderID", SqlDbType.VarChar, 50));
                    sqlCmd.Parameters.Add(new SqlParameter("@WarehouseType", SqlDbType.Int));
                    sqlCmd.Parameters.Add(new SqlParameter("@ClientCode", SqlDbType.VarChar, 50));
                    sqlCmd.Parameters.Add(new SqlParameter("@SortingRequire", SqlDbType.Int));
                    sqlCmd.Parameters.Add(new SqlParameter("@EntryNoticeStatus", SqlDbType.Int));
                    sqlCmd.Parameters.Add(new SqlParameter("@Status", SqlDbType.Int));
                    sqlCmd.Parameters.Add(new SqlParameter("@CreateDate", SqlDbType.DateTime));
                    sqlCmd.Parameters.Add(new SqlParameter("@UpdateDate", SqlDbType.DateTime));
                    sqlCmd.Parameters.Add(new SqlParameter("@Summary", SqlDbType.NChar, 300));

                    sqlCmd.Parameters["@ID"].Value = entryNoticeID;
                    sqlCmd.Parameters["@OrderID"].Value = this.ID;
                    sqlCmd.Parameters["@WarehouseType"].Value = (int)Enums.WarehouseType.HongKong;
                    sqlCmd.Parameters["@ClientCode"].Value = this.Client.ClientCode;
                    sqlCmd.Parameters["@SortingRequire"].Value = this.Client.ClientRank == Enums.ClientRank.ClassFive ? Enums.SortingRequire.UnPacking : Enums.SortingRequire.Packed;
                    sqlCmd.Parameters["@EntryNoticeStatus"].Value = (int)Enums.EntryNoticeStatus.UnBoxed;
                    sqlCmd.Parameters["@Status"].Value = (int)Enums.Status.Normal;
                    sqlCmd.Parameters["@CreateDate"].Value = this.CreateDate;
                    sqlCmd.Parameters["@UpdateDate"].Value = this.UpdateDate;
                    sqlCmd.Parameters["@Summary"].Value = "";

                    sqlCmd.ExecuteNonQuery();
                    #endregion

                    #region 新增EntryNoticeItem
                    DataTable dt = new DataTable();
                    dt.Columns.Add("ID");
                    dt.Columns.Add("EntryNoticeID");
                    dt.Columns.Add("OrderItemID");
                    dt.Columns.Add("DecListID");
                    dt.Columns.Add("IsSpotCheck");
                    dt.Columns.Add("EntryNoticeStatus");
                    dt.Columns.Add("Status");
                    dt.Columns.Add("CreateDate");
                    dt.Columns.Add("UpdateDate");

                    foreach (var item in this.Items)
                    {
                        DataRow dr = dt.NewRow();
                        dr[0] = Needs.Overall.PKeySigner.Pick(PKeyType.EntryNoticeItem);
                        dr[1] = entryNoticeID;
                        dr[2] = item.ID;
                        dr[3] = null;
                        dr[4] = item.IsSampllingCheck;
                        dr[5] = (int)Enums.EntryNoticeStatus.UnBoxed;
                        dr[6] = (int)Enums.Status.Normal;
                        dr[7] = this.CreateDate;
                        dr[8] = this.UpdateDate;

                        dt.Rows.Add(dr);
                    }

                    SqlBulkCopy bulkCopy = new SqlBulkCopy(conn);
                    bulkCopy.DestinationTableName = "EntryNoticeItems";
                    bulkCopy.BatchSize = dt.Rows.Count;
                    bulkCopy.WriteToServer(dt);
                    #endregion
                    if (isPickUp) //是否自提
                    {
                        //HK提货通知                    
                        var deliverynotice = new DeliveryNotice();
                        deliverynotice.DeliveryNoticeStatus = Enums.DeliveryNoticeStatus.Delivered;
                        deliverynotice.Order = this;
                        deliverynotice.Admin = this.Client.Merchandiser;
                        deliverynotice.Enter();
                        var deliveryConsignees = new DeliveryConsignee();
                        deliveryConsignees.DeliveryNoticeID = deliverynotice.ID;
                        deliveryConsignees.Supplier = this.OrderConsignee.ClientSupplier.ChineseName;
                        deliveryConsignees.PickUpDate = this.OrderConsignee.PickUpTime.Value;
                        deliveryConsignees.Address = this.OrderConsignee.Address == null ? " " : this.OrderConsignee.Address;
                        deliveryConsignees.Contact = this.OrderConsignee.Contact == null ? " " : this.OrderConsignee.Contact;
                        deliveryConsignees.Tel = this.OrderConsignee.Mobile==null?" ": this.OrderConsignee.Mobile;
                        deliveryConsignees.Enter();
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void testInsert(DataTable dtitem,DataTable dtCategory,DataTable dtTax,DataTable dtPremiums)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                for (int i = 0; i < dtitem.Rows.Count; i++)
                {
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.OrderItems
                    {
                        ID = dtitem.Rows[i]["ID"].ToString(),
                        OrderID = dtitem.Rows[i]["OrderID"].ToString(),
                        Name = dtitem.Rows[i]["Name"].ToString(),
                        Model = dtitem.Rows[i]["Model"].ToString(),
                        Manufacturer = dtitem.Rows[i]["Manufacturer"].ToString(),
                        Batch = dtitem.Rows[i]["Batch"].ToString(),
                        Origin = dtitem.Rows[i]["Origin"].ToString(),
                        Quantity = Convert.ToDecimal(dtitem.Rows[i]["Quantity"]),
                        //DeclaredQuantity = Convert.ToDecimal(dtitem.Rows[i]["DeclaredQuantity"]),
                        Unit = dtitem.Rows[i]["Unit"].ToString(),
                        UnitPrice = Convert.ToDecimal(dtitem.Rows[i]["UnitPrice"]),
                        TotalPrice = Convert.ToDecimal(dtitem.Rows[i]["TotalPrice"]),
                        GrossWeight = Convert.ToDecimal(dtitem.Rows[i]["GrossWeight"]),
                        IsSampllingCheck = Convert.ToBoolean(dtitem.Rows[i]["IsSampllingCheck"]),
                        ClassifyStatus = Convert.ToInt16(dtitem.Rows[i]["ClassifyStatus"]),
                        Status = 200,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        Summary = "",
                        ProductUniqueCode = dtitem.Rows[i]["ProductUniqueCode"].ToString(),
                    });
                }

                if (dtCategory.Rows.Count > 0)
                {
                    for (int i = 0; i < dtCategory.Rows.Count; i++)
                    {
                        reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.OrderItemCategories
                        {
                            ID = dtCategory.Rows[i]["ID"].ToString(),
                            OrderItemID = dtCategory.Rows[i]["OrderItemID"].ToString(),
                            ClassifyFirstOperator = dtCategory.Rows[i]["ClassifyFirstOperator"].ToString(),
                            ClassifySecondOperator = dtCategory.Rows[i]["ClassifySecondOperator"].ToString(),
                            Type = Convert.ToInt16(dtCategory.Rows[i]["Type"]),
                            TaxCode = dtCategory.Rows[i]["TaxCode"].ToString(),
                            TaxName = dtCategory.Rows[i]["TaxName"].ToString(),
                            HSCode = dtCategory.Rows[i]["HSCode"].ToString(),
                            Unit1 = dtCategory.Rows[i]["Unit1"].ToString(),
                            Unit2 = string.IsNullOrEmpty(dtCategory.Rows[i]["Unit2"].ToString()) ? null : dtCategory.Rows[i]["Unit2"].ToString(),
                            Name = dtCategory.Rows[i]["Name"].ToString(),
                            Elements = dtCategory.Rows[i]["Elements"].ToString(),
                            //Qty1 = Convert.ToDecimal(dtCategory.Rows[i]["Qty1"]),
                            //Qty2 = Convert.ToDecimal(dtCategory.Rows[i]["Qty2"]),
                            CIQCode = dtCategory.Rows[i]["CIQCode"].ToString(),
                            Status = 200,
                            CreateDate = DateTime.Now,
                            UpdateDate = DateTime.Now,
                            Summary = ""
                        });
                    }
                }

                if (dtTax.Rows.Count > 0)
                {
                    for (int i = 0; i < dtTax.Rows.Count; i++)
                    {
                        reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.OrderItemTaxes
                        {
                            ID = dtTax.Rows[i]["ID"].ToString(),
                            OrderItemID = dtTax.Rows[i]["OrderItemID"].ToString(),
                            Type = Convert.ToInt16(dtTax.Rows[i]["Type"]),
                            Rate = Convert.ToDecimal(dtTax.Rows[i]["Rate"]),
                            ReceiptRate = Convert.ToDecimal(dtTax.Rows[i]["ReceiptRate"]),
                            //Value = Convert.ToDecimal(dtTax.Rows[i]["Value"]),
                            Status = 200,
                            CreateDate = DateTime.Now,
                            UpdateDate = DateTime.Now,
                            Summary = ""
                        });
                    }
                }

                if (dtPremiums.Rows.Count > 0)
                {
                    for (int i = 0; i < dtPremiums.Rows.Count; i++)
                    {
                        reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.OrderPremiums
                        {
                            ID = dtPremiums.Rows[i]["ID"].ToString(),
                            OrderID = dtPremiums.Rows[i]["OrderID"].ToString(),
                            OrderItemID = dtPremiums.Rows[i]["OrderItemID"].ToString(),
                            AdminID = dtPremiums.Rows[i]["AdminID"].ToString(),
                            Type = Convert.ToInt16(dtPremiums.Rows[i]["Type"]),
                            //Name = dtPremiums.Rows[i]["Name"].ToString(),
                            Count = Convert.ToInt16(dtPremiums.Rows[i]["Count"]),
                            UnitPrice = Convert.ToDecimal(dtPremiums.Rows[i]["UnitPrice"]),
                            Currency = dtPremiums.Rows[i]["Currency"].ToString(),
                            Rate = Convert.ToDecimal(dtPremiums.Rows[i]["Rate"]),
                            //StandardID = dtPremiums.Rows[i]["StandardID"].ToString(),
                            //StandardPrice = Convert.ToDecimal(dtPremiums.Rows[i]["StandardPrice"]),
                            //StandardCurrency = dtPremiums.Rows[i]["StandardCurrency"].ToString(),
                            //StandardRemark = dtPremiums.Rows[i]["StandardRemark"].ToString(),
                            Status = 200,
                            CreateDate = DateTime.Now,
                            UpdateDate = DateTime.Now,
                            Summary = ""
                        });
                    }
                }
            }
        }
    }
}
