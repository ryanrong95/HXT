/// <reference path="../../jquery-easyui-1.7.6/jquery.easyui.min.js" />
/// <reference path="../../jquery-easyui-1.7.6/jquery.min.js" />
/*
form值：
name="{formName}"
*/
(function ($) {

    $.extend($.fn.validatebox.defaults.rules, {
        factory: {
            validator: function (value, param) {
                var sender = $('#' + param[0]);
                var data = sender.factory('getData');

                if (data == null) {
                    return false;
                }

                var id = sender.data('inputs').inputFactoryID.val();
                var og = $.grep(data, function (item, index) {
                    return $.trim(item.Name) == $.trim(value);//|| item.id == id;
                });

                return og.length > 0;
            },
            message: '必须选择已有原厂供应商!'
        }
    });

    //接口地址
    var AjaxUrl = {
        getFactories: '/csrmapi/Suppliers/Factories'//通过供应商名称搜索供应商的数据
    }

    //编写factory插件
    $.fn.factory = function (opt, param) {
        //如果options为string，则是方法调用
        if (typeof opt == 'string') {
            var method = $.fn.factory.methods[opt];
            if (method) {
                return method(this, param);//如果有该方法就调用该方法
            } else {
                alert("该插件没有这个方法：" + opt)
            }
        }

        var options = opt || {};
        options = $.extend(true, {}, $.fn.factory.defaults, options);

        return this.each(function () {
            var sender = $(this);
            //保存设置类型
            sender.data('options', options);

            var sender_id = sender.prop('id');
            if (!sender_id) {
                sender_id = 'factory_sender_' + Math.random().toString().substring(2);
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
            var inputFactoryName = $('<input type="hidden" value="' + '' + '" name="' + formName + 'FactoryName">');
            var inputFactoryID = $('<input type="hidden" value="' + '' + '" name="' + formName + 'FactoryID">');
            sender.after(inputFactoryName).after(inputFactoryID);

            sender.data('inputs', {
                inputFactoryName: inputFactoryName,
                inputFactoryID: inputFactoryID
            });

            sender.combobox({
                loader: function (param, success, error) {
                    var q = $.trim(sender.combobox('getText'));
                    q = $.trim(q);
                    if (q.length <= 1) { return false; }
                    clearTimeout(setTimeoutAjax);

                    setTimeoutAjax = setTimeout(function () {
                        $.ajax({
                            url: AjaxUrl.getFactories,
                            dataType: 'jsonp',
                            type: 'GET',
                            data: { key: q },
                            success: function (json) {
                                var items = $.map(json.Data, function (item, index) {
                                    if (q == item.Name || $.trim(q) == $.trim(item.Name)) {
                                        inputFactoryName.val($.trim(item.Name));
                                        inputFactoryID.val($.trim(item.ID));
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
                valueField: 'ID',
                textField: 'Name',
                formatter: function (row) {
                    var s = '<div><span class="level' + row.Grade + '"></span>' + row.Name + '</div>'
                    return s;
                },
                prompt: options.prompt,
                required: options.required,
                value: options.value,
                onSelect: function (record) {
                    var oldValue = inputFactoryName.val();
                    inputFactoryName.val($.trim(record.Name));
                    inputFactoryID.val($.trim(record.ID));
                    if (myOnChange) {
                        myOnChange(record.ID, oldValue);
                    }
                },
                validType: 'factory["' + sender_id + '"]'
            });

            ////初始化调用的值
            if (myOnChange && value) {
                myOnChange(value, '');
            }

            //结束
        });
    }

    //供应商插件的默认配置
    $.fn.factory.defaults = {
        value: '',
        prompt: '请输入两位以上搜索原厂供应商',
        required: true,
        onChange: null //默认值也需要有意触发
    };

    //供应商插件对外的方法
    $.fn.factory.methods = {
        //获取名称
        getName: function (jq) {
            var sender = $(jq);
            return sender.data('inputs').inputFactoryName.val();
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
    $.parser.plugins.push('factory');
})(jQuery)