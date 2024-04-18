using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMvc.Models
{
    /// <summary>
    /// 供应商信息
    /// </summary>
    public class SupplierInfoViewModel
    {
        //ID
        public string ID { get; set; }

        //中文名称
        public string ChineseName { get; set; }

        //英文名称
        public string Name { get; set; }

        //备注
        public string Summary { get; set; }
    }

    /// <summary>
    /// 供应商银行账号
    /// </summary>
    public class BeneficiarieInfoViewModel
    {
        //ID
        public string ID { get; set; }

        //供应商编号
        public string ClientSupplierID { get; set; }

        //银行名称
        public string BankName { get; set; }

        //银行地址
        public string BankAddress { get; set; }

        //银行账户
        public string BankAccount { get; set; }

        //银行代码
        public string SwiftCode { get; set; }

        //状态
        public int Status { get; set; }

        //创建日期
        public DateTime CreateDate { get; set; }

        //备注
        public string Summary { get; set; }
    }

    /// <summary>
    /// 供应商提货地址
    /// </summary>
    public class SupplierAddressesViewModel
    {
        //ID
        public string ID { get; set; }

        //供应商编号
        public string ClientSupplierID { get; set; }

        //是否默认
        public bool IsDefault { get; set; }

        //公司地址(省市区)
        public string[] Address { get; set; }

        //详细地址
        public string DetailAddress { get; set; }

        //地址
        public string AllAddress { get; set; }

        
        //邮编
        public string ZipCode { get; set; }

        //联系人
        public string Name { get; set; }

        //手机号
        public string Mobile { get; set; }

        //备注
        public string Summary { get; set; }
    }
}