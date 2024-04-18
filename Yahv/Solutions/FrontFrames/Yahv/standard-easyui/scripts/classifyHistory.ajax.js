(function ($) {
    $.classifyAjax = function (options, fun) {
        var options = $.extend(true, {}, $.classifyAjax.defaults, options);
        var openUrl, title;
        openUrl = $.classifyAjax.conts.openUrl;
        top.window['topdata'] = options;

        $.myWindow({
            width: '90%',
            height: '70%',
            url: openUrl,
            title: '归类历史数据',
            onClose: function () {
                if (fun && fun.onClose) {
                    fun.onClose();
                }
            }
        });
    };

    $.classifyAjax.defaults = {
        PartNumber: '', //型号
        Manufacturer: '', //品牌/制造商
        HSCode: '', //海关编码
        TariffName: '', //报关品名
        TaxCode: '', //税务名称
        TaxName: '', //税务编码
        ImportPreferentialTaxRate: '',//优惠税率
        VATRate: '', //增值税率
        ExciseTaxRate:'',//消费税率
        LegalUnit1: '', //法定第一单位
        LegalUnit2: '', //法定第二单位
        CIQCode: '', //检验检疫编码
        Elements: '', //申报要素

        CIQ: false, //是否商检
        CIQprice: 0, //商检费
        Ccc: false, //是否需要3C认证
        Embargo: false, //是否禁运
        HkControl: false, //是否香港管制
        Coo: false, //是否需要原产地证明
    };

    $.classifyAjax.conts = {
        openUrl: '/PvData/SysConfig/ClassifyHistory/Edit.html'
    };
})(jQuery);

