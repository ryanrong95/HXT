//代仓储客户（可以输入可以选择）

//name              表示调用控件的dom的name属性
//有2个<input type="hidden">的input，以下它们的name值，这些值携带了响应的数据
//name              表示用户输入的企业名称
//name_result       表示某个企业的数据

(function ($) {
    var epData = null;    //存储数据

    //接口地址
    var ajaxUrl = {
        getEnterpriseData: '/Finance/BasicInfo/Companies/Infos.aspx?action=Index',//获取企业数据(前20条)
        getEnterprises: '/Finance/BasicInfo/Companies/Infos.aspx?action=Search&name=',//通过企业名称搜索企业的数据
        getEnterpriseById: '/Finance/BasicInfo/Companies/Infos.aspx?action=Getbyid&id='
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

    //根据输入内容搜索
    function getEnterprises(name, cb) {
        $.ajax({
            url: ajaxUrl.getEnterprises + name,
            dataType: 'jsonp',
            success: function (data) {
                if (data.Code == "200") {
                    if (data.Data && data.Data.length > 0) {
                        cb(jltrim(data.Data));
                    } else {
                        epData = [];
                        cb(epData);
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

    //获取企业前20条数据
    function getDatas(cb) {
        $.ajax({
            type: "get",
            url: ajaxUrl.getEnterpriseData,
            dataType: "jsonp",
            success: function (data) {
                if (data.Code == "200") {
                    if (data.Data && data.Data.length > 0) {
                        epData = jltrim(data.Data);
                    } else {
                        epData = [];
                    }
                    cb(epData);
                } else if (data.Code == "300") {
                    console.log("接口异常");
                }
            },
            error: function (err) {
                console.log(err);
            }
        });
    }

    function getEnterpriseById(id, cb) {
        $.ajax({
            url: ajaxUrl.getEnterpriseById + id,
            dataType: 'jsonp',
            success: function (data) {
                if (data.Code == "200") {
                    if (data.Data || data.Data.length > 0) {
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

    //编写Enterprise插件
    $.fn.Enterprise = function (opt, param) {
        //如果options为string，则是方法调用
        if (typeof opt == 'string') {
            var method = $.fn.Enterprise.methods[opt];
            if (method) {
                return method(this, param);//如果有该方法就调用该方法
            } else {
                alert("该插件没有这个方法");
            }
        }

        var options = opt || {};
        var sender = this;
        var epData = null;//下拉企业数据

        //data-options,html配置
        (function () {
            var data_options = {};
            var s = $.trim(sender.attr("data-options"));
            if (s) {
                if (s.substring(0, 1) != "{") {
                    s = "{" + s + "}";
                }
                data_options = (new Function("return " + s))();
                options = $.extend(true, {}, $.fn.Enterprise.defaults, data_options, options);
            } else {
                options = $.extend(true, {}, $.fn.Enterprise.defaults, options);
            }
        })();

        $(sender).data("options", options);

        //从远程获取数据
        var myloader = function (param, success, error) {
            //禁用验证
            //$(sender).combobox("disableValidation");

            var q = param.q || '';
            if (q.length <= 1) {
                return false;
            }
            $.ajax({
                url: ajaxUrl.getEnterprises + q,
                dataType: 'jsonp',
                success: function (data) {
                    if (data.Code == "200") {
                        epData = jltrim(data.Data);
                        if (epData && epData.length > 0) {
                            success(jltrim(data.Data));
                        } else {
                            epData = [];
                            success(epData);
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

        options.loader = debounce(myloader, 200);

        //input框数值改变
        options.onChange = function (n, o) {
            if (n == "") {
                if (options.value && options.value != "" && epData == null) {
                    getDatas(function () {
                        $(sender).combobox("loadData", epData);
                    });
                } else {
                    $(sender).combobox("loadData", epData);
                }
            } else if (n != "") {
                var data = $(sender).combobox('getData');
                var valueField = $(sender).data('options').valueField;
                if (data.length == 1) {
                    $(sender).combobox("select", data[0][valueField]);
                }
            }
        };

        //在加载远程数据成功的时候触发
        options.onLoadSuccess = function () {
            //启用验证
            $(sender).combobox("enableValidation");

            var valueField = $(sender).data('options').valueField;
            var textField = $(sender).data('options').textField;
            var newVal = $(sender).combobox('getValue');

            if (newVal != "") {
                var data = $(sender).combobox('getData');
                if (data.length) {
                    for (var i = 0; i < data.length; i++) {
                        if (newVal == data[i].ID) {
                            break;
                        } else if (newVal == data[i].Name) {
                            $(sender).combobox("select", data[i][valueField]);
                            break;
                        }
                    }
                }
            }
        };

        //选择下拉时
        options.onSelect = function (record) {
            if (options.onDoChange) {
                options.onDoChange(record);
            }
        }

        $(sender).combobox(options);

        //如果有数据就将数据放入下拉列表中
        if (epData) {
            $(sender).combobox("loadData", epData);
        }

        //如果有value的话就不需要获取前20条数据了
        if (options.value && options.value != "") {
            $(sender).Enterprise('setVal', options.value);
        } else if (!options.value) {
            getDatas(function (epData) {
                $(sender).combobox("loadData", epData);
            });
        }
    }

    //企业插件的默认配置
    $.fn.Enterprise.defaults = $.extend({}, $.fn.combobox.defaults, {
        width: 220,
        target: null,
        value: null,
        valueField: 'ID',
        textField: 'Name',
        prompt: '请输入企业',
        panelMaxHeight: 300,
        loader: null,
        mode: 'remote',
        required: true,
        missingMessage: '企业不能为空',
        novalidate: true,
        tipPosition: 'right',
        formatter: null,
        validType: null,
        onChange: null,
        onLoadSuccess: null,
        onSelect: null,
        onDoChange: null
    });

    //企业插件对外的方法
    $.fn.Enterprise.methods = {
        //获取input框的值
        getVal: function (jq) {
            return $(jq).data('val');
        },
        //设置input框的值
        setVal: function (jq, param) {
            var valueField = $(jq).data('options').valueField;
            getEnterprises(param,
                function (data) {
                    if (data.length > 0) {
                        for (var i = 0; i < data.length; i++) {
                            if (data[i].Name == param) {
                                $(jq).combobox("loadData", data);
                                $(jq).combobox("select", data[i][valueField]);
                            }
                        }
                    }
                });
        },
        //根据id，设置企业的值
        setValbyID: function (jq, id) {
            var num = 0;
            var timer = setInterval(function () {
                num = $(jq).combobox("getData").length;
                if (num > 0) {
                    getEnterpriseById(id,
                        function (data) {
                            if (data.ID) {
                                var data2 = [];
                                data2.push(data);
                                $(jq).combobox("loadData", data2);
                                var valueField = $(jq).data('options').valueField;
                                if (valueField == 'ID') {
                                    $(jq).combobox("select", data.ID);
                                } else if (valueField == "Name") {
                                    $(jq).combobox("select", data.Name);
                                }

                            }
                        });
                    clearInterval(timer);
                }
            },
                100);
        },
        //获取企业options
        options: function (jq) {
            return $(jq).combobox('options');
        },
        //获取某个企业的数据
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
    $.parser.plugins.push('Enterprise');
})(jQuery)