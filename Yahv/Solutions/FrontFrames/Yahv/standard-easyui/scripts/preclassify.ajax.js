(function ($) {
    $.preclassifyAjax = function (options, fun, otherOptions) {
        var options = $.extend(true, {}, $.preclassifyAjax.defaults, options);
        var openUrl, title;
        openUrl = $.preclassifyAjax.conts.openUrl;  //+ '?callBackUrl=' + options.callBackUrl;
        //如果没有数据就ajax请求数据
        if (options == null && otherOptions && otherOptions.nextUrl) {
            $.post(otherOptions.nextUrl, function (data) {
                options = data;
            });
        }
        top.window['topdata'] = options;

        //如果是继续归类，就要传这个值otherOptions.openTimes
        if (otherOptions && otherOptions.openTimes) {
            $.myWindow.close2();
        }

        //如果是芯达通系统，则用人家原来的那种myWindow
        if (otherOptions && otherOptions.xdt) {
            top.$.myWindow({
                url: openUrl,
                title: '产品预归类',
                operaIframe: function (iframe) {
                    //var $form = $('<form></form>');
                    //$.each(options, function (key, value) {
                    //    var $input = $('<input type="hidden" />');
                    //    $input.attr('name', key).val(value);
                    //    $form.append($input);
                    //});
                    //var timer2 = setTimeout(function () {
                    //    if (iframe[0].contentDocument) {
                    //        var wIframe = iframe.contents();
                    //        if (wIframe.find('body').length == 1) {
                    //            if (wIframe.find('body').find('form').length == 2) {
                    //                clearInterval(timer2);
                    //            } else if (wIframe.find('body').find('form').length == 1) {
                    //                wIframe.find('body').append($form);
                    //                clearInterval(timer2);
                    //            }
                    //        }
                    //    }
                    //}, 200)
                },
                onClose: function () {
                    if (fun && fun.onClose) {
                        fun.onClose();
                    }
                }
            })
        } else {
            $.myWindow({
                width: '90%',
                height: '95%',
                url: openUrl,
                title: '产品预归类',
                operaIframe: function (iframe) {
                    //iframe[0].src = openUrl;

                    //$.get(openUrl, function (html) {
                    //    iframe[0]['data'] = options;
                    //    iframe[0].contentDocument.write(html);
                    //    //iframe[0].document.write(html);
                    //});

                    //var $form = $('<form></form>');
                    //$.each(options, function (key, value) {
                    //    var $input = $('<input type="hidden" />');
                    //    $input.attr('name', key).val(value);
                    //    $form.append($input);
                    //});

                    //var timer2 = setTimeout(function () {
                    //    if (iframe[0].contentDocument) {
                    //        var wIframe = iframe.contents();
                    //        if (wIframe.find('body').length == 1) {
                    //            if (wIframe.find('body').find('form').length == 2) {
                    //                clearInterval(timer2);
                    //            } else if (wIframe.find('body').find('form').length == 1) {
                    //                wIframe.find('body').append($form);
                    //                clearInterval(timer2);
                    //            }
                    //        }
                    //    }
                    //}, 200)
                },
                onClose: function () {
                    if (fun && fun.onClose) {
                        fun.onClose();
                    }
                }
            });
        }


    };

    $.preclassifyAjax.defaults = {
        ClientName: '', //客户名称
        ClientCode: '', //客户编号/入仓号
        ProductUnionCode: '',//单据号
        PIs: '', //合同发票
        Supplier: '',//供应商
        MainID: '',//预归类产品的ID

        PartNumber: '', //型号
        Manufacturer: '', //品牌/制造商
        Origin: '', //原产地
        UnitPrice: '', //单价
        Quantity: '', //数量
        Currency: '', //币种
        HSCode: '', //海关编码
        TariffName: '', //报关品名
        TaxCode: '', //税务名称
        TaxName: '', //税务编码
        ImportPreferentialTaxRate: '',//优惠税率
        OriginATRate: '',//加征税率
        VATRate: '', //增值税率
        ExciseTaxRate: '',//消费税率
        LegalUnit1: '', //法定第一单位
        LegalUnit2: '', //法定第二单位
        CIQCode: '', //检验检疫编码
        Elements: '', //申报要素
        Summary: '', //摘要备注

        CIQ: false, //是否商检
        CIQprice: 0, //商检费
        Ccc: false, //是否需要3C认证
        Embargo: false, //是否禁运
        HkControl: false, //是否香港管制
        IsHighPrice: false, //是否属于高价值产品
        Coo: false, //是否需要原产地证明
        CallBackUrl: '', //归类完成回调该url，将界面修改后的数据post给相应子系统，子系统根据自己的设计完成数据更新
        //最好使用 reffer

        PvDataApiUrl: '',//编辑页面获取数据的接口
        NextUrl: ''//获取下一条数据的接口地址
    };

    $.preclassifyAjax.conts = {
        openUrl: '/PvData/PreClassify/Edit.html'
    };
})(jQuery);

