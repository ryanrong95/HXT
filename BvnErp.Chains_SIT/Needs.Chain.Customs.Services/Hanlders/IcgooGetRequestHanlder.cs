using Needs.Ccs.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Hanlders
{
    /// <summary>
    /// Icgoo请求完成时发生
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void IcgooGetRequestHanlder(object sender, IcgooGetRequestEventArgs e);

    /// <summary>
    /// 发送短信
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void IcgooSendSMSHanlder(object sender, IcgooGetRequestEventArgs e);

    /// <summary>
    /// Icgoo请求后的参数
    /// </summary>
    public class IcgooGetRequestEventArgs : EventArgs
    {
       
        public string Supplier { get; private set; }

        public int Days { get; private set; }

        public string Url { get; private set; }

        public string Info { get; private set; }

        public bool IsSuccess { get; private set; }
        public CompanyTypeEnums CompanyType { get; set; }

        public IcgooGetRequestEventArgs(string Supplier, int Days, string Url,string Info,bool IsSuccess, CompanyTypeEnums companyType)
        {       
            this.Supplier = Supplier;
            this.Days = Days;
            this.Url = Url;
            this.Info = Info;
            this.IsSuccess = IsSuccess;
            this.CompanyType = companyType;
        }

        public IcgooGetRequestEventArgs() { }
    }
}
