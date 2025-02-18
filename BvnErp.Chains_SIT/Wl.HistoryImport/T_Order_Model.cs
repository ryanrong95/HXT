//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Wl.HistoryImport
{
    using System;
    using System.Collections.Generic;
    
    public partial class T_Order_Model
    {
        public string ID { get; set; }
        public string OrderID { get; set; }
        public string OrderDeclareID { get; set; }
        public Nullable<int> ClassifyStatus { get; set; }
        public string AbnormalCause { get; set; }
        public string BatchNo { get; set; }
        public string Manufacturer { get; set; }
        public string ProductName { get; set; }
        public string DeclareProductName { get; set; }
        public string InvoiceProductName { get; set; }
        public string ModelInfoClassificationValue { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string PlaceOfProduction { get; set; }
        public decimal Quantity { get; set; }
        public Nullable<decimal> UnitPrice { get; set; }
        public string Unit { get; set; }
        public decimal TotalDeclarePrice { get; set; }
        public Nullable<decimal> GrossWeight { get; set; }
        public Nullable<decimal> TotalDeclareCNYPrice { get; set; }
        public Nullable<decimal> TariffRate { get; set; }
        public Nullable<decimal> Tariff { get; set; }
        public Nullable<decimal> ValueAddedTaxRate { get; set; }
        public Nullable<decimal> ValueAddedTax { get; set; }
        public Nullable<decimal> AgencyFee { get; set; }
        public string CustomsCode { get; set; }
        public string CIQCode { get; set; }
        public Nullable<bool> IsInspection { get; set; }
        public Nullable<decimal> InspectionFee { get; set; }
        public string Elements { get; set; }
        public Nullable<bool> IsInSZStorage { get; set; }
        public Nullable<decimal> SZStock { get; set; }
        public string Description { get; set; }
        public Nullable<int> DisplayOrder { get; set; }
        public Nullable<int> RecordStatus { get; set; }
        public string Creator { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public string Updator { get; set; }
        public Nullable<System.DateTime> UpdateTime { get; set; }
        public string FirstLegalUnit { get; set; }
        public string SecondLegalUnit { get; set; }
        public Nullable<decimal> FirstQuantity { get; set; }
        public Nullable<decimal> SecondQuantity { get; set; }
        public string CustomsSupervisionCondition { get; set; }
        public Nullable<bool> IsModelControl { get; set; }
        public Nullable<int> IsCCC { get; set; }
        public string CCCAttach { get; set; }
        public Nullable<bool> IsSampling { get; set; }
        public Nullable<bool> SamplingResult { get; set; }
        public string SamplingRemark { get; set; }
    }
}
