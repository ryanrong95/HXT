//原产地插件（可以输入可以选择）

//name              表示调用控件的dom的name属性
//有2个<input type="hidden">的input，以下它们的name值，这些值携带了响应的数据
//name              表示用户输入的原产地名称
//name_result       表示某个原产地的数据


(function ($) {
    //保存input输入的value值
    function saveVal(sender, val) {
        $(sender).data("val", val)
    }
    //保存某个原产地的数据
    function saveResult(sender, result) {
        $(sender).data("result", result)
    }
    //编写originPlace插件
    $.fn.originPlace = function (opt, param) {
        //如果options为string，则是方法调用
        if (typeof opt == 'string') {
            var method = $.fn.originPlace.methods[opt];
            if (method) {
                return method(this, param);//如果有该方法就调用该方法
            } else {
                alert("该插件没有这个方法")
            }
        }

        var options = opt || {};
        var sender = this;
        //html配置
        (function () {
            var data_options = {};
            var s = $.trim(sender.attr("data-options"));
            if (s) {
                if (s.substring(0, 1) != "{") {
                    s = "{" + s + "}";
                }
                data_options = (new Function("return " + s))();
                options = $.extend(true, {}, $.fn.originPlace.defaults, data_options, options);
            } else {
                options = $.extend(true, {}, $.fn.originPlace.defaults, options);
            }
        })();
        //如果已经初始化，就不需要再进行初始化了
        if ($(sender).data("dom_result")) {
            return;
        }
        $(sender).data("options", options);
        function init() {
            var name = $(sender).attr("name") + "_result";
            var valuer = $('<input type="hidden" name="' + name + '" />');//保存原厂地的input元素
            sender.before(valuer);

            $(sender).data("dom_result", valuer);//存储保存原产地的input元素

            $(sender).data("data", originPlaceData);//存储保存原产地数据

            options.validType = "OnlySelectDropValue['" + $(sender).attr("id") + "']";

            options.onChange = function (n, o) {
                saveVal(sender, n);
                if (options.isOnlySelectDropValue) {
                    $(sender).combobox("enableValidation");
                }
            };
            options.onSelect = function (record) {
                saveVal(sender, record.Name);
                saveResult(sender, record);
                $(sender).data("dom_result").val(JSON.stringify(record));
            };
            $(sender).data("options", options);
            $(sender).combobox(options);
        }
        init.call(this);

        if (originPlaceData && originPlaceData.length > 0) {
            $(sender).combobox("loadData", originPlaceData);
        }

        if (options.value && options.value != "") {
            $(sender).originPlace('setVal', options.value);
        }
    }

    //标准原产地名称补齐插件的默认配置
    $.fn.originPlace.defaults = $.extend({}, $.fn.combobox.default, {
        width: 260,
        valueField: 'Name',
        textField: 'Name',
        prompt: '请选择原产地',
        panelMaxHeight: 300,
        required: true,
        missingMessage: '原产地不能为空',
        novalidate: true,
        tipPosition: 'right',
        value: null,
        validType: null,
        onChange: null,
        onSelect: null,
        isOnlySelectDropValue: false
    });

    //标准原产地名称补齐插件对外的方法
    $.fn.originPlace.methods = {
        //获取input框的值
        getVal: function (jq) {
            return $(jq).data('val')
        },
        //设置input框的值
        setVal: function (jq, param) {
            var valueField = $(jq).data('options').valueField;
            var data = $(jq).data("data");
            if (data && data.length > 0) {
                for (var i = 0; i < data.length; i++) {
                    if (data[i][valueField] == param) {
                        $(jq).combobox("select", data[i][valueField]);
                        break;
                    }
                }
            }
        },
        //获取原产地options
        options: function (jq) {
            return $(jq).data('options');
        },
        //获取某个原产地的数据
        getResult: function (jq) {
            return $(jq).data('result');
        },
        //获取插件携带的所有值
        getAllData: function (jq) {
            return {
                options: $(jq).data('options'),
                val: $(jq).data('val'),
                result: $(jq).data('result')
            }
        }
    };
    //将自定义的插件加入 easyui 的插件组
    $.parser.plugins.push('originPlace');
})(jQuery)