//标准型号名称补齐插件（可以输入可以选择）

//name              表示调用控件的dom的name属性
//有2个<input type="hidden">的input，以下它们的name值，这些值携带了响应的数据
//name              表示用户输入的标准型号名称补齐名称
//name_result       表示某个标准型号名称补齐的数据

(function ($) {
    $.extend($.fn.combobox.defaults.rules, {
        isNull: {
            validator: function (value) {
                return $.trim(value)!=''
            },
            message: '型号不能为空'
        }
    });
    //接口地址
    var AjaxUrl = {
        getStandardModels: ajaxPrexUrl + '/Api/standard/models?key=',
        IsStandardModel: ajaxPrexUrl + '/Api/validate/models?key='
    }
    //获取产品信息
    function getStandardModels(model, cb) {
        $.ajax({
            type: "get",
            url: AjaxUrl.getStandardModels + model,
            dataType: "jsonp",
            success: function (data) {
                cb(data);
            },
            error: function (err) {
                console.log(err);
            }
        });
    }
    //验证产品是否为标准型号
    function IsStandardModel(model, cb) {
        $.ajax({
            type: "get",
            url: AjaxUrl.IsStandardModel + model,
            dataType: "jsonp",
            success: function (data) {
                cb(data);
            },
            error: function (err) {
                console.log(err);
            }
        });
    }
    //设置边框颜色
    function setBorder(ele, color) {
        $(ele).next().css("border-color", color);
    }
    //设置背景提示图片
    function setBgImg(ele, className, title) {
        $em = $("<em title=" + title + ">")
        $em.addClass(className);
        //$em.css({ "margin-top": "5px", "margin-right": "5px" });
        $em.css({ "margin-right": "5px" });
        $(ele).next().append($em);
    }
    //计算input的宽度
    function comInputWidth(inputw, ele) {
        var widthTotal = 0;
        $(ele).next().find("em").each(function () {
            widthTotal += $(this).width() + 5;
        })
        $(ele).next().find("input.validatebox-text").css("margin-right", "0")
        var w = inputw - widthTotal;
        $(ele).next().find("input.validatebox-text").width(w);
    }
    //失去焦点或者选中时验证是否是CCC或禁运型号或者是否需要CIQ检测，符合条件添加响应的标志
    function vailHint(data, sender, inputw) {
        //把上一个型号的水印都去掉
        $(sender).next().find("em").remove();
        //额外关税
        if (data.AddedTariffs > 0) {
            setBgImg(sender, "icon-AddedTariffs", "额外关税");
            setBorder(sender, "#ff5722");
        }
        //关税率
        if (data.TariffRate > 0) {
            setBgImg(sender, "icon-tariff", "关税");
            setBorder(sender, "#ff5722");
        }
        //CCC(中国强制认证)
        if (data.NeedCCCCertificate) {
            setBgImg(sender, "icon-CCC", "CCC");
            setBorder(sender, "#ff5722");
        }
        //禁运
        if (data.IsEmbargo) {
            setBgImg(sender, "icon-embargo", "禁运");
            setBorder(sender, "#ff5722");
        }
        //CIQ(中华人民共和国出入境检验检疫局)
        if (data.IsInsp) {
            setBgImg(sender, "icon-CIQ", "CIQ");
            setBorder(sender, "#ff5722");
        }
        comInputWidth(inputw, sender);
    }
    //验证数据是否标准
    function verifyData(sender, inputw, cb3) {
        IsStandardModel($(sender).val(), function (data) {
            if (data.Standard == "bug") {
                setBorder(sender, "#ffc107");//型号不是标准可补齐（黄色）
            } else if (data.Standard == "no") {
                setBorder(sender, "#FF9800");//型号不存在（橙色）
            } else if (data.Standard == "yes") {
                setBorder(sender, "#D3D3D3");//型号存在
            }
            vailHint(data, sender, inputw)
            cb3(data);
        })
    }
    //input输入框值为空时恢复原始状态
    function revertState(inputw, sender) {
        setBorder(sender, "#D3D3D3");
        $(sender).next().find("em").remove();
        comInputWidth(inputw, sender);
    }
    //保存input输入的value值
    function saveVal(sender, val) {
        $(sender).data("val", val)
    }
    //保存标准型号的数据
    function saveResult(sender, result) {
        $(sender).data("result", result)
    }

    //保存检验是否是标准型号的数据
    function saveValidateResult(sender, result) {
        $(sender).data("validateResult", result)
    }

    //设置型号结果数据
    function setDom_result(sender, data) {
        if (data != null) {
            $(sender).data('dom_result').val(JSON.stringify(data));
        } else {
            $(sender).data('dom_result').val(null);
        }
    }

    //设置型号结果验证数据
    function setDom_validateResult(sender, data) {
        if (data != null) {
            $(sender).data('dom_validateResult').val(JSON.stringify(data));
        } else {
            $(sender).data('dom_validateResult').val(null);
        }
    }

    //编写standardModel插件
    $.fn.standardModel = function (opt, param) {
        //如果options为string，则是方法调用
        if (typeof opt == 'string') {
            var method = $.fn.standardModel.methods[opt];
            if (method) {
                return method(this, param);//如果有该方法就调用该方法
            } else {
                alert("该插件没有这个方法")
            }
        }

        var options = opt || {};
        var sender = this;
        var standardModelListData = null;//下拉标准型号名称补齐数据
        var IsSelected = false;//判断该型号是否为下拉获取的
        var changeVal;  //存储blur时验证的数据
        //data-options,html配置
        (function () {
            var data_options = {};
            var s = $.trim(sender.attr("data-options"));
            if (s) {
                if (s.substring(0, 1) != "{") {
                    s = "{" + s + "}";
                }
                data_options = (new Function("return " + s))();
                options = $.extend(true, {}, $.fn.standardModel.defaults, data_options, options);
            } else {
                options = $.extend(true, {}, $.fn.standardModel.defaults, options);
            }
        })();

        //如果已经初始化，就不需要再进行初始化了
        if ($(sender).data("dom_result")) {
            return;
        }

        $(sender).data("options", options);

        var inputw;//组件初始化后input的宽度
        //添加隐藏的input存储选择结果
        var name = $(sender).attr("name");
        var name1 = name + "_result";
        var valuer = $('<input type="hidden" name="' + name1 + '" />');
        sender.before(valuer);
        $(sender).data('dom_result', valuer);//存储保存结果的input元素
        var name2 = name + "_validateResult";
        var valuer2 = $('<input type="hidden" name="' + name2 + '" />');
        sender.before(valuer2);
        $(sender).data('dom_validateResult', valuer2);//存储保存验证结果的input元素
        //从远程获取数据
        var myloader = function (param, success, error) {
            var q = param.q || '';
            if (q.length <= 2) { return false };
            $.ajax({
                url: AjaxUrl.getStandardModels + q,
                dataType: 'jsonp',
                success: function (data) {
                    standardModelListData = data;
                    success(data);
                },
                error: function () {
                    error.apply(this, arguments);
                }
            });
        }

        options.loader = debounce(myloader, 200);
        options.onChange = function (n, o) {
            if (!inputw) {
                inputw = $(sender).next().find("input.validatebox-text").width();//input框的宽度
            }
            if (n == "") {
                IsSelected = false;
                setBorder(sender, "#D3D3D3");
                saveResult(sender, null);
                saveValidateResult(sender, null);

                setDom_result(sender, null);
                setDom_validateResult(sender, null);

                revertState(inputw, sender);
            }
            saveVal(sender, n)
        };
        options.onLoadSuccess = function () {
            //启用验证
            var newVal = $(sender).combobox('getValue');
            if (newVal != "") {
                var data = $(sender).combobox('getData');
                if (data.length) {
                    for (var i = 0; i < data.length; i++) {
                        if (newVal == data[i].Model) {
                            $(sender).combobox("select", data[i].Model);
                            $(sender).combobox("hidePanel");
                            setBorder(sender, "#D3D3D3");
                            vailHint(data[i], sender, inputw);
                            saveVal(sender, data[i].Model)
                            saveResult(sender, data[i]);
                            saveValidateResult(sender, null);

                            saveResult(sender, data[i]);
                            setDom_validateResult(sender, null);

                            IsSelected = true;

                            break;
                        } else {
                            IsSelected = false;
                            saveResult(sender, { "Model": newVal });
                            setDom_result(sender, { "Model": newVal });
                        }
                    }
                } else {
                    IsSelected = false;
                    saveResult(sender, { "Model": newVal });
                    setDom_result(sender, { "Model": newVal });
                }
            }
        };
        options.onSelect = function (record) {
            if (!inputw) {
                inputw = $(sender).next().find("input.validatebox-text").width();//input框的宽度
            }
            IsSelected = true;
            //回复原本的边框颜色
            setBorder(sender, "#D3D3D3");
            //验证是否是3c，禁运等等
            vailHint(record, sender, inputw);
            //保存值
            saveVal(sender, record.Model);

            saveResult(sender, record);
            saveValidateResult(sender, null);

            if (options.onDoChange) {
                options.onDoChange(record)
            }
            //保存值
            setDom_result(sender, record);
            setDom_validateResult(sender, null);
        };

        $(sender).combobox(options);

        if (options.value && options.value != "") {
            $(sender).standardModel('setVal', options.value);
        }
        //失去焦点事件
        $(sender).combobox('textbox').bind('blur', function (e) {
            if (!inputw) {
                inputw = $(sender).next().find("input.validatebox-text").width();
            }
            var val = $(sender).val();

            if (val == "") {
                revertState(inputw, sender);
            } else if (val != "" && !IsSelected && changeVal != val) {
                verifyData(sender, inputw, function (data) {
                    IsSelected = false;
                    changeVal = val;
                    saveVal(sender, val);
                    saveResult(sender, { "Model": val });
                    setDom_result(sender, { "Model": val });
                    saveValidateResult(sender, data);
                    setDom_validateResult(sender, data);
                })
            }
        });
    }

    //标准型号名称补齐插件的默认配置
    $.fn.standardModel.defaults = $.extend({}, $.fn.combobox.defaults, {
        value: null,
        width: 200,
        valueField: 'Model',
        textField: 'Model',
        loader: null,
        required: true,
        prompt: '请输入三位以上搜索型号',
        missingMessage: '型号不能为空',
        panelMaxHeight: 300,
        mode: 'remote',
        novalidate: true,
        tipPosition: 'right',
        onChange: null,
        onLoadSuccess:null,
        onSelect: null,
        onDoChange: null,
        validType: 'isNull'
    });

    //标准型号名称补齐插件对外的方法
    $.fn.standardModel.methods = {
        //获取input框的值
        getVal: function (jq) {
            return $(jq).data('val');
        },
        //设置input的值
        setVal: function (jq, param) {
            var valueField = $(jq).data('options').valueField;
            var flag = false;
            saveVal(jq, param);
            getStandardModels(param, function (data) {
                if (data.length > 0) {
                    for (var i = 0; i < data.length; i++) {
                        if (data[i].Model == param) {
                            $(jq).combobox("loadData", data);
                            $(jq).combobox("select", data[i][valueField]);
                            setDom_result(jq, data[i]);
                            setDom_validateResult(jq, null);
                            flag = true;
                        }
                    }
                }
                if (flag == false) {
                    $(jq).combobox("setValue", param);
                    saveResult(jq, { "Model": param });
                    setDom_result(jq, { "Model": param });
                    var inputw;
                    if (!inputw) {
                        inputw = $(jq).next().find("input.validatebox-text").width();
                    }

                    verifyData(jq, inputw, function (data) {
                        saveValidateResult(jq, data);
                        setDom_validateResult(jq, data);
                    })
                }
            })
        },
        //获取标准型号名称补齐options
        options: function (jq) {
            return $(jq).data('options');
        },
        //获取某个标准型号名称补齐的数据
        getResult: function (jq) {
            return $(jq).data('result');
        },
        //获取插件携带的所有值
        getAllData: function (jq) {
            return {
                options: $(jq).data('options'),
                val: $(jq).data('val'),
                result: $(jq).data('result')
            };
        }
    };
    //将自定义的插件加入 easyui 的插件组
    $.parser.plugins.push('standardModel');
})(jQuery)