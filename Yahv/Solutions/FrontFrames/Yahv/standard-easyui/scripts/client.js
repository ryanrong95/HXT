/// <reference path="../../jquery-easyui-1.7.6/jquery.easyui.min.js" />
/// <reference path="../../jquery-easyui-1.7.6/jquery.min.js" />

/*
form值：
inputClientName name="{formName}ClientName"
inputClientID name="{formName}ClientID"
*/


if (v_RFQ_3_0) {
    var url = document.scripts[document.scripts.length - 1].src;
    var lower = url.toLowerCase();
    var prexUrl = url.substring(0, lower.indexOf('/yahv/') + '/yahv/'.length);
    writeScript('' + prexUrl + '/standard-easyui/v_RFQ_3_0/client.js');
}
else
(function ($) {

    $.extend($.fn.validatebox.defaults.rules, {
        client: {
            validator: function (value, param) {
                var sender = $('#' + param[0]);
                var data = sender.client('getData');
                if (data == null) {
                    return false;
                }

                var id = sender.data('inputs').inputClientID.val();
                var og = $.grep(data, function (item, index) {
                    return $.trim(item.name) == $.trim(value);//|| item.id == id;
                });

                return og.length > 0;
            },
            message: '必须选择已有客户!'
        }
    });

    //接口地址
    var AjaxUrl = {
        getclients: '/csrmapi/clients/Search'//通过客户名称搜索客户的数据
    }

    //编写client插件
    $.fn.client = function (opt, param) {
        //如果options为string，则是方法调用
        if (typeof opt == 'string') {
            var method = $.fn.client.methods[opt];
            if (method) {
                return method(this, param);//如果有该方法就调用该方法
            } else {
                alert("client.该插件没有这个方法：" + opt)
            }
        }

        var options = opt || {};
        options = $.extend(true, {}, $.fn.client.defaults, options);

        return this.each(function () {
            var sender = $(this);
            //保存设置类型
            sender.data('options', options);

            var sender_id = sender.prop('id');
            if (!sender_id) {
                sender_id = 'client_sender_' + Math.random().toString().substring(2);
                sender.prop('id', sender_id);
            }

            var setTimeoutAjax = 0;
            //首次默认值查询
            var firstQuery = options.value;

            var formName = sender.prop('name');
            if (!formName) {
                alert('client.控件name不能为空！');
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
            var inputClientName = $('<input type="hidden" value="' + '' + '" name="' + formName + 'ClientName">');
            var inputClientID = $('<input type="hidden" value="' + '' + '" name="' + formName + 'ClientID">');
            sender.after(inputClientName).after(inputClientID);

            sender.data('inputs', {
                inputClientName: inputClientName,
                inputClientID: inputClientID
            });

            sender.combobox({
                loader: function (param, success, error) {

                    var q = $.trim(sender.combobox('getText'));
                    q = $.trim(q);
                    if (q.length <= 1) { return false; }
                    clearTimeout(setTimeoutAjax);
                    setTimeoutAjax = setTimeout(function () {
                        $.ajax({
                            url: AjaxUrl.getclients,
                            dataType: 'json',
                            type: 'GET',
                            data: { name: q },
                            success: function (json) {

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
                                    if ($.trim(o.name) == q || o.name == q) {
                                        selected = o;
                                    }
                                    return o;
                                });
                                sender.data('items', items);
                                success(items);
                                sender.combobox('isValid');

                                if (selected) {
                                    inputClientName.val($.trim(selected.name));
                                    inputClientID.val($.trim(selected.id));
                                    if (myOnChange) {
                                        myOnChange(selected.name, '');
                                    }
                                }


                            },
                            error: function (err) {
                                alert('client.error:' + JSON.stringify(err));
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
                    var oldValue = inputClientName.val();
                    inputClientName.val($.trim(record.name));
                    inputClientID.val($.trim(record.id));

                    if (myOnChange) {
                        myOnChange(record.name, oldValue);
                    }
                },
                validType: 'client["' + sender_id + '"]'
            });

            //结束
        });
    }

    //客户插件的默认配置
    $.fn.client.defaults = {
        value: '',
        prompt: '请输入两位以上搜索客户',
        required: true,
        onChange: null //默认值也需要有意触发
    };

    //客户插件对外的方法
    $.fn.client.methods = {
        //获取名称
        getName: function (jq) {
            var sender = $(jq);
            return sender.data('inputs').inputClientName.val();
        },
        //获取数据
        getData: function (jq) {
            var sender = $(jq);
            return sender.data('items');
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
        //获取数据
        setValue: function (jq, value) {
            var sender = $(jq);
            sender.combobox('setValue', value);
            sender.combobox('reload');
            return sender;
        }
    };

    //将自定义的插件加入 easyui 的插件组
    $.parser.plugins.push('client');
})(jQuery)