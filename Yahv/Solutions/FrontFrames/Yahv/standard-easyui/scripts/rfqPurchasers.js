/// <reference path="../../jquery-easyui-1.7.6/jquery.easyui.min.js" />
/// <reference path="../../jquery-easyui-1.7.6/jquery.min.js" />
/*
询报价专用控件
获取提交人的询价人
也是：提交的人被代理人
真是说不太清楚，不过终于算是明白了

目前采用异步写法
*/

(function ($) {
    //接口访问
    var AjaxUrl = {
        rfqPurchasers: "/rfqapi/Admins/GetPurchasers/"
    };

    function getData(cb) {
        $.ajax({
            type: "get",
            url: AjaxUrl.rfqPurchasers,
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

    $.extend($.fn.validatebox.defaults.rules, {
        rfqPurchasers: {
            validator: function (value, param) {
                var sender = $('#' + param[0]);
                var data = sender.combobox('getData');


                if (data == null) {
                    return false;
                }


                var id = sender.val();
                var og = $.grep(data, function (item, index) {
                    return $.trim(item.Name) == $.trim(value);//|| item.id == id;
                });


                return og.length > 0;
            },
            message: '必须选择现有人员!'
        }
    });

    $.fn.rfqPurchasers = function (opt, param) {

        //如果options为string，则是方法调用
        if (typeof opt == 'string') {
            var method = $.fn.rfqPurchasers.methods[opt];
            if (method) {
                return method(this, param);//如果有该方法就调用该方法
            } else {
                alert("rfqPurchasers.该插件没有这个方法:" + opt)
            }
        }

        //情况一，在html中直接配置data-options
        var options = opt || {};
        options = $.extend(true, {}, $.fn.rfqPurchasers.defaults, options);

        var data = null;
        var senders = this;
        getData(function (json) {
            init(senders, json)
        });
        var init = function (senders, data) {
            return senders.each(function () {
                var sender = $(this);

                //保存设置类型
                sender.data('options', options);
                //保存税率类型

                var sender_id = sender.prop('id');
                if (!sender_id) {
                    sender_id = 'currnecy_sender_' + Math.random().toString().substring(2);
                    sender.prop('id', sender_id);
                }

                sender.combobox({
                    valueField: 'ID',
                    textField: 'Name',
                    prompt: options.prompt,
                    required: options.required,
                    value: options.value,
                    data: data,
                    editable: true,
                    onChange: options.onChange,
                    validType: 'rfqPurchasers["' + sender_id + '"]'
                });
            });
        };
        return this;
    }

    //币种插件的默认配置
    $.fn.rfqPurchasers.defaults = {
        prompt: '选择采购人',
        required: false,
        value: '',
        onChange: function (newValue, oldValue) {

        }
    };

    //币种插件对外的方法
    $.fn.rfqPurchasers.methods = {
        tester: function (jq, type) {
            var sender = $(jq);
            return sender;
        },
        //设置必填非必谈
        setRequired: function (jq, required) {
            var sender = $(jq);
            sender.combobox({ required: true });
            return sender;
        }
    };

    $.parser.plugins.push('rfqPurchasers'); //将自定义的插件加入 easyui 的插件组    
})(jQuery)
