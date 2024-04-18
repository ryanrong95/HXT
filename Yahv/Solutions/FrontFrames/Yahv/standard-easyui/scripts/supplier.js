/// <reference path="../../jquery-easyui-1.7.6/jquery.easyui.min.js" />
/// <reference path="../../jquery-easyui-1.7.6/jquery.min.js" />
/*
form值：
name="{formName}"
*/

if (v_RFQ_3_0) {
    var url = document.scripts[document.scripts.length - 1].src;
    var lower = url.toLowerCase();
    var prexUrl = url.substring(0, lower.indexOf('/yahv/') + '/yahv/'.length);
    writeScript('' + prexUrl + '/standard-easyui/v_RFQ_3_0/supplier.js');
}
else
(function ($) {

    $.extend($.fn.validatebox.defaults.rules, {
        supplier: {
            validator: function (value, param) {
                var sender = $('#' + param[0]);
                var data = sender.supplier('getData');

                if (data == null) {
                    return false;
                }

                var id = sender.data('inputs').inputSupplierID.val();
                var og = $.grep(data, function (item, index) {
                    return $.trim(item.name) == $.trim(value);//|| item.id == id;
                });

                return og.length > 0;
            },
            message: '必须选择已有供应商!'
        }
    });

    //接口地址
    var AjaxUrl = {
        getsupplierData: '/csrmapi/Suppliers',//获取供应商数据(前20条)
        getSuppliers: '/csrmapi/Suppliers/Search'//通过供应商名称搜索供应商的数据
    }

    //编写supplier插件
    $.fn.supplier = function (opt, param) {
        //如果options为string，则是方法调用
        if (typeof opt == 'string') {
            var method = $.fn.supplier.methods[opt];
            if (method) {
                return method(this, param);//如果有该方法就调用该方法
            } else {
                alert("该插件没有这个方法：" + opt)
            }
        }

        var options = opt || {};
        options = $.extend(true, {}, $.fn.supplier.defaults, options);

        return this.each(function () {
            var sender = $(this);
            //保存设置类型
            sender.data('options', options);

            var sender_id = sender.prop('id');
            if (!sender_id) {
                sender_id = 'supplier_sender_' + Math.random().toString().substring(2);
                sender.prop('id', sender_id);
            }

            var setTimeoutAjax = 0;
            //首次默认值查询
            var firstQuery = options.value;

            var formName = sender.prop('name') || sender.attr('textboxname');
            if (!formName) {
                alert('控件name不能为空！');
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
            var inputSupplierName = $('<input type="hidden" value="' + '' + '" name="' + formName + 'SupplierName">');
            var inputSupplierID = $('<input type="hidden" value="' + '' + '" name="' + formName + 'SupplierID">');
            sender.after(inputSupplierName).after(inputSupplierID);

            sender.data('inputs', {
                inputSupplierName: inputSupplierName,
                inputSupplierID: inputSupplierID
            });

            sender.combobox({
                loader: function (param, success, error) {
                    var q = $.trim(sender.combobox('getText'));
                    q = $.trim(q);
                    if (q.length <= 1) { return false; }
                    clearTimeout(setTimeoutAjax);

                    setTimeoutAjax = setTimeout(function () {
                        $.ajax({
                            url: AjaxUrl.getSuppliers,
                            dataType: 'json',
                            type: 'GET',
                            data: { name: q },
                            success: function (json) {
                                var items = $.map(json.Data, function (item, index) {
                                    if (q == item.Name || $.trim(q) == $.trim(item.Name)) {
                                        inputSupplierName.val($.trim(item.Name));
                                        inputSupplierID.val($.trim(item.ID));
                                    }
                                    return {
                                        id: item.ID,
                                        name: item.Name,
                                        grade: item.Grade,
                                    };
                                });

                                sender.data('items', items);
                                success(items);
                                sender.combobox('isValid');
                            },
                            error: function (err) {
                                alert('supplier.error:' + JSON.stringify(err));
                            }
                        });
                    }, 100);
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
                onSelect: function (record) {
                    var oldValue = inputSupplierName.val();
                    inputSupplierName.val($.trim(record.name));
                    inputSupplierID.val($.trim(record.id));
                    if (myOnChange) {
                        myOnChange(record.name, oldValue);
                    }
                },
                validType: 'supplier["' + sender_id + '"]'
            });

            ////初始化调用的值
            if (myOnChange && value) {
                myOnChange(value, '');
            }

            //结束
        });
    }

    //供应商插件的默认配置
    $.fn.supplier.defaults = {
        value: '',
        prompt: '请输入两位以上搜索供应商',
        required: true,
        onChange: null //默认值也需要有意触发
    };

    //供应商插件对外的方法
    $.fn.supplier.methods = {
        //获取名称
        getName: function (jq) {
            var sender = $(jq);
            return sender.data('inputs').inputSupplierName.val();
        },
        //获取数据
        getData: function (jq) {
            var sender = $(jq);
            return sender.data('items');
        },
        //获取数据
        setValue: function (jq, value) {
            var sender = $(jq);

            var oldValue = sender.combobox('getValue');

            sender.combobox('setValue', value);
            sender.combobox('reload');
            sender.combobox('reload');

            var options = sender.data('options');

            if (options.onChange) {
                options.onChange(value, oldValue);
            }

            return sender;
        }
    };

    //将自定义的插件加入 easyui 的插件组
    $.parser.plugins.push('supplier');
})(jQuery)