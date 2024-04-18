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
        rfqAgents: "/rfqapi/Admins/GetAgents/"
    };

    function getAgents(cb) {
        $.ajax({
            //async: false,
            type: "get",
            url: AjaxUrl.rfqAgents,
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

    $.fn.rfqAgents = function (opt, param) {

        //如果options为string，则是方法调用
        if (typeof opt == 'string') {
            var method = $.fn.rfqAgents.methods[opt];
            if (method) {
                return method(this, param);//如果有该方法就调用该方法
            } else {
                alert("rfqAgents.该插件没有这个方法:" + opt)
            }
        }

        //情况一，在html中直接配置data-options
        var options = opt || {};
        options = $.extend(true, {}, $.fn.rfqAgents.defaults, options);

        var data = null;
        var senders = this;
        getAgents(function (json) {
            init(senders, json)
        });
        var init = function (senders, data) {
            senders.hide();
            if (data.IsMessage) {
                return senders.each(function () {
                    var sender = $(this);
                    var html = '<span>' + data.Admin.Name + '</span>';
                    sender.after(html);
                });
            } else {
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
                    var initdata = data.data;

                    sender.combobox({
                        valueField: 'ID',
                        textField: 'Name',
                        prompt: options.prompt,
                        required: options.required,
                        value: options.value,
                        data: initdata,
                        editable: false,
                        onChange: options.onChange
                    });
                });
            }
        };
        return this;
    }

    //币种插件的默认配置
    $.fn.rfqAgents.defaults = {
        prompt: '选择询价人',
        required: false,
        value: '',
        onChange: function (newValue, oldValue) {

        }
    };

    //币种插件对外的方法
    $.fn.rfqAgents.methods = {
        tester: function (jq, type) {
            var sender = $(jq);
            return sender;
        }
    };

    $.parser.plugins.push('rfqAgents'); //将自定义的插件加入 easyui 的插件组    
})(jQuery)
