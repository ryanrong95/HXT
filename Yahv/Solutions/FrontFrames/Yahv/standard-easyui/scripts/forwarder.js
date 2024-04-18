//货代供应商

(function ($) {
    $.extend($.fn.validatebox.defaults.rules, {
        forwarder: {
            validator: function (value, param) {
                var sender = $('#' + param[0]);
                var data = sender.forwarder('getData');
                if (data == null) {
                    return false;
                }
                var id = sender.data('inputs').inputForwarderID.val();
                var og = $.grep(data, function (item, index) {
                    return $.trim(item.Name) == $.trim(value);
                })
                return og.length > 0;
            },
            message: "必须选择已有货代供应商"
        }
    })

    //接口地址
    var AjaxUrl = {
        getForwarder: '/csrmapi/Suppliers/Forwarders'//通过供应商名称搜索货代供应商的数据
    }
    //编写forwarder插件
    $.fn.forwarder = function (opt, param) {
        //如果options为string，则是方法调用
        if (typeof (opt) == 'string') {
            var method = $.fn.forwarder.methods[opt];
            if (method) {
                return method(this, param);
            }
            else {
                alert("该插件没有这个方法：" + opt);
            }
        }

        var options = opt || {};
        options = $.extend(true, {}, $.fn.forwarder.defaults, options);

        return this.each(function () {
            var sender = $(this);
            var sender_id = sender.prop('id');
            if (!sender_id) {
                sender_id = 'forwarder_sender' + Math.random().toString().substring(2);
                sender.prop('id', sender_id);
            }
            var setTimeoutAjax = 0;
            //首次默认值查询
            var firstQuery = options.value;
            var formName = sender.prop('name') || sender.attr('textboxname');
            if (!formName) {
                alert('控件name不能为空！')
            }
            var value = sender.val();
            if (value) {
                firstQuery = options.value = value;
            }
            //设置自定义onChange事件
            var myOnChange = null;
            if (options.onChange) {
                myOnChange = options.onChange;
            }
            //创建form返回值
            var inputForwarderName = $('<input type="hidden" value="' + '' + '" name="' + formName + 'FactoryName">');
            var inputForwarderID = $('<input type="hidden" value="' + '' + '" name="' + formName + 'FactoryID">');
            sender.after(inputForwarderName).after(inputForwarderID);
            sender.data('inputs', {
                inputForwarderName: inputForwarderName,
                inputForwarderID: inputForwarderID
            })
            sender.combobox({
                loader: function (param, success, error) {
                    var q = $.trim(sender.combobox('getText'));
                    q = $.trim(q);
                    if (q.length <= 0) {
                        return false;
                    }
                    clearTimeout(setTimeoutAjax);
                    setTimeoutAjax = setTimeout(function () {
                        $.ajax({
                            url: AjaxUrl.getForwarder,
                            dataType: 'jsonp',
                            type: 'GET',
                            data: { key: q },
                            success: function (json) {
                                var items = $.map(json.Data, function (item, index) {
                                    if (q == item.Name || $.trim(q) == $.trim(item.Name)) {
                                        inputForwarderName.val($.trim(item.Name));
                                        inputForwarderID.val($.trim(item.ID));
                                    }
                                    return {
                                        ID: item.ID,
                                        Name: item.Name,
                                        Grade: item.Grade,
                                    };
                                });

                                sender.data('items', items);
                                success(items);
                                sender.combobox('isValid');
                            },
                            error: function (err) {
                                alert('factory.error:' + JSON.stringify(err));
                            }
                        });
                    }, 100);
                },
                //mode: 'remote',
                valueField: "ID",
                textField: "Name",
                formatter: function (row) {
                    var s = '<div><span class="level' + row.Grade + '"></span>' + row.Name + '</div>'
                    return s;
                },
                prompt: options.prompt,
                required: options.required,
                value: options.value,
                onSelect: function (record) {
                    var OldValue = inputForwarderName.val();
                    inputForwarderName.val($.trim(record.Name));
                    inputForwarderID.val($.trim(record.ID));
                    if (myOnChange) {
                        myOnChange(record.ID, OldValue);
                    }
                },
                validType: 'forwarder["' + sender_id + '"]'
            })
            ////初始化调用的值
            if (myOnChange && value) {
                myOnChange(value, '');
            }
        });
    }
    //默认配置
    $.fn.forwarder.defaults = {
        value: '',
        prompt: '请输入两位以上搜索货代供应商',
        required: true,
        onChange: null //默认值也需要有意触发
    };
    //插件对外的方法
    $.fn.forwarder.methods = {
        //获取名称
        getName: function (jq) {
            var sender = $(jq);
            return sender.data('inputs').inputForwarderName.val();
        },
        //获取数据
        getData: function (jq) {
            var sender = $(jq);
            return sender.data('items');
        },
        //获取数据
        setValue: function (jq, value) {
            var sender = $(jq);
            sender.combobox('setValue', value);
            sender.combobox('reload');
            return sender;
        }
    };
    //将自定义的插件加入 easyui 的插件组
    $.parser.plugins.push('forwarder');
})(jQuery)