//如果价格，未税，含税同时存在，则按照价格计算
//如果价格为0或者null，未税，含税同时存在，则按照未税计算
//只有含税，则按照含税计算
(function ($) {
    //接口url
    var AjaxUrl = {
        getRate: "/PvDataApi/ExchangeRates"//获取汇率(以及换算好的价格,传参不一样)
    };

    function digitalConversion(num) {
        return num.replace(/,/g, '');
    }
    //从接口中获取汇率
    function getRate(from, to, type, cb) {
        $.ajax({
            type: "get",
            url: AjaxUrl.getRate,
            //dataType: "jsonp",
            data: {
                from: from,
                to: to,
                type: type
            },
            success: function (data) {
                cb(data.data);
            },
            error: function (err) {
                alert(err);
            }
        });
    }
    //获取换算结果以及汇率
    function getPriceResult(from, to, price, type, cb) {
        $.ajax({
            type: "get",
            url: AjaxUrl.getRate,
            //dataType: "jsonp",
            data: {
                from: from,
                to: to,
                price: price,
                type: type
            },
            success: function (data) {
                cb(data.data);
            },
            error: function (err) {
                alert(err);
            }
        });
    }
    //存储汇率
    function saveERate(sender, ERate) {
        $(sender).data('ERate', ERate);
        $($(sender).data('dom_ERate')).val(ERate);
    }
    //用户onEnter事件
    function enterFun(options, P1, P2, P3, ERate, VATRate) {
        if (options.onEnter) {
            options.onEnter(options, P1, P2, P3, ERate, VATRate);
        }
    }
    //在非CNY的情况下，P2计算P1
    function computP1ByP2(sender, options, currency1, P2, P3) {
        getPriceResult('CNY', currency1, P2, options.rateType, function (data) {
            var P1 = data.price;
            $(options.target1).numberbox('setValue', P1);
            ERate = data.rate;
            saveERate(sender, ERate);
            enterFun(options, P1, P2, P3, ERate, VATRate);
        })
    }
    //根据P2计算P3
    function computP3ByP2(options, P2) {
        var P3 = ConvertToTaxPrice(2, P2);
        $(options.target3).numberbox('setValue', P3);
        return P3;
    }
    //根据P3计算P1,P2
    function computP1P2ByP3(sender, P3) {
        var options = $(sender).data('options');
        var P2 = ConvertToPrice(2, P3);
        $(options.target2).numberbox('setValue', P2);
        var currency1 = $(options.target1).data('options').currency;
        var P1;
        if (currency1 == 'CNY') {
            P1 = P2;
            $(options.target1).numberbox('setValue', P1);
            saveERate(options.target1, 1);
            enterFun(options, P1, P2, P3, 1, VATRate);
        } else {
            computP1ByP2(sender, options, currency1, P2, P3);
        }
    }
    //根据P2计算P1,P3
    function computP1P3ByP2(sender, P2) {
        var options = $(sender).data('options');
        var P3 = computP3ByP2(options, P2);
        var P1;
        var currency1 = $(options.target1).data('options').currency;
        if (currency1 == 'CNY') {
            P1 = P2;
            $(options.target1).numberbox('setValue', P1);
            saveERate(options.target1, 1);
            enterFun(options, P1, P2, P3, 1, VATRate);
        } else {
            computP1ByP2(sender, options, currency1, P2, P3);
        }
    }
    //根据P1计算P2,P3
    function computP2P3ByP1(sender, P1) {
        var options = $(sender).data('options');
        var currency = options.currency;
        var ERate, P2;
        if (currency == 'CNY') {
            ERate = 1;
            P2 = P1;
            $(options.target2).numberbox('setValue', P2);
            if (options.isNeedERate) {
                saveERate(sender, ERate);
            }
            var P3 = computP3ByP2(options, P2);
            enterFun(options, P1, P2, P3, ERate, VATRate);
        } else {
            getPriceResult(currency, 'CNY', P1, options.rateType, function (data) {
                //getPriceResult(currency, 'CNY', digitalConversion(P1), function (data) {
                ERate = data.rate;
                P2 = data.price;
                saveERate(sender, ERate);
                $(options.target2).numberbox('setValue', P2);
                var P3 = computP3ByP2(options, P2);
                enterFun(options, P1, P2, P3, ERate, VATRate);
            })
        }
    }
    //初始化flag表示是否为初始化
    function init(sender, price, flag) {
        var options = $(sender).data('options');
        var p1, p2, p3;
        if (options.target1 != null) {
            p1 = $(options.target1).data('options').value;
        }
        if (options.target2 != null) {
            p2 = $(options.target2).data('options').value;
        }
        if (options.target3 != null) {
            p3 = $(options.target3).data('options').value;
        }
        if (price && price != '') {
            if (options.currency == null) {
                options.currency = 'CNY'
            }
            if (options.target1 == null && options.target2 != null && options.target3 != null) {
                computP2P3ByP1(sender, price);
            } else if (options.target1 != null && options.target2 == null && options.target3 != null) {
                if (flag) {
                    if (p1 == null || p1 == 0) {
                        computP1P3ByP2(sender, price);
                    } else {
                        computP2P3ByP1(sender, p1);
                    }
                } else {
                    computP1P3ByP2(sender, price);
                }
            } else if (options.target1 != null && options.target2 != null && options.target3 == null) {
                if (flag) {
                    if ((p1 == null || p1 == 0) && (p2 == null || p2 == 0)) {
                        computP1P2ByP3(sender, price);
                    } else if ((p1 != null && p1 != 0 && p1 != '') && (p2 == null || p2 == 0)) {
                        computP2P3ByP1(sender, p1);
                    } else if ((p2 != null && p2 != 0 && p2 != '') && (p1 == null || p1 == 0)) {
                        computP1P3ByP2(sender, p2);
                    }
                } else {
                    computP1P2ByP3(sender, price);
                }
            }
        }
    }

    //默认专票税率1.13
    var VATRate = 1.13;
    //目标input
    $.fn.numberbox2 = function (opt, param) {
        //如果options为string，则是方法调用
        if (typeof opt == 'string') {
            var method = $.fn.numberbox2.methods[opt];
            if (method) {
                return method(this, param);//如果有该方法就调用该方法
            } else {
                alert("该插件没有这个方法");
            }
        }

        //情况一，在html中直接配置data-options
        var options = opt || {};
        var sender = this;
        sender.css({ 'text-align': 'right' });

        //html配置
        (function () {
            var data_options = {};
            var s = $.trim(sender.attr("data-options"));
            if (s) {
                if (s.substring(0, 1) != "{") {
                    s = "{" + s + "}";
                }
                data_options = (new Function("return " + s))();
                options = $.extend(true, {}, $.fn.numberbox2.defaults, data_options, options);
            } else {
                options = $.extend(true, {}, $.fn.numberbox2.defaults, options);
            }
        })();

        if ((options.value != null && options.value == "") || (options.value == null)) {
            options.value = 0;
        }
        //存储options
        $(sender).data('options', options);
        $(sender).data('finished', false);
        var groupName = $(this).attr("name");//获取this的name值
        if (options.isNeedERate) {
            var dom_ERate = $('<input type="hidden" name="' + groupName + '_ERate" />');
            $(sender).before(dom_ERate);
            $(sender).data('dom_ERate', dom_ERate);
        }
        if (options.isNeedVATRate) {
            var dom_VATRate = $('<input type="hidden" name="' + groupName + '_VATRate" />');
            $(sender).before(dom_VATRate);
            $(sender).data('dom_VATRate', dom_VATRate);
            $($(sender).data('dom_VATRate')).val(VATRate);
            $(sender).data('VATRate', VATRate);
        }
        //实例化插件
        sender.numberbox(options);
        $(sender).data('finished', true);
        if (options.finish) {
            options.finish();
            init(sender, options.value, true);
            if (options.value == 0) {
                if (options.target1 == null && options.target2 != null && options.target3 != null) {
                    var p2 = $(options.target2).data('options').value;
                    var p3 = $(options.target3).data('options').value;
                    if (p2 == 0 && p3 == 0) {
                        if (options.currency == 'CNY') {
                            saveERate(sender, 1);
                        } else {
                            getRate(options.currency, 'CNY', options.rateType, function (ERate) {
                                saveERate(sender, ERate);
                            })
                        }
                    }
                }
            }
        }
        //含税的输入框
        function keyupFun() {
            var price = $(this).val();
            init(sender, price);
        }
        $(sender).numberbox('textbox').on("keyup", debounce(keyupFun, 200));
    }

    //币种插件的默认配置
    $.fn.numberbox2.defaults = $.extend({}, $.fn.numberbox.defaults, $.fn.numberbox.defaults, {
        currency: 'CNY',
        rateType: 30, //汇率类型（默认固定汇率） 海关汇率：10，实时汇率/浮动汇率 20，固定汇率 30，预设汇率 40
        value: null,
        readonly: false,
        isNeedERate: false,
        isNeedVATRate: false,
        target1: null,
        target2: null,
        target3: null,
        finish: null
    })

    //币种插件对外的方法
    $.fn.numberbox2.methods = {
        options: function (jq) {
            return $(jq).numberbox("options");
        },
        readonly: function (jq, mode) {
            $(jq).numberbox("readonly", mode);
        },
        setValue: function (jq, param) {
            $(jq).numberbox("setValue", param);
        },
        getERate: function (jq) {
            var ERate = null;
            if ($(jq).data('options').isNeedERate) {
                ERate = $(jq).data('ERate');
            }
            return ERate;
        },
        getVATRate: function (jq) {
            var VATRate = null;
            if ($(jq).data('options').isNeedVATRate) {
                VATRate = $(jq).data('VATRate');
            }
            return VATRate;
        },
        getAllData: function (jq) {
            return {
                options: $(jq).data('options'),
                ERate: $(jq).data('ERate'),
                VATRate: $(jq).data('VATRate'),
                price: $(jq).numberbox('getValue')
            }
        }
    };
    $.parser.plugins.push('numberbox2'); //将自定义的插件加入 easyui 的插件组    
})(jQuery)