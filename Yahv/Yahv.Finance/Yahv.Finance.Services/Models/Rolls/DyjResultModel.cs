using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.Finance.Services.Models.Rolls
{
    /// <summary>
    /// 大赢家接口返回格式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DyjResultModel<T> where T : class
    {
        public int status { get; set; }
        public string message { get; set; }
        public bool isSuccess { get; set; }
        public bool isPage { get; set; }
        public PageInfo pageInfo { get; set; }
        //public object list { get; set; }
        //public object list1 { get; set; }
        public List<T> data { get; set; }
        //public object filelist { get; set; }
        //public string taken { get; set; }
    }

    public class PageInfo
    {
        public int counts { get; set; }
        public int pagesize { get; set; }
        public int pages { get; set; }
        public int currentpage { get; set; }
    }
}
