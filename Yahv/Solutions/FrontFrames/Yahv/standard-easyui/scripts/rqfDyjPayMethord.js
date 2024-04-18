/// <reference path="../../jquery-easyui-1.7.6/jquery.easyui.min.js" />
/// <reference path="../../jquery-easyui-1.7.6/jquery.min.js" />
/*
返回值要求：(一下所有的都需要配配合前缀名称)

rqfDyjPayMethord
price1
price2
price3
*/

(function ($) {

    //接口访问
    var AjaxUrl = {
        rqfDyjPayMethord: '/PvDataApi/Enums/RqfDyjPayMethord/'
    };

    function getRqfDyjPayMethord(cb) {
        $.ajax({
            async: false,
            type: "get",
            url: AjaxUrl.rqfDyjPayMethord,
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
    if (!top.$.baseData.RqfDyjPayMethord) {
        getRqfDyjPayMethord(function (json) {
            top.$.baseData.RqfDyjPayMethord = json;
        });
    }

    //目标input
    $.fn.rqfDyjPayMethord = function (opt, param) {

        //如果options为string，则是方法调用
        if (typeof opt == 'string') {
            var method = $.fn.rqfDyjPayMethord.methods[opt];
            if (method) {
                return method(this, param);//如果有该方法就调用该方法
            } else {
                alert("该插件没有这个方法:" + opt)
            }
        }

        //情况一，在html中直接配置data-options
        var options = opt || {};
        options = $.extend(true, {}, $.fn.rqfDyjPayMethord.defaults, options);

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
            var data = top.$.baseData.RqfDyjPayMethord;

            sender.combobox({
                valueField: 'ID',
                textField: 'Name',
                prompt: options.prompt,
                required: options.required,
                value: options.value,
                data: data,
                editable: false,
                onChange: options.onChange
            });

            if (options.onChange) {
                options.onChange(options.value, null);
            }

            return sender;


        });
    };

    //币种插件的默认配置
    $.fn.rqfDyjPayMethord.defaults = {
        prompt: '请选择付款方式',
        required: true,
        value: top.$.baseData.RqfDyjPayMethord[0].ID,
        onChange: function (newValue, oldValue) {

        }
    };

    //币种插件对外的方法
    $.fn.rqfDyjPayMethord.methods = {
        //设置币种
        setrqfDyjPayMethord: function (jq, rqfDyjPayMethord) {
            var sender = $(jq);
            var numberboxes = sender.data("numberboxes");
            if (rqfDyjPayMethord == 'CNY') {
                numberboxes.numberbox2.numberbox('readonly', false);
                numberboxes.numberbox3.numberbox('readonly', false);
            } else {
                numberboxes.numberbox2.numberbox('readonly');
                numberboxes.numberbox3.numberbox('readonly');
            }

            var menuid = sender.data('menu').id;

            var og = $.grep(top.$.baseData.Currencies, function (item, index) {
                return item.ShortName == rqfDyjPayMethord;
            });

            var menu = $('#' + menuid).data('sender');
            menu.find('.l-btn-text').html(og[0].Symbol);

            sender.data('rqfDyjPayMethord', og[0].ID);
            //协助触发重新计算
            numberboxes.numberbox1.textbox('textbox').keyup();
            //币种返回值
            numberboxes.rqfDyjPayMethord1.val(og[0].ID);

            return sender;
        },
        //获取币种
        getrqfDyjPayMethord: function (jq) {
            var sender = $(jq);
            var name = sender.data('rqfDyjPayMethord');
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


        //获取数据
        getValue: function (jq) {
            var sender = $(jq);
            return sender.combobox('getValue');
        }


    };

    $.parser.plugins.push('rqfDyjPayMethord'); //将自定义的插件加入 easyui 的插件组    
})(jQuery)
