/// <reference path="../../jquery-easyui-1.7.6/jquery.easyui.min.js" />
/// <reference path="../../jquery-easyui-1.7.6/jquery.min.js" />

(function ($) {
    //扩展验证规则
    $.extend($.fn.validatebox.defaults.rules, {
        agentBrand: {
            validator: function (value, param) {
                var sender = $('#' + param[0]);
                var data = sender.agentBrandCrmPlus('getData');
                if (data == null) {
                    return false;
                }

                var og = $.grep(data, function (item, index) {
                    return $.trim(item.Name) == $.trim(value);
                });

                return og.length > 0;
            },
            message: '必须选择已有代理品牌!'
        }
    });

    //接口地址
    var ajaxUrl = {
        getAgentBrands: '/crmplusapi/Standard/AgentBrands'
    }

    //仅供内部调用的方法
    var innerMethods = function () { };
    innerMethods.prototype = {

        //ajax请求代理品牌数据
        getAgentBrands: function (sender, value, success) {
            $.ajax({
                url: ajaxUrl.getAgentBrands,
                dataType: 'json',
                type: 'GET',
                data: { key: value },
                success: function (json) {
                    var selected = null;
                    var items = $.map(json.data, function (item, index) {
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
                    alert('agentBrand.error:' + JSON.stringify(err));
                }
            });
        },

    }

    //编写agentBrand插件
    $.fn.agentBrandCrmPlus = function (opt, param) {
        //如果options为string，则是方法调用
        if (typeof opt == 'string') {
            var method = $.fn.agentBrandCrmPlus.methods[opt];
            if (method) {
                return method(this, param);//如果有该方法就调用该方法
            } else {
                alert("agentBrand.插件没有这个方法: " + opt)
            }
        }

        var options = opt || {};
        options = $.extend(true, {}, $.fn.agentBrandCrmPlus.defaults, options);

        return this.each(function () {
            var sender = $(this);
            //保存设置类型
            sender.data('options', options);

            var myMethods = new innerMethods();

            //获取控件id
            var sender_id = sender.prop('id');
            if (!sender_id) {
                sender_id = 'agentBrand_sender_' + Math.random().toString().substring(2);
                sender.prop('id', sender_id);
            }

            //控件属性验证
            var formName = sender.prop('name');
            if (!formName) {
                alert('agentBrand.控件name不能为空!');
            }

            var setTimeoutAjax = 0;

            sender.combobox({
                loader: function (param, success, error) {
                    //输入两位以上再进行搜索
                    var q = $.trim(sender.combobox('getText'));
                    if (q.length < 2) {
                        return false;
                    }

                    //查询客户数据
                    clearTimeout(setTimeoutAjax);
                    setTimeoutAjax = setTimeout(myMethods.getAgentBrands(sender, q, success), 100);
                },
                mode: 'remote',
                valueField: 'ID',
                textField: 'Name',
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
                validType: 'agentBrand["' + sender_id + '"]'
            });
        });
    }

    //插件的默认配置
    $.fn.agentBrandCrmPlus.defaults = {
        prompt: '请输入两位以上搜索代理品牌',
        required: true,
        onChange: function (newValue, oldValue) {

        },
        onSelect: function (record) {
        },
    };

    //插件对外的方法
    $.fn.agentBrandCrmPlus.methods = {
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
    $.parser.plugins.push('agentBrandCrmPlus');
})(jQuery);