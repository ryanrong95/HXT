//单选控件
(function ($) {
    function randomID(model) {
        return model + '_' + parseInt(Math.random() * Math.pow(10, 10))
    }
    //获取单选控件的数据
    function load(url, cb) {
        $.ajax({
            type: "get",
            url: url,
            dataType: "jsonp",
            success: function (data) {
                if (data.Code == "200") {
                    cb(data.Data);
                } else if (data.Code == "300") {
                    console.log("接口异常")
                }
            },
            error: function (err) {
                console.log(err);
            }
        });
    }

    //初始化数据
    function init(sender) {
        var data = $(sender).data("data");
        var options = $(sender).data("options");
        var label = options.labelField;
        var valueField = options.valueField;
        var checked = options.checked;
        var checkedResult = null;
        var flag = false;
        for (var i = 0; i < data.length; i++) {
            var $input = $("<input name='" + options.name + "'>");
            $(sender).append($input);
            var optionsName = options.name + "_" + i;
            $(sender).data(optionsName, $input);//存储input

            if (checked != null && data[i][valueField] == checked) {
                checkedResult = data[i];
                flag = true;
            }
        }
        if (flag == false) {
            checked = data[0][valueField];
            checkedResult = data[0];
        }

        var name1 = options.name + "_result";
        var $Checkeditem = $("<input type='hidden' name=" + name1 + ">");
        $(sender).append($Checkeditem);
        $(sender).data("Checkeditem", $Checkeditem);//存储选中的那条数据

        $(sender).data("saveCheckedValue", checked);//存储选中的那条数据
        $Checkeditem.val(JSON.stringify(checkedResult));

        $(sender).data("saveCheckeditem", checkedResult);//存储选中的那条数据

        $(sender).find("input[name=" + options.name + "]").each(function (index, element) {
            $(this).radiobutton({
                labelWidth: options.labelWidth,
                label: data[index][label],
                labelPosition: options.labelPosition,
                value: data[index][valueField],
                checked: (checked != null && data[index][valueField] == checked) ? true : false,
                onChange: function (x) {
                    $(sender).data("saveCheckedValue", data[index][valueField]);//存储选中的那条数据
                    $Checkeditem.val(JSON.stringify(data[index]));
                    $(sender).data("saveCheckeditem", data[index]);//存储选中的那条数据
                    if (x) {
                        if (options.onChange) {
                            options.onChange(data[index]);
                        }
                    }
                }
            })
        })

    }

    $.fn.radio = function (opt, param) {
        var sender = this;
        //如果options为string，则是方法调用
        if (typeof opt == 'string') {
            var method = $.fn.radio.methods[opt];
            if (method) {
                return method(this, param);//如果有该方法就调用该方法
            } else {
                alert("该插件没有这个方法")
            }
        }
        var options = opt || {};
        var data_options = {};
        var s = $.trim(sender.attr("data-options"));
        //如果html配置了data-options
        if (s) {
            if (s.substring(0, 1) != "{") {
                s = "{" + s + "}";
            }
            data_options = (new Function("return " + s))();
            //$.extend(true, {}, $.fn.radio.defaults, data_options, opt);第一个参数为true，则是深度合并对象
            options = $.extend(true, {}, $.fn.radio.defaults, data_options, opt);
        } else {
            options = $.extend(true, {}, $.fn.radio.defaults, opt);
        }

        //如果已经初始化，就不需要再进行初始化了
        if ($(sender).data("saveCheckeditem")) {
            return;
        }

        $(sender).data("options", options);//存储options的值

        if (options.data && options.data.length > 0) {
            $(sender).data("data", options.data);//存储页面数据
            init(sender);
        } else {
            load(options.url, function (data) {
                $(sender).data("data", data);//存储页面数据
                init(sender);
            })
        }
    }

    $.fn.radio.defaults = {
        name: "name",//input统一的name值
        data: null,//数据
        url: null,//请求数据的接口
        labelPosition: 'after',//label的书写位置
        valueField: null,//value值
        labelField: null,//展示label的属性
        checked: null//选中的值为某一条的valueField
    };

    $.fn.radio.methods = {
        options: function (jq) {
            return $(jq).data("options");//返回options
        },
        checkedValue: function (jq) {
            return $(jq).data("saveCheckedValue");//返回选择的数据value值
        },
        checkeditem: function (jq) {
            return $(jq).data("saveCheckeditem");///返回选择的整条数据
        },
        setCheck: function (jq, param) {
            $(jq).data("saveCheckedValue", param);//存储选中的那条数据
            var options = $(jq).data("options");
            var data = $(jq).data("data");
            var index = 0;
            for (var i = 0; i < data.length; i++) {
                if (data[i][options.valueField] == param) {
                    $(jq).data("Checkeditem").val(JSON.stringify(data[i]));
                    $(jq).data("saveCheckeditem", data[i]);//存储选中的那条数据
                    index = i;
                    break;
                }
            }
            var optionsName = options.name + "_" + index;
            $(jq).data(optionsName).radiobutton("check");
        }
    };
    $.parser.plugins.push('radio');

})(jQuery)
