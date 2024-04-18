//代理品牌与企业关系插件插件（可以输入可以选择）

(function ($) {
    $.extend($.fn.validatebox.defaults.rules, {
        agentMaps: {
            validator: function (value, param) {
                var sender = $('#' + param[0]);
                var data = sender.combobox('getData');

                //alert(JSON.stringify(data));

                if (data == null) {
                    return false;
                }
                var og = $.grep(data, function (item, index) {
                    return $.trim(item.EnterpriseName) == $.trim(value);
                })
                return og.length > 0;
            },
            message: "必须选择已有的企业"
        }
    })

    //接口地址
    var AjaxUrl = {
        getagentMaps: '/csrmapi/Brands/Maps'//通过品牌名称搜索代理品牌
    }

    //编写agentMaps插件
    $.fn.agentMaps = function (opt, param) {
        //如果options为string，则是方法调用
        if (typeof opt == 'string') {
            var method = $.fn.agentMaps.methods[opt];
            if (method) {
                return method(this, param);//如果有该方法就调用该方法
            } else {
                alert("该插件没有这个方法")
            }
        }
        var options = opt || {};
        options = $.extend(true, {}, $.fn.agentMaps.defaults, options);

        var initData;
        $.ajax({
            async: false,
            url: AjaxUrl.getagentMaps + "?type=" + options.type + '&id=' + options.brandID,
            dataType: 'json',
            type: 'GET',
            success: function (json) {
                initData = json.Data;
            },
            error: function (err) {
                alert('agentMaps.error:' + JSON.stringify(err));
            }
        });

        return this.each(function () {
            var sender = $(this);
            sender.data('options', options);
            var sender_id = sender.prop('id');
            if (!sender_id) {
                sender_id = 'agentMaps_sender_' + Math.random().toString().substring(2);
                sender.prop('id', sender_id);
            }

            var formName = sender.prop('name') || sender.attr('textboxname');
            if (!formName) {
                alert('agentMaps控件name不能为空！');
            }
            var value = sender.val();
            if (value) {
                options.value = value;
            }

            sender.combobox({
                data: initData,
                valueField: 'EnterpriseID',
                textField: 'EnterpriseName',
                method: 'get',
                panelHeight: 'auto',
                //formatter: function (row) {
                //    //建议未来把是否远程能够带出来！
                //    var s = '<div class="agentMaps" style="text-align:left;">' +
                //   '<span><em style="display:inline-block; width:78px">品牌:</em>' + row.BrandName + '</span><br>' +
                //   '<span><em style="display:inline-block; width:78px">关系企业:</em>' + row.EnterpriseName + '</span><br>' +
                //   '</div> ';
                //    return s;
                //},
                prompt: options.prompt,
                required: options.required,
                value: options.value,
                validType: 'agentMaps["' + sender_id + '"]',
                onChange: options.onChange,
            });
        })
    }


    //代理品牌插件的默认配置
    $.fn.agentMaps.defaults = {
        type: 'Company', //Company,Supplier
        value: '',
        prompt: '请输入两位以上搜索品牌',
        required: true,
        onChange: function () {//默认值也需要有意触发

        },
        brandID: '',
    };

    //代理品牌插件对外的方法
    $.fn.agentMaps.methods = {

        //获取数据
        getData: function (jq) {
            var sender = $(jq);
            return sender.data('items');
        }
        ,
        //获取数据
        setValue: function (jq, value) {

            alert('原则上不会使用本方法');
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
    $.parser.plugins.push('agentMaps');
})(jQuery)