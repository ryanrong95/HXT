//客户联系人插件getcusContactData

//name              表示调用控件的dom的name属性
//有2个<input type="hidden">的input，以下它们的name值，这些值携带了响应的数据
//name              表示用户输入的客户联系人名称
//name_result       表示某个客户联系人的数据


(function ($) {
    //扩展验证方法
    $.extend($.fn.combobox.defaults.rules, {
        OnlySelectDrop: {
            validator: function (value, param) {
                var flag = false;
                if (value == "" || value == '客户联系人数据为空') {
                    flag = true;
                } else {
                    var data = $("#" + param[0]).combobox('getData');
                    if (data.length) {
                        for (var i = 0; i < data.length; i++) {
                            if (value == data[i].Name || value == data[i].Mobile || value == data[i].Tel) {
                                flag = true;
                                return flag;
                            } else {
                                flag = false;
                            }
                        }
                    }
                }
                return flag;
            },
            message: '只能选择下拉列表的数据'
        },
        cusContactNull: {
            validator: function (value) {
                if (value == "客户联系人数据为空") {
                    flag = false;
                } else {
                    flag = true;
                }
                return flag;
            },
            message: '客户联系人不能为空'
        }
    });

    //接口地址
    var AjaxUrl = {
        getcusContactData: '/csrmapi/Clients/Contacts?id=',//获取客户联系人数据
        searchUrl: '/csrmapi/Clients/SearchContacts?'
    };
    var cusContactData = null;//存储客户联系人数据
    var initsItem = [];//存储调用改组件的dom元素

    //获取客户联系人数据
    function getcusContactData(sender, id) {
        $.ajax({
            type: "get",
            url: AjaxUrl.getcusContactData + id,
            dataType: "jsonp",
            success: function (data) {
                if (data.Code == "100") {
                    $(sender).combobox("setValue", "客户不存在");
                    setBorder(sender, "#fb4d4d");
                    setColor(sender);
                } else if (data.Code == "200") {
                    cusContactData = data.Data;
                    for (var index = 0; index < initsItem.length; index++) {
                        $(initsItem[index]).combobox("loadData", cusContactData);
                    };
                    if (data.Data.length == 0) {
                        //$(sender).combobox("setValue", "客户联系人数据为空");
                        //setBorder(sender, "#fb4d4d");
                        //setColor(sender);
                        saveResult(sender, { "Name": "", "Mobile": "" })
                    } else {
                        $(sender).combobox("setValue", null);
                        removeColor(sender);
                        if (data.Data.length == 1) {
                            $(sender).combobox("select", data.Data[0].ID);
                        } else if (data.Data.length > 1) {
                            $(sender).combobox("showPanel");
                        }
                    }

                } else if (data.Code == "300") {
                    console.log("接口异常")
                }
            },
            error: function (err) {
                console.log(err);
            }
        });
    }

    function getcusContactData2(sender, id, cb) {
        $.ajax({
            type: "get",
            url: AjaxUrl.getcusContactData + id,
            dataType: "jsonp",
            success: function (data) {
                if (data.Code == "100") {
                    $(sender).combobox("setValue", "客户不存在");
                    setBorder(sender, "#fb4d4d");
                    setColor(sender);
                } else if (data.Code == "200") {
                    cusContactData = data.Data;
                    if (data.Data.length == 0) {
                        //$(sender).combobox("setValue", "客户联系人数据为空");
                        //setBorder(sender, "#fb4d4d");
                        //setColor(sender);
                        saveResult(sender, { "Name": "", "Mobile": "" })
                    } else {
                        $(sender).combobox("setValue", null);
                        setBorder(sender, "#D3D3D3");
                        removeColor(sender);
                        if (data.Data.length == 1) {
                            $(sender).combobox("select", data.Data[0].ID);
                        } else if (data.Data.length > 1) {
                            $(sender).combobox("showPanel");
                        }
                    }
                    cb(cusContactData);
                } else if (data.Code == "300") {
                    console.log("接口异常");
                }
            },
            error: function (err) {
                console.log(err);
            }
        });
    }

    //格式化下拉列表数据
    function formatItem(row) {
        var s = '<div class="formatList cus-contact">' +
                //'<span><em>客户名称:</em>' + row.Enterprise + '</span>' +
                '<span><em>联系人姓名:</em>' + row.Name + '</span>' +
                '<span><em>手机号:</em>' + row.Mobile + '</span>' +
                '<span><em>电话:</em>' + row.Tel + '</span></div>';
        return s;
    }
    //设置边框颜色
    function setBorder(ele, color) {
        $(ele).next().css("border-color", color);
    }
    //设置字体颜色
    function setColor(ele) {
        $(ele).next().find("input.textbox-text").addClass("red");
    }
    //移除字体颜色
    function removeColor(ele) {
        $(ele).next().find("input.textbox-text").removeClass("red");
    }

    //保存input输入的value值
    function saveVal(sender, val) {
        $(sender).data("val", val)
    }

    //保存某个客户联系人的数据
    function saveResult(sender, result) {
        $(sender).data("result", result)
    }
    //为隐藏的input设置结果值
    function setDomResult(sender, result) {
        $(sender).data("dom_result").val(JSON.stringify(result));
    }

    //编写cusContact插件
    $.fn.cusContact = function (opt, param) {
        //如果options为string，则是方法调用
        if (typeof opt == 'string') {
            var method = $.fn.cusContact.methods[opt];
            if (method) {
                return method(this, param);//如果有该方法就调用该方法
            } else {
                alert("该插件没有这个方法")
            }
        }

        var sender = this;
        var options = opt || {};

        //添加隐藏的input来存储值
        var name = $(sender).attr("name");
        var valuer = $('<input type="hidden" name="' + name + '_result"/>');
        sender.before(valuer);
        $(sender).data("dom_result", valuer);

        //html配置
        (function () {
            var data_options = {};
            var s = $.trim(sender.attr("data-options"));
            if (s) {
                if (s.substring(0, 1) != "{") {
                    s = "{" + s + "}";
                }
                data_options = (new Function("return " + s))();
                options = $.extend(true, {}, $.fn.cusContact.defaults, data_options, options);
            } else {
                options = $.extend(true, {}, $.fn.cusContact.defaults, options);
            }
        })();


        var selectedVal = null;//保存下拉选择的值
        //获取客户开票信息数据
        var myloader = function (param, success, error) {
            var q = param.q || '';
            if (q.length == 0) { return false };
            if (options.CustomerCompanyID != '') {
                $.ajax({
                    url: AjaxUrl.searchUrl + "id=" + options.CustomerCompanyID + "&name=" + q,
                    dataType: 'jsonp',
                    success: function (data) {
                        if (data.Code == "100") {
                            $(sender).combobox("setValue", "客户不存在");
                            setBorder(sender, "#fb4d4d");
                            setColor(sender);
                        } else if (data.Code == "200") {
                            success(data.Data)
                        } else if (data.Code == "300") {
                            console.log("接口异常")
                        }
                    },
                    error: function (data) {
                        error.apply(this, arguments);
                    }
                });
            }
        }
        options.loader = debounce(myloader, 200);
        options.formatItem = formatItem;
        //options.validType = ["cusContactNull", "OnlySelectDrop['" + $(sender).attr("id") + "']"];
        options.validType = ["OnlySelectDrop['" + $(sender).attr("id") + "']"];
        options.onChange = function (n, o) {
            //判断如果输入框的值属于数据中的某一个则相当于选中下拉列表中的某一个
            saveVal(sender, n)
            if (n == "") {
                $(sender).combobox("loadData", cusContactData);
                setBorder(sender, "#D3D3D3");
                saveResult(sender, { "Name": "", "Mobile": "" });
            }
        };
        options.onLoadSuccess = function () {
            var newVal = $(sender).combobox('getValue');
            if (newVal != "") {
                var data = $(sender).combobox('getData');
                if (data.length) {
                    for (var i = 0; i < data.length; i++) {
                        if (newVal == data[i].ID) {
                            saveResult(sender, data[i]);
                            break;
                        } else if (newVal == data[i].Name || newVal == data[i].Mobile || newVal == data[i].Tel) {
                            $(sender).combobox("select", data[i].ID);
                            saveVal(sender, data[i].ID)
                            saveResult(sender, data[i]);
                            setDomResult(sender, data[i]);
                            $(sender).combobox("hidePanel");
                            break;
                        } else {
                            saveResult(sender, null);
                        }
                    }
                } else {
                    saveResult(sender, null);
                }
            }
        };
        options.onSelect = function (record) {
            saveVal(sender, record.ID);
            saveResult(sender, record);
            setDomResult(sender, record);
            setBorder(sender, "#D3D3D3");
        };

        $(sender).combobox(options);

        if (options.CustomerCompanyID != '') {
            getcusContactData(sender, options.CustomerCompanyID);
            if (cusContactData) {
                $(sender).combobox("loadData", cusContactData);
            } else {
                initsItem.push(sender);
            }
        }
    }

    //标准客户联系人名称补齐插件的默认配置
    $.fn.cusContact.defaults = $.extend({}, $.fn.combobox.defaults, {
        value: null,
        width: 200,
        CustomerCompanyID: '',//配置客户公司ID
        valueField: 'ID',
        textField: 'Name',
        prompt: '请选择客户联系人',
        panelMinWidth: 400,//最小宽度
        panelHeight: 'auto',
        panelMaxHeight: 300,
        loader: null,
        mode: 'remote',
        formatter: null,
        required: false,
        missingMessage: '客户联系人不能为空',
        novalidate: true,
        tipPosition: 'right',
        validType: null,
        onChange: null,
        onLoadSuccess: null,
        onSelect: null
    });
    //标准客户联系人名称补齐插件对外的方法
    $.fn.cusContact.methods = {
        //获取input框的值
        getVal: function (jq) {
            return $(jq).data('val')
        },
        //获取客户联系人options
        options: function (jq) {
            return $(jq).combobox('options')
        },
        //获取某个客户联系人的数据
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
        },
        //通过id值获取数据
        loadDataByID: function (jq, param) {
            $(jq).combobox("setValue", null);
            $(jq).data('result', { Name: "", Mobile: "" });
            setDomResult(jq, { Name: "", Mobile: "" });
            getcusContactData2(jq, param.id, function (data) {
                $(jq).combobox("loadData", data);
                if (param.cb) {
                    param.cb(data);
                }
            })
        }
    };
    $.parser.plugins.push('cusContact'); //将自定义的插件加入 easyui 的插件组    
})(jQuery)