﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class YD_LogisticsDBEntities : DbContext
    {
        public YD_LogisticsDBEntities()
            : base("name=YD_LogisticsDBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<T_Address_Area> T_Address_Area { get; set; }
        public virtual DbSet<T_Address_City> T_Address_City { get; set; }
        public virtual DbSet<T_Address_Province> T_Address_Province { get; set; }
        public virtual DbSet<T_Attachment> T_Attachment { get; set; }
        public virtual DbSet<T_Control_3C> T_Control_3C { get; set; }
        public virtual DbSet<T_Control_ApproveRecord> T_Control_ApproveRecord { get; set; }
        public virtual DbSet<T_Control_CCC> T_Control_CCC { get; set; }
        public virtual DbSet<T_Control_Model> T_Control_Model { get; set; }
        public virtual DbSet<T_Declaration_Elements> T_Declaration_Elements { get; set; }
        public virtual DbSet<T_Declaration_Elements_AddTraiff> T_Declaration_Elements_AddTraiff { get; set; }
        public virtual DbSet<T_Declaration_Elements_Default> T_Declaration_Elements_Default { get; set; }
        public virtual DbSet<T_Declaration_Origin> T_Declaration_Origin { get; set; }
        public virtual DbSet<T_Declare> T_Declare { get; set; }
        public virtual DbSet<T_Delivery_Note> T_Delivery_Note { get; set; }
        public virtual DbSet<T_Delivery_NoteDetail> T_Delivery_NoteDetail { get; set; }
        public virtual DbSet<T_Download_History> T_Download_History { get; set; }
        public virtual DbSet<T_Download_History_Order> T_Download_History_Order { get; set; }
        public virtual DbSet<T_ExchangeRate> T_ExchangeRate { get; set; }
        public virtual DbSet<T_Finance_Account> T_Finance_Account { get; set; }
        public virtual DbSet<T_Finance_Invoice> T_Finance_Invoice { get; set; }
        public virtual DbSet<T_Finance_InvoiceDetails> T_Finance_InvoiceDetails { get; set; }
        public virtual DbSet<T_Finance_Payment> T_Finance_Payment { get; set; }
        public virtual DbSet<T_Finance_Receipts> T_Finance_Receipts { get; set; }
        public virtual DbSet<T_Finance_Serial> T_Finance_Serial { get; set; }
        public virtual DbSet<T_Finance_Vault> T_Finance_Vault { get; set; }
        public virtual DbSet<T_Icgoo_Declare> T_Icgoo_Declare { get; set; }
        public virtual DbSet<T_Icgoo_Declare_Default> T_Icgoo_Declare_Default { get; set; }
        public virtual DbSet<T_Icgoo_Order> T_Icgoo_Order { get; set; }
        public virtual DbSet<T_Icgoo_PartNo> T_Icgoo_PartNo { get; set; }
        public virtual DbSet<T_Icgoo_PostLog> T_Icgoo_PostLog { get; set; }
        public virtual DbSet<T_Icgoo_Receive> T_Icgoo_Receive { get; set; }
        public virtual DbSet<T_Icgoo_RequestLog> T_Icgoo_RequestLog { get; set; }
        public virtual DbSet<T_Icgoo_RequestPara> T_Icgoo_RequestPara { get; set; }
        public virtual DbSet<T_Icgoo_SMSContact> T_Icgoo_SMSContact { get; set; }
        public virtual DbSet<T_Member> T_Member { get; set; }
        public virtual DbSet<T_Member_DeliveryAddress> T_Member_DeliveryAddress { get; set; }
        public virtual DbSet<T_Member_Invoice> T_Member_Invoice { get; set; }
        public virtual DbSet<T_Member_InvoiceAddress> T_Member_InvoiceAddress { get; set; }
        public virtual DbSet<T_Member_InvoiceChangeRec> T_Member_InvoiceChangeRec { get; set; }
        public virtual DbSet<T_Member_Products> T_Member_Products { get; set; }
        public virtual DbSet<T_Member_SupAgreement> T_Member_SupAgreement { get; set; }
        public virtual DbSet<T_Member_Supplier> T_Member_Supplier { get; set; }
        public virtual DbSet<T_Member_Supplier_Account> T_Member_Supplier_Account { get; set; }
        public virtual DbSet<T_Member_Supplier_DeliveryAddress> T_Member_Supplier_DeliveryAddress { get; set; }
        public virtual DbSet<T_National> T_National { get; set; }
        public virtual DbSet<T_Operate_Log> T_Operate_Log { get; set; }
        public virtual DbSet<T_Order> T_Order { get; set; }
        public virtual DbSet<T_Order_Attachments> T_Order_Attachments { get; set; }
        public virtual DbSet<T_Order_Declare> T_Order_Declare { get; set; }
        public virtual DbSet<T_Order_Incidentals> T_Order_Incidentals { get; set; }
        public virtual DbSet<T_Order_InternationalExpress> T_Order_InternationalExpress { get; set; }
        public virtual DbSet<T_Order_Model> T_Order_Model { get; set; }
        public virtual DbSet<T_Order_Model_Default> T_Order_Model_Default { get; set; }
        public virtual DbSet<T_Order_Pack> T_Order_Pack { get; set; }
        public virtual DbSet<T_Order_PackDetail> T_Order_PackDetail { get; set; }
        public virtual DbSet<T_Order_PayExchange> T_Order_PayExchange { get; set; }
        public virtual DbSet<T_Order_Payment> T_Order_Payment { get; set; }
        public virtual DbSet<T_Order_ReceiptsDetails> T_Order_ReceiptsDetails { get; set; }
        public virtual DbSet<T_Sys_CommissionProportion> T_Sys_CommissionProportion { get; set; }
        public virtual DbSet<T_Sys_Departments> T_Sys_Departments { get; set; }
        public virtual DbSet<T_Sys_Functions> T_Sys_Functions { get; set; }
        public virtual DbSet<T_Sys_Log> T_Sys_Log { get; set; }
        public virtual DbSet<T_Sys_RoleFunctions> T_Sys_RoleFunctions { get; set; }
        public virtual DbSet<T_Sys_Roles> T_Sys_Roles { get; set; }
        public virtual DbSet<T_Sys_UserDepartments> T_Sys_UserDepartments { get; set; }
        public virtual DbSet<T_Sys_UserRoles> T_Sys_UserRoles { get; set; }
        public virtual DbSet<T_Sys_Users> T_Sys_Users { get; set; }
        public virtual DbSet<T_Tax_Default> T_Tax_Default { get; set; }
        public virtual DbSet<T_Temporary> T_Temporary { get; set; }
        public virtual DbSet<T_Temporary_Attachment> T_Temporary_Attachment { get; set; }
        public virtual DbSet<T_Temporary_Model> T_Temporary_Model { get; set; }
        public virtual DbSet<T_Icgoo_User> T_Icgoo_User { get; set; }
        public virtual DbSet<T_Tax_New> T_Tax_New { get; set; }
        public virtual DbSet<T_Tax_NewDefault> T_Tax_NewDefault { get; set; }
    }
}
