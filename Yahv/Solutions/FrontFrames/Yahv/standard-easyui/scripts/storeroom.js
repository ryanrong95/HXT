//库房插件

//name              表示调用控件的dom的name属性
//有2个<input type="hidden">的input，以下它们的name值，这些值携带了响应的数据
//name              表示用户输入的库房名称
//name_result       表示某个库房的数据


(function ($) {
    var storeroomListData = null; //库房数据
    var initsItem = [];                 //存储调用改组件的dom元素
    //接口地址
    var AjaxUrl = {
        getstoreroomData: '/csrmapi/WareHouses'//获取库房数据
    };
    function getstoreroomData(cb) {
        $.ajax({
            type: "get",
            url: AjaxUrl.getstoreroomData,
            dataType: "jsonp",
            success: function (data) {
                if (data.Code == "200") {
                    storeroomListData = data.Data;
                    cb(storeroomListData);
                } else if (data.Code == "300") {
                    console.log("接口异常")
                }
            },
            error: function (err) {
                console.log(err);
            }
        });
    }
    //获取全部库房数据
    (function () {
        $.ajax({
            type: "get",
            url: AjaxUrl.getstoreroomData,
            dataType: "jsonp",
            success: function (data) {
                if (data.Code == "200") {
                    storeroomListData = data.Data;
                    for (var index = 0; index < initsItem.length; index++) {
                        $(initsItem[index]).combobox("loadData", storeroomListData);
                    };
                } else if (data.Code == "300") {
                    console.log("接口异常")
                }
            },
            error: function (err) {
                console.log(err);
            }
        });
    })();

    //保存input输入的value值
    function saveVal(sender, val) {
        $(sender).data("val", val)
    }

    //保存某个库房的数据
    function saveResult(sender, result) {
        $(sender).data("result", result)
    }

    //编写storeroom插件
    $.fn.storeroom = function (opt, param) {
        //如果options为string，则是方法调用
        if (typeof opt == 'string') {
            var method = $.fn.storeroom.methods[opt];
            if (method) {
                return method(this, param);//如果有该方法就调用该方法
            } else {
                alert("该插件没有这个方法")
            }
        }

        var options = opt || {};
        var sender = this;

        //data-options,html配置
        (function () {
            var data_options = {};
            var s = $.trim(sender.attr("data-options"));
            if (s) {
                if (s.substring(0, 1) != "{") {
                    s = "{" + s + "}";
                }
                data_options = (new Function("return " + s))();
                options = $.extend(true, {}, $.fn.storeroom.defaults, data_options, options);
            } else {
                options = $.extend(true, {}, $.fn.storeroom.defaults, options);
            }
        })();

        $(sender).data("options", options);
        var name = $(sender).attr("name");
        name = name + "_result";
        var valuer = $('<input type="hidden" name="' + name + '" />');
        sender.before(valuer);

        options.onSelect = function (record) {
            saveVal(sender, record.ID);
            saveResult(sender, record);
            $("input[name='" + name + "']").val(JSON.stringify(record));
        };
        $(sender).combobox(options);

        if (storeroomListData && storeroomListData.length > 0) {
            $(sender).combobox("loadData", storeroomListData);
        } else {
            initsItem.push(sender);
        }
        if (options.value && options.value != "") {
            $(sender).storeroom('setVal', options.value);
        }
    }

    //标准库房名称补齐插件的默认配置
    $.fn.storeroom.defaults = $.extend({}, $.fn.combobox.defaults, {
        width: 200,
        value: null,
        valueField: 'ID',
        textField: 'Name',
        prompt: '请选择库房',
        editable: false,
        required: true,
        tipPosition: 'right',
        novalidate: true,
        missingMessage: '库房不能为空',
        panelHeight: 'auto',
        panelMaxHeight: 300,
        onSelect:null//选择下拉时
    });
    //标准库房名称补齐插件对外的方法
    $.fn.storeroom.methods = {
        //获取input框的值
        getVal: function (jq) {
            return $(jq).data('val')
        },
        //设置input框的值
        setVal: function (jq, param) {
            var valueField = $(jq).data('options').valueField;
            if (storeroomListData && storeroomListData.length > 0) {
                for (var i = 0; i < storeroomListData.length; i++) {
                    if (storeroomListData[i].Name == param) {
                        $(jq).combobox("loadData", storeroomListData);
                        $(jq).combobox("select", storeroomListData[i][valueField]);
                    }
                }
            } else {
                getstoreroomData(function (data) {
                    if (data.length > 0) {
                        for (var i = 0; i < data.length; i++) {
                            if (data[i].Name == param) {
                                $(jq).combobox("loadData", data);
                                $(jq).combobox("select", data[i][valueField]);
                            }
                        }
                    }
                })
            }
        },
        //获取库房options
        options: function (jq) {
            return $(jq).combobox('options')
        },
        //获取某个库房的数据
        getResult: function (jq) {
            return $(jq).data('result');
        },
        //获取插件携带的所有值
        getAllData: function (jq) {
            return {
                options: $(jq).combobox('options'),
                val: $(jq).data('val'),
                result: $(jq).data('result')
            }
        }
    };
    $.parser.plugins.push('storeroom'); //将自定义的插件加入 easyui 的插件组    
})(jQuery)