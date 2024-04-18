//代理品牌插件（可以输入可以选择）

if (v_RFQ_3_0) {
    var url = document.scripts[document.scripts.length - 1].src;
    var lower = url.toLowerCase();
    var prexUrl = url.substring(0, lower.indexOf('/yahv/') + '/yahv/'.length);
    writeScript('' + prexUrl + '/standard-easyui/v_RFQ_3_0/agentBrand.js');
}
else 
(function ($) {
    $.extend($.fn.validatebox.defaults.rules, {
        agentBrand: {
            validator: function (value, param) {
                var sender = $('#' + param[0]);
                var data = sender.combobox('getData');
                if (data == null) {
                    return false;
                }
                var og = $.grep(data, function (item, index) {
                    return $.trim(item.Name) == $.trim(value);
                })
                return og.length > 0;
            },
            message: "必须选择代理品牌"
        }
    })
    //接口地址
    var AjaxUrl = {
        getagentBrands: '/csrmapi/Brands/Agents'//通过品牌名称搜索代理品牌
    }

    function ajaxData(async, uri, cb) {
        $.ajax({
            async: async,
            type: "get",
            url: uri,
            dataType: "json",
            success: function (json) {
                if (cb) {
                    cb(json)
                }
            },
            error: function (err) {
                alert('error:' + JSON.stringify(err));
            }
        });
    }

    /// if (!top.$.baseData) {
    //   top.$.baseData = {};
    // }

    //业务类型
    // if (!top.$.baseData.agentBrands) {
    //     ajaxData(false, AjaxUrl.getagentBrands, function (json) {
    //      top.$.baseData.agentBrands = json.Data;
    //   });
    //  }

    //临时写法很要命！
    //setInterval(function () {
    //    ajaxData(true, AjaxUrl.getagentBrands, function (json) {
    //        top.$.baseData.agentBrands = json.Data;
    //    });
    //}, 1000);

    //编写agentBrand插件
    $.fn.agentBrand = function (opt, param) {
        var sender = $(this);
        //如果options为string，则是方法调用
        if (typeof opt == 'string') {
            var method = $.fn.agentBrand.methods[opt];
            if (method) {
                return method(this, param);//如果有该方法就调用该方法
            } else {
                alert("该插件没有这个方法")
            }
        }
        var options = opt || {};
        options = $.extend(true, {}, $.fn.agentBrand.defaults, options);

        return this.each(function () {
            var sender = $(this);
            //保存设置类型
            sender.data('options', options);

            var sender_id = sender.prop('id');
            if (!sender_id) {
                sender_id = 'agentbrand_sender_' + Math.random().toString().substring(2);
                sender.prop('id', sender_id);
            }

            var formName = sender.prop('name');
            if (!formName) {
                alert('agentBrand.控件name不能为空！');
            }
            var value = sender.val();
            if (value) {
                options.value = value;
            }
            //  var initdata = top.$.baseData.agentBrands;
            // sender.data('items', initdata);

            sender.combobox({
                loader: function (param, success, error) {
                    $.ajax({
                        url: AjaxUrl.getagentBrands,
                        dataType: 'json',
                        type: 'GET',
                        success: function (json) {
                            var selected = null;
                            var items = $.map(json.Data, function (item, index) {
                                var o = {
                                    ID: item.ID,
                                    Name: item.Name,
                                };
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
                },
                valueField: 'ID',
                textField: 'Name',
                panelHeight: 'auto',
                prompt: options.prompt,
                required: true,
                value: options.value,
                validType: 'agentBrand["' + sender_id + '"]',
                onChange: options.onChange
            });
        })
    }


    //代理品牌插件的默认配置
    $.fn.agentBrand.defaults = {
        value: '',
        prompt: '请择代理品牌',
        required: true,
        onChange: function (newValue) { //默认值也需要有意触发

        }
    };

    //代理品牌插件对外的方法
    $.fn.agentBrand.methods = {
        //获取名称
        getName: function (jq) {
            var sender = $(jq);
            return sender.combobox('setValue');
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
    $.parser.plugins.push('agentBrand');
})(jQuery)