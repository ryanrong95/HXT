/// <reference path="../../jquery-easyui-1.7.6/jquery.easyui.min.js" />
/// <reference path="../../jquery-easyui-1.7.6/jquery.min.js" />
/*
返回值要求：(一下所有的都需要配配合前缀名称)

currencySimple
price1
price2
price3
*/

(function ($) {

    //接口访问
    var AjaxUrl = {
        currencySimple: "/PvDataApi/Currencies",//获取币种数据的接口地址
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
            url: AjaxUrl.currencySimple,
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
    $.fn.currencySimple = function (opt, param) {

        //如果options为string，则是方法调用
        if (typeof opt == 'string') {
            var method = $.fn.currencySimple.methods[opt];
            if (method) {
                return method(this, param);//如果有该方法就调用该方法
            } else {
                alert("该插件没有这个方法:" + opt)
            }
        }

        //情况一，在html中直接配置data-options
        var options = opt || {};
        options = $.extend(true, {}, $.fn.currencySimple.defaults, options);

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
            var initdata = top.$.baseData.Currencies;

            sender.combobox({
                valueField: 'ID',
                textField: 'Symbol',
                prompt: options.prompt,
                required: options.required,
                value: options.value,
                data: initdata,
                editable: false,
                onChange: options.onChange
            });

            return sender;


        });
    }

    //币种插件的默认配置
    $.fn.currencySimple.defaults = {
        prompt: '请选择币种',
        required: true,
        value: top.$.baseData.Currencies[0].ID,
        onChange: function (newValue, oldValue) {

        }
    };

    //币种插件对外的方法
    $.fn.currencySimple.methods = {
        //设置币种
        setcurrencySimple: function (jq, currencySimple) {
            var sender = $(jq);
            var numberboxes = sender.data("numberboxes");
            if (currencySimple == 'CNY') {
                numberboxes.numberbox2.numberbox('readonly', false);
                numberboxes.numberbox3.numberbox('readonly', false);
            } else {
                numberboxes.numberbox2.numberbox('readonly');
                numberboxes.numberbox3.numberbox('readonly');
            }

            var menuid = sender.data('menu').id;

            var og = $.grep(top.$.baseData.Currencies, function (item, index) {
                return item.ShortName == currencySimple;
            });

            var menu = $('#' + menuid).data('sender');
            menu.find('.l-btn-text').html(og[0].Symbol);

            sender.data('currencySimple', og[0].ID);
            //协助触发重新计算
            numberboxes.numberbox1.textbox('textbox').keyup();
            //币种返回值
            numberboxes.currencySimple1.val(og[0].ID);

            return sender;
        },
        //获取币种
        getcurrencySimple: function (jq) {
            var sender = $(jq);
            var name = sender.data('currencySimple');
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
        }

    };

    $.parser.plugins.push('currencySimple'); //将自定义的插件加入 easyui 的插件组    
})(jQuery)
