#define DEVELOP     //开发
//#define TEST      //测试版
//#define FINAL     //生产版

using System;
using Yahv.Underly.Attributes;

namespace WebApp.Services
{

    public enum FromType
    {

#if DEVELOP
        [Description("http")]
        Scheme = 0,
        [Description("localhost:8080")]
        Web = 1,
        [Description("localhost:8080/wmsapi")]
        WebApi = 2,
        [Description("http://hv.erp.b1b.com/ermapi/")]
        ErmApi = 3
#endif

#if Publish
        [Description("http")]
        Scheme = 0,
        [Description("hv.warehouse.b1b.com")]
        Web = 1,
        [Description("hv.warehouse.b1b.com/wmsapi")]
        WebApi = 2,
             [Description("http://hv.erp.b1b.com/ermapi/")]
        ErmApi = 3
#endif

#if TEST
        [Description("https")]
        Scheme =0,
        [Description("warehouse0.ic360.cn")]
        Web = 1,
        [Description("warehouse0.ic360.cn/wmsapi")]
        WebApi =2,
             [Description("http://hv.erp.b1b.com/ermapi/")]
        ErmApi = 3
#endif

#if FINAL
        [Description("https")]
        Scheme =0,
        [Description("www.pfwms.com")]
        Web = 1,
        [Description("www.pfwms.com/wmsapi")]
        WebApi =2,
             [Description("http://hv.erp.b1b.com/ermapi/")]
        ErmApi = 3
#endif       


    }
}
