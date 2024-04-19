#define DEVELOP     //开发
#define TEST      //测试版
//#define FINAL     //生产版
using NtErp.Wss.Sales.Services.Utils.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Underly
{
    public enum FromType
    {
#if DEVELOP
        [Naming("bv.b1b.com")]
        B1B = 1,
        [Naming("bv.sso.b1b.com")]
        SSO = 2,
        [Naming("bv.seller.b1b.com")]
        Seller = 9,
        [Naming("bv.buyer.b1b.com")]
        Buyer = 10,
        [Naming("192.168.66.223")]
        //[Naming("192.168.29.59:81")]
        Fixed = 13,
        [Naming("192.168.29.59")]
        Data = 14,
        [Naming("erp.service.com")]
        ErpService = 15,
        [Naming("bv.helper.b1b.com")]
        HC = 16,
        [Naming("new.yd.51db.com")]
        ERP = 17,
        [Naming("new.app.ic360.cn")]
        APP = 18,
        [Naming("new.www.ic360.cn")]
        IC360 = 19,
        [Naming("new.www.d9.com.cn")]
        D9 = 20,
        [Naming("api.sso.ic360.cn")]
        SSOApi = 100
#elif TEST
         [Naming("b1b7.ic360.cn")]
        B1B = 1,
        [Naming("sso7.ic360.cn")]
        SSO = 2,
        [Naming("bv.seller.b1b.com")]
        Seller = 9,
        [Naming("buyer7.ic360.cn")]
        Buyer = 10,
        [Naming("fixed7.ic360.cn")]
        Fixed = 13,
        [Naming("datas0.ic360.cn")]
        Data = 14,
        [Naming("erpservice7.ic360.cn")]
        ErpService = 15,
        [Naming("help7.ic360.cn")]
        HC = 16,
        [Naming("erp7.ic360.cn")]
        ERP = 17,
        [Naming("app.ic360.cn")]
        APP = 18,
        [Naming("www.ic360.cn")]
        IC360 = 19,
        [Naming("www.d9.com.cn")]
        D9 = 20,
        [Naming("ssoapi0.ic360.cn")]
        SSOApi = 100
#elif FINAL
         [Naming("bv.b1b.com")]
        B1B = 1,
        [Naming("bv.sso.b1b.com")]
        SSO = 2,
        [Naming("bv.seller.b1b.com")]
        Seller = 9,
        [Naming("bv.buyer.b1b.com")]
        Buyer = 10,
        [Naming("fixed.ic360.cn")]
        Fixed = 13,
        [Naming("bv.fixed.b1b.com")]
        Data = 14,
        [Naming("erp.service.com")]
        ErpService = 15,
        [Naming("new.help.ic360.cn")]
        HC = 16,
        [Naming("new.yd.51db.com")]
        ERP = 17,
        [Naming("new.app.ic360.cn")]
        APP = 18,
        [Naming("new.www.ic360.cn")]
        IC360 = 19,
        [Naming("new.www.d9.com.cn")]
        D9 = 20,
        [Naming("api.sso.ic360")]
        SSOApi = 100
#endif

    }
}
