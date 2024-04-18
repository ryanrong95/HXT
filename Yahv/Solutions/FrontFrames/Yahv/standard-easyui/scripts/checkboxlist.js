//多选控件
(function ($) {
    function randomID(model) {
        return model + '_' + parseInt(Math.random() * Math.pow(10, 10))
    }
    //获取多选数据
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
        var checkedResult = [];
        var flag = false;
        for (var i = 0; i < data.length; i++) {
            var $input = $("<input name='" + options.name + "'>");
            $(sender).append($input);
            var optionsName = options.name + "_" + i;
            $(sender).data(optionsName, $input);//存储input

            if (checked != null && checked.length > 0) {
                for (var j = 0; j < checked.length; j++) {
                    if (data[i][valueField] == checked[j]) {
                        checkedResult.push(data[i]);
                    }
                }
                flag = true;
            }
        }
        if (flag == false) {
            checked = [];
            checked.push(data[0][valueField]);
            checkedResult.push(data[0]);
        }

        var name1 = options.name + "_result";
        var $Checkeditem = $("<input name=" + name1 + " style='width:0;height:0;border:none;' />");
        $(sender).append($Checkeditem);
        $(sender).data("Checkeditem", $Checkeditem);//存储选中的那条数据

        var savecheckValue = checked;
        $(sender).data("saveCheckedValue", savecheckValue);//存储选中的那条数据

        $(sender).data("Checkeditem").val(JSON.stringify(checkedResult));
        $(sender).data("saveCheckeditem", checkedResult);//存储选中的那条数据

        $(sender).find("input[name=" + options.name + "]").each(function (index, element) {
            $(this).checkbox({
                label: data[index][label],
                labelPosition: options.labelPosition,
                value: data[index][valueField],
                checked: (checked.indexOf(data[index][valueField]) != -1) ? true : false,
                onChange: function (x) {
                    var saveCheckeditem = $(sender).data("saveCheckeditem");
                    var flag = false;
                    for (var i = 0; i < saveCheckeditem.length; i++) {
                        if (saveCheckeditem[i][valueField] == data[index][valueField]) {
                            flag = true;
                        } else {
                            flag = false;
                        }
                    }

                    if (flag) {
                        saveCheckeditem = saveCheckeditem.filter(function (item) {
                            return item[valueField] != data[index][valueField]
                        })
                    } else {
                        saveCheckeditem.push(data[index]);
                    }

                    $(sender).data("saveCheckeditem", saveCheckeditem);//存储选中的那些数据

                    if (saveCheckeditem.length == 0) {
                        $(sender).data("Checkeditem").val(null);
                    } else {
                        $(sender).data("Checkeditem").val(JSON.stringify(saveCheckeditem));
                    }

                    savecheckValue = [];
                    for (var j = 0; j < saveCheckeditem.length; j++) {
                        savecheckValue.push(saveCheckeditem[j][valueField]);
                    }

                    $(sender).data("saveCheckedValue", savecheckValue);//存储选中的那些数据的valueField的值
                    //当选中的时候调用onselect，否则会产生死循环 
                    if (options.onSelect && x == true) {
                        options.onSelect(sender, data[index]);
                    }

                }
            })
        })

        $(sender).data("Checkeditem").validatebox({
            required: options.required || false,
            missingMessage: options.missingMessage || "该项不能为空"
        })
    }

    $.fn.checkboxlist = function (opt, param) {
        var sender = this;
        //如果options为string，则是方法调用
        if (typeof opt == 'string') {
            var method = $.fn.checkboxlist.methods[opt];
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
            //$.extend(true, {}, $.fn.checkboxlist.defaults, data_options, opt);第一个参数为true，则是深度合并对象
            options = $.extend(true, {}, $.fn.checkboxlist.defaults, data_options, opt);
        } else {
            options = $.extend(true, {}, $.fn.checkboxlist.defaults, opt);
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

    $.fn.checkboxlist.defaults = {
        name: "name",//input统一的name值
        data: null,//数据
        url: null,//请求数据的接口
        labelPosition: 'after',//label的书写位置
        valueField: null,//value值
        labelField: null,//展示label的属性
        checked: null,//选中的值为某几条的valueField ["a,b"]
        required: false
    };

    $.fn.checkboxlist.methods = {
        options: function (jq) {
            return $(jq).data("options");//返回options
        },
        checkedValue: function (jq) {
            return $(jq).data("saveCheckedValue");//返回options
        },
        checkeditem: function (jq) {
            return $(jq).data("saveCheckeditem");//返回options
        },
        setCheck: function (jq, param) {
            $(jq).data("saveCheckedValue", param);//存储选中的那些数据
            var options = $(jq).data("options");
            var data = $(jq).data("data");
            var saveCheckeditem = [];
            for (var i = 0; i < param.length; i++) {
                for (var j = 0; j < data.length; j++) {
                    if (data[j][options.valueField] == param[i]) {
                        saveCheckeditem.push(data[j]);
                        var optionsName = options.name + "_" + j;
                        $(jq).data(optionsName).checkbox("check");
                    } else {
                        var kk = 0;
                        for (var k = 0; k < saveCheckeditem.length; k++) {
                            if (saveCheckeditem[k][options.valueField] == param[i]) {
                                kk++;
                                break;
                            }
                        }
                        if (kk == saveCheckeditem.length) {
                            var optionsName = options.name + "_" + j;
                            $(jq).data(optionsName).checkbox("uncheck");
                        }
                    }
                }
            }
            $(jq).data("Checkeditem").val(JSON.stringify(saveCheckeditem));
            $(jq).data("saveCheckeditem", saveCheckeditem);//存储选中的那条数据
        }
    };
    $.parser.plugins.push('checkboxlist');

})(jQuery)
