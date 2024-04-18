(function ($) {
    $.classify = function (options, options2) {
        var options = $.extend(true, {}, $.classify.defaults, options);
        var openUrl,title;
        if (options2) {
            openUrl = options2.openUrl || $.classify.conts.openUrl;  //+ '?callBackUrl=' + options.callBackUrl;
            title = options2.title || '产品归类';
        } else {
            openUrl = $.classify.conts.openUrl;  //+ '?callBackUrl=' + options.callBackUrl;
            title = '产品归类';
        }

        $.myWindow({
            url: '',
            title: title,
            operaIframe: function (iframe) {
                var $form = $('<form></form>');
                $form.attr('action', openUrl);
                $form.attr('method', 'post');
                $.each(options, function (key, value) {
                    var $input = $('<input type="hidden" />');
                    $input.attr('name', key).val(value);
                    $form.append($input);
                });

                var wIframe = iframe.contents();

                var timer2 = setInterval(function () {
                    if (wIframe.find('body').length == 1) {
                        if ($.trim(wIframe.find('body').html()) == '') {
                            wIframe.find('body').append($form);
                            setTimeout(function () {
                                $form.submit();
                            }, 1);
                        }
                    } else {
                        clearInterval(timer2);
                    }
                }, 100)

            }
        });
    };

    $.classify.defaults = {
        clientName: '', //客户名称
        clientCode: '', //客户编号/入仓号
        orderedDate: '', //下单日期
        pis: '', //合同发票
        MainID: '', //MainID  可以是 OrderID PreclassifyID OtherID
        ItemID: '', //OrderItemID
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
        //最好使用 reffer
    };

    $.classify.conts = {
        openUrl: 'http://hv.erp.b1b.com/PvData/Classify/Edit.aspx',
    };

    //归类成功后跳转方法
    //values归类数据
    //归类后跳转的子系统链接地址
    $.classify.postback=function(values, openUrl) {
        $('body').empty();
        var $form = $('<form></form>');
        $form.attr('action', openUrl);
        $form.attr('method', 'post');
        $.each(values, function (key, value) {
            var $input = $('<input type="hidden" />');
            $input.attr('name', key).val(value);
            $form.append($input);
        });
        $('body').append($form);
        $form.submit();
    }
})(jQuery);