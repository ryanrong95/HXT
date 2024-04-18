/// <reference path="../../jquery-easyui-1.7.6/jquery.easyui.min.js" />
/// <reference path="../../jquery-easyui-1.7.6/jquery.min.js" />


//企业收件地址
(function ($) {
    //接口地址
    var AjaxUrl = {
        getConsignee: '/crmplusapi/Enterprises/Consignees'
    }

    //编写consigneeCrmPlus插件
    $.fn.consigneeCrmPlus = function (opt, param) {
        //如果options为string，则是方法调用
        if (typeof opt == 'string') {
            var method = $.fn.consigneeCrmPlus.methods[opt];
            if (method) {
                return method(this, param);//如果有该方法就调用该方法
            } else {
                alert("consigneeCrmPlus.该插件没有这个方法：" + opt)
            }
        }

        var options = opt || {};
        options = $.extend(true, {}, $.fn.consigneeCrmPlus.defaults, options);

        return this.each(function () {
            var sender = $(this);


            //保存设置类型
            sender.data('options', options);

            var sender_id = sender.prop('id');
            if (!sender_id) {
                sender_id = 'Consignee_sender_' + Math.random().toString().substring(2);
                sender.prop('id', sender_id);
            }
            var formName = sender.prop('name');
            if (!formName) {
                alert('consigneeCrmPlus.控件name不能为空！2');
            }

            var initdata = [];

            sender.combobox({
                valueField: 'ID',
                textField: 'Address',
                formatter: function (row) {
                    var s = '<div>' +
                            '<div><em>地址:</em>' + row.Address + '</div>' +
                            '<div><em>联系人:</em>' + row.Contact + '</div>' +
                            '<div><em>手机:</em>' + row.Mobile + '</div>' +
                            '<div><em>电话:</em>' + row.Tel + '</div>' +
                            '</div> ';
                    return s;
                },
                prompt: options.prompt,
                required: options.required,
                value: options.value,
                onSelect: function (record) {
                    
                },
                data: initdata,
                editable: false,
                validateOnCreate: true
            });

            //结束
        });
    }

    //企业插件的默认配置
    $.fn.consigneeCrmPlus.defaults = {
        //value: '', //企业ID
        prompt: '收件地址',
        required: true,
    };

    //企业插件对外的方法
    $.fn.consigneeCrmPlus.methods = {
        //获取数据
        getData: function (jq) {
            var sender = $(jq);
            return sender.data('items');
        },

        //设置企业ID
        setEnterpriseID: function (jq, id) {
            var sender = $(jq);

            sender.combobox('disable');
            sender.combobox('clear');
            sender.combobox('loadData', []);

            $.ajax({
                url: AjaxUrl.getConsignee,
                dataType: 'json',
                type: 'GET',
                data: { enterpriseID: id },
                success: function (json) {
                    var data = json;
                    sender.combobox('loadData', data);
                    sender.data('items', data);
                    sender.combobox('enable');
                    if (data.length > 0) {
                        sender.combobox('setValue', data[0].ID)
                    }
                },
                error: function (err) {
                    alert('consigneeCrmPlus.error:' + JSON.stringify(err));
                }
            });

            return sender;
        },
        //param:{ enterpriseID: '企业ID', consigneeID: '地址ID' }
        //加载企业数据再选中某个选项
        loadSetValue: function (jq, param) {
            var sender = $(jq);

            sender.combobox('disable');
            sender.combobox('clear');
            sender.combobox('loadData', []);

            $.ajax({
                url: AjaxUrl.getConsignee,
                dataType: 'json',
                type: 'GET',
                data: { enterpriseID: param.enterpriseID },
                success: function (json) {
                    var data = json;
                    sender.combobox('loadData', data);
                    sender.data('items', data);
                    sender.combobox('enable');
                    var selector = null;
                    $.map(data, function (item, index) {
                        if (param.consigneeID == item.ID) {
                            selector = item;
                        }
                    });
                    selector == null ? alert('地址不存在') : sender.combobox('setValue', param.consigneeID)
                },
                error: function (err) {
                    alert('consigneeCrmPlus.error:' + JSON.stringify(err));
                }
            });

            return sender;
        },
        //设置值
        setValue: function (jq, value) {
            var sender = $(jq);
            sender.combobox('setValue', value);
            return sender;
        },

    };

    //将自定义的插件加入 easyui 的插件组
    $.parser.plugins.push('consigneeCrmPlus');
})(jQuery)