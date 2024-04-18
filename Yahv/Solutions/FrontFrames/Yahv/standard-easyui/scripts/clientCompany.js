//客户公司插件（可以输入可以选择）

//name              表示调用控件的dom的name属性
//有2个<input type="hidden">的input，以下它们的name值，这些值携带了响应的数据
//name              表示用户输入的客户公司名称
//name_result       表示某个客户公司的数据

(function ($) {
    var clientCompanyData = null;//存储客户公司数据
    var initsItem = [];//存储调用改组件的dom元素
    //接口地址
    var AjaxUrl = {
        getclientCompanyData: '/csrmapi/Clients',//获取客户公司数据(前20条)
        getclientCompanys: '/csrmapi/Clients/Search?name=',//通过客户公司名称搜索客户公司的数据
        getclientCompanyById: '/csrmapi/Clients/Getbyid?id='
    }
    function jltrim(data) {
        if (data && data.length > 0) {
            for (var i = 0; i < data.length; i++) {
                if (data[i].Name) {
                    data[i].Name = $.trim(data[i].Name);
                }
            }
        }
        return data;
    }
    //格式化下拉列表
    function formatItem(row) {
        var s = '<div>' + row.Name + '</div>';
        if (row.Vip != -1 && row.Vip != 0) {
            s = '<div><span class="vip' + row.Vip + '"></span>' + row.Name + '</div>';
        } else if (row.Vip == -1) {
            s = '<div><span class="vip"></span>' + row.Name + '</div>';
        }
        return s;
    }
    function getclientCompanys(Name, cb) {
        $.ajax({
            url: AjaxUrl.getclientCompanys + Name,
            dataType: 'jsonp',
            success: function (data) {
                if (data.Code == "200") {
                    if (data.Data && data.Data.length > 0) {
                        cb(jltrim(data.Data));
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
    function getclientCompanyById(id, cb) {
        $.ajax({
            url: AjaxUrl.getclientCompanyById + id,
            dataType: 'jsonp',
            success: function (data) {
                if (data.Code == "200") {
                    if (data.Data && data.Data.length > 0) {
                        cb(jltrim(data.Data));
                    }
                } else if (data.Code == "300") {
                    console.log("接口异常");
                }
            },
            error: function (err) {
                console.log(err);
            }
        });
    }

    //获取客户公司前20条数据
    function getDatas(cb) {
        $.ajax({
            type: "get",
            url: AjaxUrl.getclientCompanyData,
            dataType: "jsonp",
            success: function (data) {
                if (data.Code == "200") {
                    if (data.Data && data.Data.length > 0) {
                        clientCompanyData = jltrim(data.Data);
                    } else {
                        clientCompanyData = [];
                    }
                    cb(clientCompanyData);
                } else if (data.Code == "300") {
                    console.log("接口异常");
                }
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
    //保存input输入的value值
    function saveVal(sender, val) {
        $(sender).data("val", val)
    }
    //保存某个客户公司的数据
    function saveResult(sender, result) {
        $(sender).data("result", result)
    }

    //编写clientCompany插件
    $.fn.clientCompany = function (opt, param) {
        //如果options为string，则是方法调用
        if (typeof opt == 'string') {
            var method = $.fn.clientCompany.methods[opt];
            if (method) {
                return method(this, param);//如果有该方法就调用该方法
            } else {
                alert("该插件没有这个方法")
            }
        }

        var options = opt || {};
        var sender = this;
        var clientCompanyListData = null;//下拉客户公司数据

        //data-options,html配置
        (function () {
            var data_options = {};
            var s = $.trim(sender.attr("data-options"));
            if (s) {
                if (s.substring(0, 1) != "{") {
                    s = "{" + s + "}";
                }
                data_options = (new Function("return " + s))();
                options = $.extend(true, {}, $.fn.clientCompany.defaults, data_options, options);
            } else {
                options = $.extend(true, {}, $.fn.clientCompany.defaults, options);
            }
        })();

        $(sender).data("options", options);
        //添加隐藏的input存储选择结果
        var name = $(sender).attr("name");
        name = name + "_result";
        var valuer = $('<input type="hidden" name="' + name + '" />');
        sender.before(valuer);
        $(sender).data("clientCompany_result", valuer);

        //从远程获取数据
        var myloader = function (param, success, error) {
            //禁用验证
            $(sender).combobox("disableValidation");
            var q = param.q || '';
            if (q.length <= 1) { return false }
            $.ajax({
                url: AjaxUrl.getclientCompanys + q,
                dataType: 'jsonp',
                success: function (data) {
                    if (data.Code == "200") {
                        clientCompanyListData = jltrim(data.Data);
                        if (clientCompanyListData && clientCompanyListData.length > 0) {
                            success(jltrim(data.Data));
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

        //将该客户的性质和类型展示出来
        function setTargetVal(Nature, Type) {
            if (options.target) {
                $(options.target).text("性质：" + Nature + "，  类型：" + Type);
            }
        }

        //清空客户的性质和类型
        function setTargetValNull() {
            if (options.target) {
                $(options.target).text("");
            }
        }
        options.loader = debounce(myloader, 200);
        options.formatter = formatItem;
        options.validType = "OnlySelectDropValue['" + $(sender).attr("id") + "']";
        //input框数值改变
        options.onChange = function (n, o) {
            //判断如果输入框的值属于数据中的某一个则相当于选中下拉列表中的某一个
            saveVal(sender, n)
            if (n == "") {
                if (options.value && options.value != "" && clientCompanyData == null) {
                    getDatas(function () {
                        $(sender).combobox("loadData", clientCompanyData);
                    });
                } else {
                    $(sender).combobox("loadData", clientCompanyData);
                }
                setBorder(sender, "#D3D3D3");
                saveResult(sender, null);
                $($(sender).data("clientCompany_result")).val(null);
                setTargetValNull();
            } else if (n != "") {
                var data = $(sender).combobox('getData');
                if (data.length) {
                    for (var i = 0; i < data.length; i++) {
                        if (n == data[i].Name) {
                            $(sender).combobox("select", data[i].ID);
                            saveVal(sender, data[i].ID)
                            saveResult(sender, data[i]);
                            $(sender).combobox("hidePanel");
                            setTargetVal(data[i].NatureDes, data[i].AreaTypeDes);
                            break;
                        }
                    }
                }
            }
        };
        //在加载远程数据成功的时候触发
        options.onLoadSuccess = function () {
            //启用验证
            $(sender).combobox("enableValidation");
            var newVal = $(sender).combobox('getValue');
            if (newVal != "") {
                var data = $(sender).combobox('getData');
                if (data.length) {
                    for (var i = 0; i < data.length; i++) {
                        if (newVal == data[i].ID) {
                            saveResult(sender, data[i]);
                            setTargetVal(data[i].NatureDes, data[i].AreaTypeDes);
                            break;
                        } else if (newVal == data[i].Name) {
                            $(sender).combobox("select", data[i].ID);
                            saveVal(sender, data[i].ID)
                            saveResult(sender, data[i]);
                            $(sender).combobox("hidePanel");
                            setTargetVal(data[i].NatureDes, data[i].AreaTypeDes);
                            break;
                        } else {
                            setTargetValNull();
                            saveResult(sender, null);
                        }
                    }
                } else {
                    setTargetValNull();
                    saveResult(sender, null);
                }
            }
        };
        //选择下拉时
        options.onSelect = function (record) {
            saveVal(sender, record.Name);
            saveResult(sender, record);
            $($(sender).data("clientCompany_result")).val(JSON.stringify(record));
            setBorder(sender, "#D3D3D3");
            if (options.onDoChange) {
                options.onDoChange(record)
            }
            setTargetVal(record.NatureDes, record.AreaTypeDes);
        }
        $(sender).combobox(options);

        //如果有数据就将数据放入下拉列表中
        if (clientCompanyData) {
            $(sender).combobox("loadData", clientCompanyData);
        } else {
            //如果没有则把this放入数组中
            initsItem.push(sender);
        }

        //如果有value的话就不需要获取前20条数据了
        if (options.value && options.value != "") {
            $(sender).clientCompany('setVal', options.value);
        } else if (!options.value) {
            getDatas(function (clientCompanyData) {
                $(sender).combobox("loadData", clientCompanyData);
            });
        }
    }

    //客户公司插件的默认配置
    $.fn.clientCompany.defaults = $.extend({}, $.fn.combobox.defaults, {
        width: 220,
        target: null,
        value: null,
        valueField: 'ID',
        textField: 'Name',
        prompt: '请输入两位以上搜索客户公司',
        panelMaxHeight: 300,
        loader: null,
        mode: 'remote',
        required: true,
        missingMessage: '客户公司不能为空',
        novalidate: true,
        tipPosition: 'right',
        formatter:null,
        validType: null,
        onChange: null,
        onLoadSuccess: null,
        onSelect: null,
        onDoChange: null
    });

    //客户公司插件对外的方法
    $.fn.clientCompany.methods = {
        //获取input框的值
        getVal: function (jq) {
            return $(jq).data('val')
        },
        //设置input框的值
        setVal: function (jq, param) {
            var valueField = $(jq).data('options').valueField;
            getclientCompanys(param, function (data) {
                if (data.length > 0) {
                    for (var i = 0; i < data.length; i++) {
                        if (data[i].Name == param) {
                            $(jq).combobox("loadData", data);
                            $(jq).combobox("select", data[i][valueField]);
                        }
                    }
                }
            })
        },
        //根据id，设置客户公司的值
        setValbyID: function (jq, id) {
            var num = 0;
            var timer = setInterval(function () {
                num = $(jq).combobox("getData").length;
                if (num > 0) {
                    getclientCompanyById(id, function (data) {
                        if (data.ID) {
                            var data2 = [];
                            data2.push(data);
                            $(jq).combobox("loadData", data2);
                            $(jq).combobox("select", id);
                        }
                    })
                    clearInterval(timer);
                }
            }, 100)
        },
        //获取客户公司options
        options: function (jq) {
            return $(jq).combobox('options')
        },
        //获取某个客户公司的数据
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

    //将自定义的插件加入 easyui 的插件组
    $.parser.plugins.push('clientCompany');
})(jQuery)