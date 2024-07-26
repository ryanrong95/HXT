using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class BankMap
    {
        public Dictionary<string, string> banks;


        public BankMap()
        {
            this.banks = new Dictionary<string, string>();
            this.banks.Add("华芯通-星展银行账户", "星展银行(中国)有限公司深圳分行");
            this.banks.Add("华芯通-中国银行深圳罗岗支行", "中国银行深圳罗岗支行");
            this.banks.Add("华芯通-宁波银行人民币户", "宁波银行深圳分行");
            this.banks.Add("华芯通-宁波银行美元户", "宁波银行深圳分行美金账户");
            this.banks.Add("华芯通-农业银行人民币账户", "中国农业银行股份有限公司深圳免税大厦支行");
            this.banks.Add("华芯通-农业银行美元账户", "中国农业银行深圳免税大厦支行美金账户");
            this.banks.Add("华芯通-兴业银行", "兴业银行股份有限公司深圳罗湖支行");
            this.banks.Add("华芯通-兴业银行快捷支付平台", "华芯通-兴业银行快捷支付平台");
            this.banks.Add("华芯通-渣打银行", "渣打银行");
            this.banks.Add("华芯通-华夏银行人民币账户", "华芯通-华夏银行人民币账户");
            this.banks.Add("华芯通-华润银行", "华润银行");
            this.banks.Add("华芯通-宁波银行承兑户", "华芯通-宁波银行承兑户");
            this.banks.Add("华芯通-华润银行承兑户", "华润银行承兑户");
            this.banks.Add("珠海华润银行股份有限公司深圳分行", "华润银行");
            this.banks.Add("宁波银行股份有限公司深圳分行营业部", "宁波银行深圳分行");
            this.banks.Add("华芯通-宁波银行保证金户", "宁波银行深圳分行保证金账户");
            this.banks.Add("华芯通-上海浦东发展银行", "华芯通-上海浦东发展银行");
        }
    }

    
}
