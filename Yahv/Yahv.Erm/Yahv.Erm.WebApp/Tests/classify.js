(function ($) {

    $.classify = function (options) {
        var options = $.extend({}, $.classify.defaults, options);
        var urlQuery = $.param(options);
        var openUrl = 'http://hv.erp.b1b.com/PvData/Classify/Product/Edit.aspx?' + urlQuery;

        $.myWindow({
            url: openUrl,
            title: '产品归类',
        });
    };



    $.classify({}, function () {
        ///?????
        //打开下一条
    });

    $.classify = function (data , next) {
        var data = $.extend({}, $.classify.defaults, data);

        //var urlQuery = $.param(options);
        //var openUrl = 'http://hv.erp.b1b.com/PvData/Classify/Product/Edit.aspx?' + urlQuery;

        //$.myWindow({
        //    url: openUrl,
        //    title: '产品归类',
        //});

        //找到 iframe下的cntent 的window 完成刚刚说的开发过程
    };

    $.preClassify = function (current , next) {
        var data = $.extend({}, $.classify.defaults, current);

        //data.

        //var urlQuery = $.param(options);
        //var openUrl = 'http://hv.erp.b1b.com/PvData/Classify/Product/Edit.aspx?' + urlQuery;

        //$.myWindow({
        //    url: openUrl,
        //    title: '产品归类',
        //});

        //找到 iframe下的cntent 的window 完成刚刚说的开发过程
    };


    $.classify.defaults = {
        clientName: '', //客户名称
        clientCode: '', //客户编号/入仓号
        orderedDate: '', //下单日期
        pis: '', //合同发票

        partNumber: '', //型号
        manufacturer: '', //品牌/制造商
        origin: '', //原产地
        unitPrice: '', //单价
        quantity: '', //数量
        currency: '', //币种
        totalPrice: '', //总价
        hsCode: '', //海关编码
        name: '', //报关品名
        taxCode: '', //税务名称
        taxName: '', //税务编码
        importTaxRate: '', //进口关税率
        vatRate: '', //增值税率
        unit: '007', //成交单位
        legalUnit1: '007', //法定第一单位
        legalUnit2: '035', //法定第二单位
        ciqCode: '999', //检验检疫编码
        elements: '', //申报要素
        summary: '', //摘要备注

        ciq: false, //是否商检
        ciqprice: 0, //商检费
        ccc: false, //是否需要3C认证
        embargo: false, //是否禁运
        hkControl: false, //是否香港管制
        highPrice: false, //是否属于高价值产品
        coo: false, //是否需要原产地证明

        callBackUrl: '' //归类完成回调该url，将界面修改后的数据post给相应子系统，子系统根据自己的设计完成数据更新
    };

})(jQuery);