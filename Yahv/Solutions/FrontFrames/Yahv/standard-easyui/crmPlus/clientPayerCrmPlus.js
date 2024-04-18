/// <reference path="../../jquery-easyui-1.7.6/jquery.easyui.min.js" />
/// <reference path="../../jquery-easyui-1.7.6/jquery.min.js" />

(function ($) {
    //接口地址
    var ajaxUrl = {
        getClientPayers: '/crmplusapi/Clients/Payers',
    }

    //仅供内部调用的方法
    var innerMethods = function () { };
    innerMethods.prototype = {

        //ajax请求客户付款人数据
        getClientPayers: function (sender, value, success) {
            sender.combobox('clear');
            sender.combobox('loadData', []);

            var options = sender.data('options');
            $.ajax({
                url: ajaxUrl.getClientPayers,
                dataType: 'json',
                type: 'GET',
                data: { id: value },
                success: function (json) {
                    var items = json.data;
                    sender.data('items', items);
                    //sender.combobox('loadData', items);
                    success(items);
                },
                error: function (err) {
                    alert('clientPayer.error:' + JSON.stringify(err));
                }
            });
        },

    }

    $.fn.clientPayerCrmPlus = function (opt, param) {
        //如果options为string，则是方法调用
        if (typeof opt == 'string') {
            var method = $.fn.clientPayerCrmPlus.methods[opt];
            if (method) {
                return method(this, param);//如果有该方法就调用该方法
            } else {
                alert("clientPayer.插件没有这个方法: " + opt)
            }
        }

        var options = opt || {};
        options = $.extend(true, {}, $.fn.clientPayerCrmPlus.defaults, options);

        return this.each(function () {
            var sender = $(this);
            //保存设置类型
            sender.data('options', options);

            var myMethods = new innerMethods();

            //获取控件id
            var sender_id = sender.prop('id');
            if (!sender_id) {
                sender_id = 'clientPayer_sender_' + Math.random().toString().substring(2);
                sender.prop('id', sender_id);
            }

            //控件属性验证
            var formName = sender.prop('name');
            if (!formName) {
                alert('clientPayer.控件name不能为空!');
            }

            sender.combobox({
                loader: function (param, success, error) {
                    if (options.clientID) {
                        myMethods.getClientPayers(sender, options.clientID, success);
                    }
                },
                mode: 'remote',
                valueField: 'ID',
                textField: 'EnterpriseName',
                formatter: function (row) {
                    var s = '<div class="supplier-payer">' +
                        '<div><em>企业:</em>' + row.EnterpriseName + '</div>' +
                        '<div><em>开户行:</em>' + row.Bank + '</div>' +
                        '<div><em>开户行地址:</em>' + row.BankAddress + '</div>' +
                        '<div><em>支付方式:</em>' + row.BookAccountMethord + '</div>' +
                        '<div><em>币种:</em>' + row.Currency + '</div>' +
                        '<div><em>帐号:</em>' + row.Account + '</div>' +
                        '</div> ';
                    return s;
                },
                prompt: options.prompt,
                required: options.required,
                value: options.value,
                editable: false,
                onSelect: function (record) {
                    if (options.onSelect) {
                        options.onSelect(record);
                    }
                },
            });
        });
    }

    //插件的默认配置
    $.fn.clientPayerCrmPlus.defaults = {
        prompt: '请选择客户付款人',
        required: true,
        clientID: '',
        onSelect: function (record) {
        },
    };

    //插件对外的方法
    $.fn.clientPayerCrmPlus.methods = {
        //获取数据
        getData: function (jq) {
            var sender = $(jq);
            return sender.data('items');
        },
        //获取名称
        getName: function (jq) {
            var sender = $(jq);
            return sender.combobox('getText');
        },
        //获取值
        getValue: function (jq) {
            var sender = $(jq);
            return sender.combobox('getValue');
        },
        //设置值
        setValue: function (jq, value) {
            var sender = $(jq);
            sender.combobox('setValue', value);
            return sender;
        },
        //设置客户ID
        setClientID: function (jq, id) {
            var sender = $(jq);
            var myMethods = new innerMethods();
            myMethods.getClientPayers(sender, id);
        },
    };

    //将自定义的插件加入 easyui 的插件组
    $.parser.plugins.push('clientPayerCrmPlus');
})(jQuery)