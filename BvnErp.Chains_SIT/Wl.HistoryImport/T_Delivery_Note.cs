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
    
    public partial class T_Delivery_Note
    {
        public string ID { get; set; }
        public string DeliveryCode { get; set; }
        public string MemberID { get; set; }
        public string OrderID { get; set; }
        public Nullable<int> BoxNumber { get; set; }
        public bool IsPrint { get; set; }
        public int DeliveryMethod { get; set; }
        public string TakeContact { get; set; }
        public string TakePhone { get; set; }
        public string TakeIdentification { get; set; }
        public string TakeIdentificationNumber { get; set; }
        public Nullable<System.DateTime> TakeDate { get; set; }
        public Nullable<System.DateTime> DeliveryDate { get; set; }
        public string RecipientName { get; set; }
        public string RecipientPhone { get; set; }
        public string CompanyName { get; set; }
        public string SZProvince { get; set; }
        public string SZCity { get; set; }
        public string SZArea { get; set; }
        public string SZAddress { get; set; }
        public string DriverName { get; set; }
        public string DriverPhone { get; set; }
        public string PlateNumber { get; set; }
        public Nullable<int> ExpressCompany { get; set; }
        public string ExpressNumber { get; set; }
        public string ExpressHtml { get; set; }
        public Nullable<int> DeliveryStatus { get; set; }
        public Nullable<int> RecordStatus { get; set; }
        public string Creator { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public string Updator { get; set; }
        public Nullable<System.DateTime> UpdateTime { get; set; }
        public string SubExpressNumbers { get; set; }
        public string SubExpressHtml { get; set; }
        public Nullable<int> ExpressPayType { get; set; }
        public Nullable<int> DeliveryType { get; set; }
    }
}
