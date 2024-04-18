/// <reference path="../../jquery-easyui-1.7.6/jquery.easyui.min.js" />
/// <reference path="../../jquery-easyui-1.7.6/jquery.min.js" />

(function ($) {
    //扩展验证规则
    $.extend($.fn.validatebox.defaults.rules, {
        client: {
            validator: function (value, param) {
                var sender = $('#' + param[0]);
                var data = sender.client('getData');
                if (data == null) {
                    return false;
                }

                var og = $.grep(data, function (item, index) {
                    return $.trim(item.name) == $.trim(value);
                });

                return og.length > 0;
            },
            message: '必须选择已有客户!'
        }
    });

    //接口地址
    var ajaxUrl = {
        getClients: '/crmplusapi/RFQV3/MyClients',
        getNewRegistered: '/crmplusapi/RFQV3/NewRegistered',
        syncClients: '/crmplusapi/RFQV3/SyncMyClients',
    }

    //客户快速注册页面地址
    var pageUrl = {
        clientQuickRegister: '/crmplus/Crm/Client/QuickRegister.aspx'
    }

    //仅供内部调用的方法
    var innerMethods = function () { };
    innerMethods.prototype = {

        //添加快速注册按钮
        addRegisterBtn: function (sender, cb_close) {
            var options = sender.data('options');
            var registerBtn = $('<a>');
            registerBtn.linkbutton({
                //iconCls: 'icon-yg-search',
                text: '大赢家同步',
            }).click(function () {
                //$.myWindow({
                //    title: '客户快速注册',
                //    url: pageUrl.clientQuickRegister + '?f_source=' + options.f_source,
                //    width: '60%',
                //    height: '500px',
                //    onClose: function () {
                //        //cb_close(sender);
                //    }
                //});
                var q = $.trim(sender.combobox('getText'));
                if (q.length < 3) {
                    $.messager.alert('操作提示', '请输入三位以上再进行同步', 'error');
                    //alert("请输入三位以上再进行同步")
                    return false;
                }
                $.ajax({
                    url: ajaxUrl.syncClients,
                    dataType: 'json',
                    type: 'GET',
                    data: { name: q },
                    beforeSend: function () {
                        ajaxLoading();
                    },
                    success: function (json) {
                        ajaxLoadEnd();
                        if (json.Code != "100") {
                            $.messager.alert('操作提示', "数据不存在，请联系伊广杭经理！");
                        } else {
                            $.messager.alert('操作提示', '同步完成');
                        }
                        sender.combobox('clear');
                    },
                    error: function (err) {
                        ajaxLoadEnd();
                        alert('clientRegister.error:' + JSON.stringify(err));
                    }
                });
            });
            sender.after(registerBtn);
        },

        //为兼容之前的插件逻辑，添加隐藏输入框，用于保存客户信息
        addClientInput: function (sender, formName) {
            var inputClientName = $('<input type="hidden" value="' + '' + '" name="' + formName + 'ClientName">');
            var inputClientID = $('<input type="hidden" value="' + '' + '" name="' + formName + 'ClientID">');
            sender.after(inputClientName).after(inputClientID);

            sender.data('inputs', {
                inputClientName: inputClientName,
                inputClientID: inputClientID
            });
        },

        //ajax请求客户数据
        getClients: function (sender, value, success) {
            $.ajax({
                url: ajaxUrl.getClients,
                dataType: 'json',
                type: 'GET',
                data: { name: value },
                beforeSend: function () {
                    ajaxLoading();
                },
                success: function (json) {
                    ajaxLoadEnd();
                    var selected = null;
                    var items = $.map(json.Data, function (item, index) {
                        var o = {
                            id: item.ID,
                            name: item.Name,
                            grade: item.Grade,
                            AreaType: item.AreaType,
                            AreaTypeDes: item.AreaTypeDes,
                            Nature: item.Nature,
                            NatureDes: item.NatureDes
                        };
                        if ($.trim(o.name) == value || o.name == value) {
                            selected = o;
                        }
                        return o;
                    });
                    sender.data('items', items);
                    //sender.combobox('loadData', items);
                    success(items);
                    sender.combobox('isValid');

                    if (selected) {
                        sender.combobox('setValue', selected.id);
                    }
                },
                error: function (err) {
                    ajaxLoadEnd();
                    alert('clientRegister.error:' + JSON.stringify(err));
                }
            });
        },
        //获取新注册的客户
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
                    alert('clientRegister.error:' + JSON.stringify(err));
                }
            });
        },
    }

    //编写clientRegister插件
    $.fn.client = function (opt, param) {
        var setTimeoutAjax = 0;
        //如果options为string，则是方法调用
        if (typeof opt == 'string') {
            var method = $.fn.client.methods[opt];
            if (method) {
                return method(this, param);//如果有该方法就调用该方法
            } else {
                alert("clientRegister.插件没有这个方法: " + opt)
            }
        }

        var options = opt || {};
        options = $.extend(true, {}, $.fn.client.defaults, options);

        return this.each(function () {
            var sender = $(this);
            //保存设置类型
            sender.data('options', options);

            var myMethods = new innerMethods();

            //获取控件id
            var sender_id = sender.prop('id');
            if (!sender_id) {
                sender_id = 'clientRegister_sender_' + Math.random().toString().substring(2);
                sender.prop('id', sender_id);
            }

            //控件属性验证
            var formName = sender.prop('name');
            if (!formName) {
                alert('clientRegister.控件name不能为空!');
            }

            var value = sender.val();
            if (value) {
                options.value = value;
            }

            //添加‘快速注册’按钮
            if (options.isRegister) {
                myMethods.addRegisterBtn(sender, myMethods.getNewRegistered);
            }

            //为兼容之前的插件逻辑，添加隐藏输入框，用于保存客户信息
            myMethods.addClientInput(sender, formName);

            sender.combobox({
                onBeforeLoad: function () {
                    //输入三位以上再进行搜索
                    var q = $.trim(sender.combobox('getText'));
                    if (q.length < 3) {
                        return false;
                    }
                },
                loader: function (param, success, error) {
                    var q = $.trim(sender.combobox('getText'));
                    //查询客户数据
                    clearTimeout(setTimeoutAjax);
                    setTimeoutAjax = setTimeout(function () {
                        myMethods.getClients(sender, q, success);
                    }, 500);
                },
                mode: 'remote',
                valueField: 'id',
                textField: 'name',
                formatter: function (row) {
                    var s = '<div><span class="level' + row.grade + '"></span>' + row.name + '</div>'
                    return s;
                },
                prompt: options.prompt,
                required: options.required,
                value: options.value,
                //onChange: function (newValue, oldValue) {
                //    if (options.onChange) {
                //        options.onChange(newValue, oldValue);
                //    }
                //},
                //其实是错误的写法，但插件已经在询报价大量使用了，只能去兼容老插件的逻辑
                onSelect: function (record) {
                    //if (options.onSelect) {
                    //    options.onSelect(record);
                    //}

                    var inputs = sender.data('inputs');
                    var oldValue = inputs.inputClientName.val();
                    inputs.inputClientName.val($.trim(record.name));
                    inputs.inputClientID.val($.trim(record.id));

                    if (options.onChange) {
                        options.onChange(record.name, oldValue);
                    }
                },
                validType: 'client["' + sender_id + '"]'
            });
        });
    }

    //插件的默认配置
    $.fn.client.defaults = {
        value: '',
        prompt: '请输入三位以上搜索客户',
        required: true,
        isRegister: true, //是否需要快速注册
        f_source: 'Tradings',
        onChange: function (newValue, oldValue) {

        },
        //onSelect: function (record) {
        //},
    };

    //插件对外的方法
    $.fn.client.methods = {
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
            var data = sender.data('items');
            if (data == null) {
                return false;
            }

            var id = sender.data('inputs').inputClientID.val();
            var og = $.grep(data, function (item, index) {
                return item.id == id;
            });
            return og[0];
        },
        //设置值
        setValue: function (jq, value) {
            var sender = $(jq);
            sender.combobox('setValue', value);
            return sender;
        },
    };

    //将自定义的插件加入 easyui 的插件组
    $.parser.plugins.push('client');
})(jQuery);