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
        origin: "/PvDataApi/Enums/Origins/"
    };

    function getData(cb) {
        $.ajax({
            async: false,
            type: "get",
            url: AjaxUrl.origin,
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
    if (!top.$.baseData.Origins) {
        getData(function (json) {
            top.$.baseData.Origins = json;
        });
    }

    $.extend($.fn.validatebox.defaults.rules, {
        origin: {
            validator: function (value, param) {
                var sender = $('#' + param[0]);
                var data = sender.combobox('getData');
                if (data == null) {
                    return false;
                }
                var id = sender.val();
                var og = $.grep(data, function (item, index) {
                    //console.log([item.Format, value]);
                    return $.trim(item.Format) == $.trim(value) || item.Format.indexOf($.trim(value)) >= 0;
                });
                return og.length > 0;
            },
            message: '必须选择规范原产地'
        }
    });

    $.fn.origin = function (opt, param) {

        //如果options为string，则是方法调用
        if (typeof opt == 'string') {
            var method = $.fn.origin.methods[opt];
            if (method) {
                return method(this, param);//如果有该方法就调用该方法
            } else {
                alert("origin.该插件没有这个方法:" + opt)
            }
        }

        //情况一，在html中直接配置data-options
        var options = opt || {};
        options = $.extend(true, {}, $.fn.origin.defaults, options);

        var data = null;
        var senders = this;
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
                    loader: function (param, success, error) {
                        var q = $.trim(sender.combobox('getText'));


                        var items = $.grep(data, function (item, index) {
                            return item.Code.indexOf(q) >= 0
                                || item.ChineseName.indexOf(q) >= 0
                                || item.EnglishName.indexOf(q) >= 0
                        });

                        success(data);
                        sender.combobox('isValid');
                    },
                    valueField: 'Code',
                    textField: 'Format',
                    formatter: function (row) {
                        var s = '<div>' + row.Code + '(' + row.ChineseName + ')</div>' +
                                '<div>' + row.EnglishName + '</div>'
                        ;
                        return s;
                    },
                    prompt: options.prompt,
                    required: options.required,
                    value: options.value,
                    data: data,
                    editable: true,
                    onChange: options.onChange,
                    validType: 'origin["' + sender_id + '"]'
                });
            });
        };
        return init(senders, top.$.baseData.Origins);
    }

    //币种插件的默认配置
    $.fn.origin.defaults = {
        prompt: '选择原产地',
        required: false,
        value: '',
        onChange: function (newValue, oldValue) {

        }
    };

    //币种插件对外的方法
    $.fn.origin.methods = {
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

    $.parser.plugins.push('origin'); //将自定义的插件加入 easyui 的插件组    
})(jQuery)
