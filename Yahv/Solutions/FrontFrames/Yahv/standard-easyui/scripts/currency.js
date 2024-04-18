/// <reference path="../../jquery-easyui-1.7.6/jquery.easyui.min.js" />
/// <reference path="../../jquery-easyui-1.7.6/jquery.min.js" />
/*
返回值要求：(一下所有的都需要配配合前缀名称)

currency
price1
price2
price3
*/

(function ($) {
    $.extend($.fn.validatebox.defaults.rules, {
        currency: {
            validator: function (value, param) {
                var price = parseFloat(value);
                var required = param[0];
                if (required) {
                    return price > 0;
                }
                return price >= 0;
            },
            message: '价格必须大于0!'
        }
    });

    //var currencyData = [{ "ID": 1, "Name": "人民币", "ShortName": "CNY", "Symbol": "CNY¥" }, { "ID": 2, "Name": "美元", "ShortName": "USD", "Symbol": "US$" }, { "ID": 3, "Name": "港元", "ShortName": "HKD", "Symbol": "HK$" }];
    //接口访问
    var AjaxUrl = {
        Currency: "/PvDataApi/Currencies",//获取币种数据的接口地址
        getRate: "/PvDataApi/ExchangeRates",//获取汇率(以及换算好的价格,传参不一样)
        getRateAlls: "/PvDataApi/ExchangeRates/" + 'Alls'
    };
    //税率数据
    var taxRates = {
        1: {
            Type: 1,
            Name: "普票税率",
            value: 1.03
        },
        2: {
            Type: 2,
            Name: "专票税率",
            value: 1.13
        },
        4: {
            Type: 4,
            Name: "不开票税率",
            value: 1
        }
    };

    function getCurrencies(cb) {
        $.ajax({
            async: false,
            type: "get",
            url: AjaxUrl.Currency,
            dataType: "json",
            success: function (json) {
                if (cb) {
                    cb(json)
                }
            },
            error: function (err) {
                alert('error:' + JSON.stringify(err));
            }
        });
    }
    function getRateAlls(cb) {
        $.ajax({
            async: false,
            type: "get",
            url: AjaxUrl.getRateAlls,
            dataType: "json",
            success: function (json) {
                if (cb) {
                    cb(json)
                }
            },
            error: function (err) {
                alert('error:' + JSON.stringify(err));
            }
        });
    }
    if (!top.$.baseData) {
        top.$.baseData = {};
    }
    //币种初始化
    if (!top.$.baseData.Currencies) {
        getCurrencies(function (json) {
            top.$.baseData.Currencies = json.data;
        });
    }
    //汇率初始化
    if (!top.$.baseData.RateAlls) {
        getRateAlls(function (json) {
            top.$.baseData.RateAlls = json.data;
            //alert(JSON.stringify(json.data));
        });
    }
    //目标input
    $.fn.currency = function (opt, param) {
        //如果options为string，则是方法调用
        if (typeof opt == 'string') {
            var method = $.fn.currency.methods[opt];
            if (method) {
                return method(this, param);//如果有该方法就调用该方法
            } else {
                alert("该插件没有这个方法:" + opt)
            }
        }
        //情况一，在html中直接配置data-options
        var options = opt || {};
        options = $.extend(true, {}, $.fn.currency.defaults, options);

        return this.each(function () {
            var sender = $(this);

            //保存设置类型
            sender.data('options', options);
            //保存税率类型
            sender.data('taxRates', options.invoiceType);

            var sender_id = sender.prop('id');
            if (!sender_id) {
                sender_id = 'currnecy_sender_' + Math.random().toString().substring(2);
                sender.prop('id', sender_id);
            }


            var menu_id = 'currnecy_menu_' + Math.random().toString().substring(2);
            var menu = $('<div>');
            menu.prop('id', menu_id);
            $(document.body).append(menu);
            menu.data('sender', sender);



            //保存menu信息 setCurrency 方法中调用
            sender.data('menu', {
                id: menu_id
            });

            var isInitCurrency = false;
            var setIntervalInit = 0;
            var initCurrency = function () {
                if (top.$.baseData.Currencies && !isInitCurrency) {
                    isInitCurrency = true;
                    var data = top.$.baseData.Currencies;
                    clearInterval(setIntervalInit);
                    var html = '';
                    for (var index = 0; index < data.length; index++) {
                        var item = data[index];
                        html += '<div onclick="$(\'#' + sender_id + '\').currency(\'setCurrency\', \'' + item.ShortName + '\')">' + item.Symbol + '</div>';
                    }
                    menu.html(html);
                    sender.menubutton({
                        menu: '#' + menu_id
                    });
                    sender.currency('setCurrency', options.currency);

                    if (!options.isShowCurrncy) {
                        sender.hide();
                    }
                }
            };

            //var setIntervalInit = setInterval(initCurrency, 1);

            //默认赋值
            if (typeof (options.Prex) == 'undefined' && !options.Prex) {
                options.Prex = 'price1';
            }

            //建立数字录入框
            var numberbox1 = $('<input value="' + options.value1 + '" name="' + options.Prex + 'price1">');
            var numberbox2 = $('<input value="' + options.value2 + '" name="' + options.Prex + 'price2">');
            var numberbox3 = $('<input value="' + options.value3 + '" name="' + options.Prex + 'price3">');
            var currency1 = $('<input type="hidden" value="' + '' + '" name="' + options.Prex + 'currency1">');
            //汇率
            var eRate = $('<input type="hidden" value="' + '' + '" name="' + options.Prex + 'ERate">');
            //增值税率
            var vatRate = $('<input type="hidden" value="' + '' + '" name="' + options.Prex + 'VATRate">');

            //数字靠右
            numberbox1.css({ 'text-align': 'right' });
            numberbox2.css({ 'text-align': 'right' });
            numberbox3.css({ 'text-align': 'right' });

            //将数字录入框包装在<span></span>中，便于控制样式
            var numberbox1Span = $('<span style="margin-right:10px"></span>').append(numberbox1);
            var numberbox2Span = $('<span style="margin-right:10px"></span>').append(numberbox2);
            var numberbox3Span = $('<span style="margin-right:10px"></span>').append(numberbox3);

            //放入存储在 setCurrency 方法中调用
            sender.data('numberboxes', {
                numberbox1: numberbox1,
                numberbox2: numberbox2,
                numberbox3: numberbox3,
                currency1: currency1,
                vatRate: vatRate,

                numberbox1Span: numberbox1Span,
                numberbox2Span: numberbox2Span,
                numberbox3Span: numberbox3Span
            });



            //增加控件
            sender.after(numberbox3Span).after(numberbox2Span).after(numberbox1Span).after(currency1)
                .after(vatRate).after(eRate);
            
            numberbox1.numberbox({
                events: {
                    'keyup': function (event) {
                        var price = parseFloat($(event.target).val());
                        if (isNaN(price)) {
                            return;
                        }

                        var currency = sender.currency('getCurrency');
                        var rate = 1;
                        //如果已经是人民币就不考虑因此是不等于
                        if (currency != 1) {
                            var og = $.grep(top.$.baseData.RateAlls, function (item, index) {
                                return item.from == currency && item.to == 1;
                            });
                            rate = og[0].rate;
                        }
                        var invoiceType = sender.currency('getInvoiceType');
                        numberbox2.numberbox('setValue', price * rate);
                        numberbox3.numberbox('setValue', price * taxRates[invoiceType].value * rate);
                        vatRate.val(taxRates[invoiceType].value);
                        eRate.val(rate);
                        options.onChange(price, price * rate);
                    }
                },
                min: 0, max: 100000000, precision: options.precision,
                validType: 'currency[' + options.required + ']', required: options.required
            });
            numberbox2.numberbox({
                events: {
                    'keyup': function (event) {

                        //经过测试，确实没有拦阻。
                        //而且，在html中也没有设置真实的input  disable
                        //做提出修改


                        if (event.keyCode == 9) {
                            return;
                        }

                        var price = parseFloat($(event.target).val());
                        if (isNaN(price)) {
                            return;
                        }
                        var currency = sender.currency('getCurrency');
                        var rate = 1;
                        //如果已经是人民币就不考虑因此是不等于
                        if (currency != 1) {
                            var og = $.grep(top.$.baseData.RateAlls, function (item, index) {
                                return item.from == currency && item.to == 1;
                            });
                            rate = og[0].rate;
                        }

                        var invoiceType = sender.currency('getInvoiceType');
                        numberbox1.numberbox('setValue', price);
                        numberbox3.numberbox('setValue', price * taxRates[invoiceType].value);
                        vatRate.val(taxRates[invoiceType].value);
                        options.onChange(price, price);
                        eRate.val(rate);
                    }
                },
                min: 0, max: 100000000, precision: options.precision, label: 'CNY(未税)', labelWidth: 63, width: 123 + 63,
                validType: 'currency[' + options.required + ']', required: options.required
            });
            numberbox3.numberbox({
                events: {
                    'keyup': function (event) {

                        //alert('price3:' + price);

                        var invoiceType = sender.currency('getInvoiceType');
                        var price = parseFloat($(event.target).val());
                        if (isNaN(price)) {
                            return;
                        }

                        var currency = sender.currency('getCurrency');
                        var rate = 1;
                        //如果已经是人民币就不考虑因此是不等于
                        if (currency != 1) {
                            var og = $.grep(top.$.baseData.RateAlls, function (item, index) {
                                return item.from == currency && item.to == 1;
                            });
                            rate = og[0].rate;
                        }

                        price = price / taxRates[invoiceType].value;
                        numberbox1.numberbox('setValue', price);
                        numberbox2.numberbox('setValue', price);
                        vatRate.val(taxRates[invoiceType].value);
                        options.onChange(price, price);

                        eRate.val(rate);
                    }
                }, min: 0, max: 100000000, precision: options.precision, label: 'CNY(含税)', labelWidth: 63, width: 123 + 63,
                validType: 'currency[' + options.required + ']', required: options.required
            });

            initCurrency();

            //设置默认初始值
            var currency = sender.currency('getCurrency');
            currency1.val(currency);

            if ($.trim(options.value1) == ''
                && $.trim(options.value2) == ''
                && $.trim(options.value3) == '') {
                numberbox1.numberbox('setValue', '');
                numberbox2.numberbox('setValue', '');
                numberbox3.numberbox('setValue', '');
            }

            return sender;
        });
    }

    //币种插件的默认配置
    $.fn.currency.defaults = {
        Prex: '',
        currency: "CNY",        //币种1
        invoiceType: 1,         //发票类型
        value1: '',           //币种1的价格值
        value2: '',           //不要用，不知道为什么都是计算的还要赋值？
        value3: '',          //不要用，不知道为什么都是计算的还要赋值？
        precision: 5,       //小数点后保存5位
        isShowCurrncy: true, //显示币种选择
        required: true, //是否必填
        onChange: function (price1, price2) { //当可输入框变更时候

        }
    };

    //币种插件对外的方法
    $.fn.currency.methods = {
        //设置币种
        setCurrency: function (jq, currency) {
            var ogc = $.grep(top.$.baseData.Currencies, function (item, index) {
                return item.ShortName == currency || item.ID == currency;
            });

            //debugger;

            currency = ogc[0].ShortName;

            var sender = $(jq);
            var numberboxes = sender.data("numberboxes");
            if (currency == 'CNY') {
                numberboxes.numberbox2.numberbox('readonly', false);
                numberboxes.numberbox3.numberbox('readonly', false);

                numberboxes.numberbox2Span.hide();
                numberboxes.numberbox3Span.show();
            } else {
                numberboxes.numberbox2.numberbox('readonly');
                numberboxes.numberbox3.numberbox('readonly');

                numberboxes.numberbox2Span.show();
                numberboxes.numberbox3Span.hide();
            }

            var menuid = sender.data('menu').id;

            var og = $.grep(top.$.baseData.Currencies, function (item, index) {
                return item.ShortName == currency || item.ID == currency;
            });

            var menu = $('#' + menuid).data('sender');
            menu.find('.l-btn-text').html(og[0].Symbol);

            sender.data('currency', og[0].ID);

            var price3 = sender.data('options').value3;
            if (price3 > 0) {
                numberboxes.numberbox3.textbox('textbox').keyup();
            } else {
                //协助触发重新计算
                numberboxes.numberbox1.textbox('textbox').keyup();
            }
            //币种返回值
            numberboxes.currency1.val(og[0].ID);

            return sender;
        },
        //获取币种
        getCurrency: function (jq) {
            var sender = $(jq);
            var name = sender.data('currency');
            return name;
        },
        //设置税率类型
        setInvoiceType: function (jq, type) {
            var sender = $(jq);
            sender.data('taxRates', type);
            var numberboxes = sender.data("numberboxes");
            //协助触发重新计算
            numberboxes.numberbox1.textbox('textbox').keyup();
            return sender;
        },
        //设置税率类型
        getInvoiceType: function (jq) {
            var sender = $(jq);
            var name = sender.data('taxRates');
            return name;
        },
        getPrice1: function (jq) {
            var sender = $(jq);
            var numberboxes = sender.data("numberboxes");
            var price1 = numberboxes.numberbox2.numberbox('getValue');
            return price1;
        },
        getPrice: function (jq) {
            var sender = $(jq);
            var numberboxes = sender.data("numberboxes");
            var price = numberboxes.numberbox1.numberbox('getValue');
            return price;
        },
    };

    $.parser.plugins.push('currency'); //将自定义的插件加入 easyui 的插件组    
})(jQuery)
