/// <reference path="../../jquery-easyui-1.7.6/jquery.easyui.min.js" />
/// <reference path="../../jquery-easyui-1.7.6/jquery.min.js" />

(function ($) {
    //扩展验证规则
    $.extend($.fn.validatebox.defaults.rules, {
        supplier: {
            validator: function (value, param) {
                var sender = $('#' + param[0]);
                var data = sender.supplierRegisterCrmPlus('getData');
                if (data == null) {
                    return false;
                }

                var og = $.grep(data, function (item, index) {
                    return $.trim(item.Name) == $.trim(value);
                });

                return og.length > 0;
            },
            message: '必须选择已有供应商!'
        }
    });

    //接口地址
    var ajaxUrl = {
        getSuppliers: '/crmplusapi/Enterprises/Suppliers',
        getNewRegistered: '/crmplusapi/Suppliers/NewRegistered'
    }

    //供应商快速注册页面地址
    var pageUrl = {
        supplierQuickRegister: '/crmplus/Srm/Drafts/QuickRegister.aspx'
    }

    //仅供内部调用的方法
    var innerMethods = function () { };
    innerMethods.prototype = {

        //添加快速注册按钮
        addRegisterBtn: function (sender, cb_close) {
            var options = sender.data('options');
            var registerBtn = $('<a>');
            registerBtn.linkbutton({
                iconCls: 'icon-yg-add',
                text: '快速注册',
            }).click(function () {
                $.myDialogFuse({
                    title: '供应商快速注册',
                    url: pageUrl.supplierQuickRegister + '?f_source=' + options.f_source,
                    width: '60%',
                    height: '500px',
                    onClose: function () {
                        cb_close(sender);
                    }
                });
            });
            sender.after(registerBtn);
        },

        //ajax请求供应商数据
        getSuppliers: function (sender, value, success) {
            $.ajax({
                url: ajaxUrl.getSuppliers,
                dataType: 'json',
                type: 'GET',
                data: { key: value },
                success: function (json) {
                    var selected = null;
                    var items = $.map(json, function (item, index) {
                        if ($.trim(item.Name) == value || item.Name == value) {
                            selected = item;
                        }
                        return item;
                    });
                    sender.data('items', items);
                    //sender.combobox('loadData', items);
                    success(items);
                    sender.combobox('isValid');

                    if (selected) {
                        sender.combobox('setValue', selected.ID);
                    }
                },
                error: function (err) {
                    alert('supplierRegister.error:' + JSON.stringify(err));
                }
            });
        },

        //获取新注册的供应商
        getNewRegistered: function (sender) {
            $.ajax({
                url: ajaxUrl.getNewRegistered,
                dataType: 'json',
                type: 'GET',
                success: function (json) {
                    var items = [];
                    items.push(json.data);
                    sender.data('items', items);
                    sender.combobox('loadData', items);
                    sender.combobox('isValid');
                    sender.combobox('setValue', items[0].ID);
                },
                error: function (err) {
                    alert('supplierRegister.error:' + JSON.stringify(err));
                }
            });
        },
    }

    //编写supplierRegister插件
    $.fn.supplierRegisterCrmPlus = function (opt, param) {
        //如果options为string，则是方法调用
        if (typeof opt == 'string') {
            var method = $.fn.supplierRegisterCrmPlus.methods[opt];
            if (method) {
                return method(this, param);//如果有该方法就调用该方法
            } else {
                alert("supplierRegister.插件没有这个方法: " + opt)
            }
        }

        var options = opt || {};
        options = $.extend(true, {}, $.fn.supplierRegisterCrmPlus.defaults, options);

        return this.each(function () {
            var sender = $(this);
            //保存设置类型
            sender.data('options', options);

            var myMethods = new innerMethods();

            //获取控件id
            var sender_id = sender.prop('id');
            if (!sender_id) {
                sender_id = 'supplierRegister_sender_' + Math.random().toString().substring(2);
                sender.prop('id', sender_id);
            }

            //控件属性验证
            var formName = sender.prop('name');
            if (!formName) {
                alert('supplierRegister.控件name不能为空!');
            }

            //添加‘快速注册’按钮
            if (options.isRegister) {
                myMethods.addRegisterBtn(sender, myMethods.getNewRegistered);
            }

            var setTimeoutAjax = 0;

            sender.combobox({
                loader: function (param, success, error) {
                    //输入两位以上再进行搜索
                    var q = $.trim(sender.combobox('getText'));
                    if (q.length < 2) {
                        return false;
                    }

                    //查询供应商数据
                    clearTimeout(setTimeoutAjax);
                    setTimeoutAjax = setTimeout(myMethods.getSuppliers(sender, q, success), 100);
                },
                mode: 'remote',
                valueField: 'ID',
                textField: 'Name',
                formatter: function (row) {
                    var s = '<div><span class="level' + row.Grade + '"></span>' + row.Name + '</div>'
                    return s;
                },
                prompt: options.prompt,
                required: options.required,
                value: options.value,
                onChange: function (newValue, oldValue) {
                    if (options.onChange) {
                        options.onChange(newValue, oldValue);
                    }
                },
                onSelect: function (record) {
                    if (options.onSelect) {
                        options.onSelect(record);
                    }
                },
                validType: 'supplier["' + sender_id + '"]'
            });
        });
    }

    //插件的默认配置
    $.fn.supplierRegisterCrmPlus.defaults = {
        prompt: '请输入两位以上搜索供应商',
        required: true,
        isRegister: false, //是否需要快速注册
        f_source: 'Suppliers',
        onChange: function (newValue, oldValue) {

        },
        onSelect: function (record) {
        },
    };

    //插件对外的方法
    $.fn.supplierRegisterCrmPlus.methods = {
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
    };

    //将自定义的插件加入 easyui 的插件组
    $.parser.plugins.push('supplierRegisterCrmPlus');
})(jQuery);