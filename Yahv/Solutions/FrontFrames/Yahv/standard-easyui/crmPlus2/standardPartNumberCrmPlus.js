/// <reference path="../../jquery-easyui-1.7.6/jquery.easyui.min.js" />
/// <reference path="../../jquery-easyui-1.7.6/jquery.min.js" />

//标准型号名称补齐插件（可以输入可以选择）
//name              表示调用控件的dom的name属性
//有2个<input type="hidden">的input，以下它们的name值，这些值携带了响应的数据
//name              表示用户输入的标准型号名称补齐名称
//name_result       表示某个标准型号名称补齐的数据

(function ($) {
    $.extend($.fn.validatebox.defaults.rules, {
        standardPartNumberCrmPlus: {
            validator: function (value, param) {
                var sender = $('#' + param[0]);
                var data = sender.combobox('getData');
                if (data == null) {
                    return false;
                }
                var val = $.trim(sender.combobox('getValue'));

                var og = $.grep(data, function (item, index) {
                    return val == $.trim(item.ID);
                });

                return og.length > 0;
            },
            message: '必须选择已有项!'
        }
    });
    //接口地址
    var AjaxUrl = {
        searhStandardBrand: '/crmplus2api/Standard/PartNumbers',
    }

    //编写standardPartNumberCrmPlus插件
    $.fn.standardPartNumberCrmPlus = function (opt, param) {
        //如果options为string，则是方法调用
        if (typeof opt == 'string') {
            var method = $.fn.standardPartNumberCrmPlus.methods[opt];
            if (method) {
                return method(this, param);//如果有该方法就调用该方法
            } else {
                alert("standardPartNumberCrmPlus.该插件没有这个方法")
            }
        }

        return this.each(function () {

            var sender = $(this);

            var options = opt || {};
            var options = $.extend(true, {
            }, $.fn.standardPartNumberCrmPlus.defaults, options);
            var value = sender.val();

            if (value) {
                options.value = value;
            }

            //保存设置类型
            sender.data('options', options);

            var sender_id = sender.prop('id');
            if (!sender_id) {
                sender_id = 'brand_sender_' + Math.random().toString().substring(2);
                sender.prop('id', sender_id);
            }
            var setTimeoutAjax = 0;

            sender.combobox({
                panelWidth: sender.width() + 1 * 20,
                loader: function (param, success, error) {
                    var q = $.trim(sender.combobox('getText'));
                    q = $.trim(q);
                    if (q.length <= 1) {
                        return false;
                    }

                    clearTimeout(setTimeoutAjax);
                    setTimeoutAjax = setTimeout(function () {
                        $.ajax({
                            url: AjaxUrl.searhStandardBrand,
                            dataType: 'json',
                            type: 'GET',
                            data: {
                                key: q
                            },
                            success: function (json) {
                                var data = json.data || [];
                                var selector = null;
                                var items = $.map(data, function (item, index) {
                                    if (q == item.Name || $.trim(q) == $.trim(item.Name)) {
                                        selector = item;
                                    }
                                    return item;
                                });

                                sender.data('items', items);
                                success(items);
                                if (data.length > 0) {
                                    sender.combobox('setValue', data[0].ID);
                                }
                                sender.combobox('isValid');

                            },
                            error: function (err) {
                                alert('standardPartNumberCrmPlus.error:' + JSON.stringify(err));
                            }
                        });
                    }, 100);
                },
                mode: 'remote',
                valueField: 'ID',
                textField: 'PartNumber',
                prompt: options.prompt,
                required: options.required,
                value: options.value,
                formatter: function (row) {
                    var s = '<div>' +
                            '<div><em>型号:</em>' + row.PartNumber + '</div>' +
                            '<div><em>品牌:</em>' + row.Brand + '</div>' +
                            '</div> ';
                    return s;
                },
                onSelect: function (record) {
                    if (options.onSelect) {
                        options.onSelect(record);
                    }
                },
                onChange: function (newValue) {

                },
                validType: 'standardPartNumberCrmPlus["' + sender_id + '"]',
                data: [],
                icons: []
            });
        });
    };

    $.fn.standardPartNumberCrmPlus.defaults = {
        value: '',
        prompt: '请输入两位以上搜索型号',
        eccnSelector: '',//设置Eccn 展示的区域
        required: true,
        onSelect: function (record) {
        },
    };

    //标准型号名称补齐插件对外的方法
    $.fn.standardPartNumberCrmPlus.methods = {
        //保留测试
        test: function (jq) {
            return $(jq);
        },
        //设置值
        setValue: function (jq, value) {
            var sender = $(jq);
            sender.combobox('setValue', value);
            sender.combobox('reload');
            return sender;
        },
    };
    //将自定义的插件加入 easyui 的插件组
    $.parser.plugins.push('standardPartNumberCrmPlus');
})(jQuery)