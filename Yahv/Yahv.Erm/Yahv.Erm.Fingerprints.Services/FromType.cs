using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.Erm.Fingerprints.Services
{
    /// <summary>
    /// 连接来源
    /// </summary>
    public class LinkSource
    {
        public const string YuanDaTime = "http://hv.erp.b1b.com/fingerapi/Times/Enterprise";
        public const string TaoboTime = "http://api.m.taobao.com/rest/api3.do?api=mtop.common.getTimestamp";

        LinkSource()
        {

        }
    }
}
