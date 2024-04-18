/// <reference path="../../jquery-easyui-1.7.6/jquery.easyui.min.js" />
/// <reference path="../../jquery-easyui-1.7.6/jquery.min.js" />
/*
form值：
inputcompanyName name="{formName}companyName"
inputcompanyID name="{formName}companyID"
*/
if (v_RFQ_3_0) {
    var url = document.scripts[document.scripts.length - 1].src;
    var lower = url.toLowerCase();
    var prexUrl = url.substring(0, lower.indexOf('/yahv/') + '/yahv/'.length);
    writeScript('' + prexUrl + '/standard-easyui/v_RFQ_3_0/company.js');
}
else
(function ($) {
    $.extend($.fn.validatebox.defaults.rules, {
        company: {
            validator: function (value, param) {
                var sender = $('#' + param[0]);
                var data = sender.company('getData');

                if (data == null) {
                    return false;
                }

                var id = sender.data('inputs').inputcompanyID.val();
                var og = $.grep(data, function (item, index) {
                    return $.trim(item.name) == $.trim(value) && item.id == id;
                });

                return og.length > 0;
            },
            message: '必须选择已有内部公司!'
        }
    });

    //接口地址
    var AjaxUrl = {
        getCompanys: '/csrmapi/Companies/Search/'
    }

    //编写company插件
    $.fn.company = function (opt, param) {
        //如果options为string，则是方法调用
        if (typeof opt == 'string') {
            var method = $.fn.company.methods[opt];
            if (method) {
                return method(this, param);//如果有该方法就调用该方法
            } else {
                alert("该插件没有这个方法：" + opt)
            }
        }

        var options = opt || {};
        options = $.extend(true, {}, $.fn.company.defaults, options);

        return this.each(function () {
            var sender = $(this);
            //保存设置类型
            sender.data('options', options);

            var sender_id = sender.prop('id');
            if (!sender_id) {
                sender_id = 'company_sender_' + Math.random().toString().substring(2);
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
            var inputcompanyName = $('<input type="hidden" value="' + '' + '" name="' + formName + 'companyName">');
            var inputcompanyID = $('<input type="hidden" value="' + '' + '" name="' + formName + 'companyID">');
            sender.after(inputcompanyName).after(inputcompanyID);

            sender.data('inputs', {
                inputcompanyName: inputcompanyName,
                inputcompanyID: inputcompanyID
            });

            sender.combobox({
                loader: function (param, success, error) {
                    var q = $.trim(sender.combobox('getText'));
                    q = $.trim(q);
                    if (q.length <= 1) { return false; }
                    clearTimeout(setTimeoutAjax);
                    sender.combobox('isValid');



                    setTimeoutAjax = setTimeout(function () {
                        $.ajax({
                            url: AjaxUrl.getCompanys,
                            dataType: 'json',
                            type: 'GET',
                            data: { name: q },
                            success: function (json) {
                                var items = $.map(json.Data, function (item, index) {
                                    if (q == item.Name || $.trim(q) == $.trim(item.Name)) {
                                        inputcompanyName.val($.trim(item.Name));
                                        inputcompanyID.val($.trim(item.ID));
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


                                if (myOnChange) {
                                    myOnChange(q, '');
                                }
                            },
                            error: function (err) {
                                alert('company.error:' + JSON.stringify(err));
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
                    var oldValue = inputcompanyName.val();
                    inputcompanyName.val($.trim(record.name));
                    inputcompanyID.val($.trim(record.id));
                    if (myOnChange) {
                        myOnChange(record.name, oldValue);
                    }
                },
                validType: 'company["' + sender_id + '"]',
            });

            ////初始化调用的值
            if (myOnChange && value) {
                myOnChange(value, '');
            }

            //结束
        });
    }

    //内部公司插件的默认配置
    $.fn.company.defaults = {
        value: '',
        prompt: '请输入两位以上搜索内部公司',
        required: true,
        onChange: null //默认值也需要有意触发
    };

    //内部公司插件对外的方法
    $.fn.company.methods = {
        //获取名称
        getName: function (jq) {
            var sender = $(jq);
            return sender.data('inputs').inputcompanyName.val();
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
            var options = sender.data('options');

            if (options.onChange) {
                options.onChange(value, oldValue);
            }

            return sender;
        }
    };

    //将自定义的插件加入 easyui 的插件组
    $.parser.plugins.push('company');
})(jQuery)