//内部公司插件

//name              表示调用控件的dom的name属性
//有2个<input type="hidden">的input，以下它们的name值，这些值携带了响应的数据
//name              表示用户输入的内部公司名称
//name_result       表示某个内部公司的数据


(function ($) {
    var InternalCompanyListData = null; //内部公司数据
    var initsItem = [];                 //存储调用改组件的dom元素
    //接口地址
    var AjaxUrl = {
        getInternalCompanyData: '/csrmapi/Companies'//获取内部公司数据
    };
    //获取全部内部公司数据的方法
    function getInternalCompanyData(cb) {
        $.ajax({
            type: "get",
            url: AjaxUrl.getInternalCompanyData,
            dataType: "jsonp",
            success: function (data) {
                if (data.Code == "200") {
                    InternalCompanyListData = data.Data;
                    cb(InternalCompanyListData);
                } else if (data.Code == "300") {
                    console.log("接口异常")
                }
            },
            error: function (err) {
                console.log(err);
            }
        });
    }

    //获取全部内部公司数据
    getInternalCompanyData(function (InternalCompanyListData) {
        for (var index = 0; index < initsItem.length; index++) {
            $(initsItem[index]).combobox("loadData", InternalCompanyListData);
        };
    })

    //保存input输入的value值
    function saveVal(sender, val) {
        $(sender).data("val", val)
    }

    //保存某个内部公司的数据
    function saveResult(sender, result) {
        $(sender).data("result", result);
        if (result == null) {
            $(sender).data("dom_result").val(null);
        } else {
            $(sender).data("dom_result").val(JSON.stringify(result));
        }
    }

    function setValByName(jq, data, param) {
        if (param) {
            var valueField = $(jq).data('options').valueField;
            for (var i = 0; i < data.length; i++) {
                if (data[i].Name == param) {
                    $(jq).combobox("loadData", data);
                    $(jq).combobox("select", data[i][valueField]);
                    saveVal(jq, data[i][valueField]);
                    saveResult(jq, data[i]);
                }
            }
        } else {
            $(jq).combobox("select", null);
            saveVal(jq, null);
            saveResult(jq, null);
        }
    }

    //编写InternalCompany插件
    $.fn.InternalCompany = function (opt, param) {
        //如果options为string，则是方法调用
        if (typeof opt == 'string') {
            var method = $.fn.InternalCompany.methods[opt];
            if (method) {
                return method(this, param);//如果有该方法就调用该方法
            } else {
                alert("该插件没有这个方法")
            }
        }

        var options = opt || {};
        var sender = this;

        //data-options,html配置
        var data_options = {};
        var s = $.trim(sender.attr("data-options"));
        if (s) {
            if (s.substring(0, 1) != "{") {
                s = "{" + s + "}";
            }
            data_options = (new Function("return " + s))();
            options = $.extend(true, {}, $.fn.InternalCompany.defaults, data_options, options);
        } else {
            options = $.extend(true, {}, $.fn.InternalCompany.defaults, options);
        }
        $(sender).data("options", options);
        var name = $(sender).attr("name");
        name = name + "_result";
        var valuer = $('<input type="hidden" name="' + name + '" />');
        sender.before(valuer);

        $(sender).data("dom_result", valuer);

        options.onSelect = function (record) {
            saveVal(sender, record.ID);
            saveResult(sender, record);
            if (options.target) {
                $(options.target).InternalCompanyBeneficiary('loadDataByID', {
                    id: record.ID, cb: function (data) {
                        if (data && data.length > 0) {
                            $(options.target).combobox("loadData", data);
                            if (data.length == 1) {
                                $(options.target).combobox("select", data[0].ID);
                                $(options.target).data('result', data[0]);
                                var name = $(options.target).next().find("input.validatebox-text").next().attr("name") + "_result";
                                $("input[name='" + name + "']").val(JSON.stringify(data[0]));
                            } else {
                                $(options.target).combobox("showPanel");
                                if (options.onDoChange) {
                                    options.onDoChange(data)
                                }
                            }
                        }
                    }
                })
            }

        }
        options.validType = "OnlySelectDropValue['" + $(sender).attr("id") + "']";
        options.onLoadSuccess = function () {
           $(sender).combobox("enableValidation");
        }
        options.onChange = function (n, o) {
            if (typeof (n) == "string") {
                saveVal(sender, n)
                var data = $(sender).combobox("getData");
                var idx = 0;
                for (var i = 0; i < data.length; i++) {
                    idx++;
                    if (n == data[i].ID) {
                        break;
                    }
                }
                if (idx == data.length) {
                    if (options.target) {
                        $(options.target).combobox("clear");
                        $(options.target).combobox("loadData", []);
                    }
                    saveResult(sender, null);
                }
            }
        }
        $(sender).data("options", options);
        $(sender).combobox(options);

        if (InternalCompanyListData && InternalCompanyListData.length > 0) {
            $(sender).combobox("loadData", InternalCompanyListData);
        } else {
            initsItem.push(sender);
        }

        if ($(sender).data("options").value && $(sender).data("options").value != "") {
            $(sender).InternalCompany('setVal', options.value);
        }
    }

    //标准内部公司名称补齐插件的默认配置
    $.fn.InternalCompany.defaults = $.extend({}, $.fn.combobox.defaults, {
        width: 250,
        valueField: 'ID',
        textField: 'Name',
        prompt: '请选择公司',
        required: true,
        tipPosition: 'right',
        novalidate: true,
        missingMessage: '内部公司不能为空',
        panelHeight: 'auto',
        panelMaxHeight: 300,
        value: null,
        onChange: null,
        onLoadSuccess: null,
        validateOnBlur: true,
        target: null
    });
    //标准内部公司名称补齐插件对外的方法
    $.fn.InternalCompany.methods = {
        //获取input框的值
        getVal: function (jq) {
            return $(jq).data('val')
        },
        //设置input框的值
        setVal: function (jq, param) {
            if (InternalCompanyListData && InternalCompanyListData.length > 0) {
                setValByName(jq, InternalCompanyListData, param);
            } else {
                getInternalCompanyData(function (data) {
                    setValByName(jq, data, param);
                })
            }
        },
        //获取内部公司options
        options: function (jq) {
            return $(jq).data('options')
        },
        //获取某个内部公司的数据
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
    $.parser.plugins.push('InternalCompany'); //将自定义的插件加入 easyui 的插件组    
})(jQuery)