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
    
    public partial class T_Icgoo_User
    {
        public string ID { get; set; }
        public string MemberID { get; set; }
        public string CompanyCode { get; set; }
        public string AccountName { get; set; }
        public string Password { get; set; }
        public Nullable<int> Status { get; set; }
        public System.DateTime CreateTime { get; set; }
        public System.DateTime UpdateTime { get; set; }
        public string Summary { get; set; }
    
        public virtual T_Member T_Member { get; set; }
    }
}
