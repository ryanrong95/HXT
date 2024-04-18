//供应商插件（可以输入可以选择）

//name              表示调用控件的dom的name属性
//有2个<input type="hidden">的input，以下它们的name值，这些值携带了响应的数据
//name              表示用户输入的供应商名称
//name_result       表示某个供应商的数据

(function ($) {
    var supplierData = null;//存储供应商数据
    var initsItem = [];//存储调用改组件的dom元素
    //接口地址
    var AjaxUrl = {
        getsupplierData: '/csrmapi/Suppliers',//获取供应商数据(前20条)
        getsuppliers: '/csrmapi/Suppliers/Search?name='//通过供应商名称搜索供应商的数据
    }
    //格式化下拉列表
    function formatItem(row) {
        var s = '<div><span class="level' + row.Grade + '"></span>' + row.Name + '</div>'
        return s;
    }

    //获取供应商数据
    function getsupplierByName(Name, cb) {
        $.ajax({
            url: AjaxUrl.getsuppliers + Name,
            dataType: 'jsonp',
            success: function (data) {
                if (data.Code == "200") {
                    cb(data.Data)
                } else if (data.Code == "300") {
                    console.log("接口异常")
                }
            },
            error: function (err) {
                console.log(err);
            }
        });
    }

    //获取供应商前20条数据
    function getDatas(cb) {
        $.ajax({
            type: "get",
            url: AjaxUrl.getsupplierData,
            dataType: "jsonp",
            success: function (data) {
                if (data.Code == "200") {
                    supplierData = data.Data;
                    cb(supplierData);
                } else if (data.Code == "300") {
                    console.log("接口异常")
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
    //保存某个供应商的数据
    function saveResult(sender, result) {
        $(sender).data("result", result)
    }

    function beneficiaryChange(options, id) {
        $(options.target).supplierBeneficiary('loadDataByID', {
            id: id, cb: function (data) {
                if (data && data.length > 0) {
                    $(options.target).combobox("loadData", data);
                    if (data.length == 1) {
                        $(options.target).combobox("select", data[0].ID);
                        $(options.target).data('result', data[0]);
                        $($(options.target).data('supplierBeneficiary_result')).val(JSON.stringify(data[0]));
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

    //编写supplier插件
    $.fn.supplier = function (opt, param) {
        //如果options为string，则是方法调用
        if (typeof opt == 'string') {
            var method = $.fn.supplier.methods[opt];
            if (method) {
                return method(this, param);//如果有该方法就调用该方法
            } else {
                alert("该插件没有这个方法")
            }
        }

        var options = opt || {};
        var sender = this;
        var supplierListData = null;//下拉供应商数据

        //data-options,html配置
        var data_options = {};
        var s = $.trim(sender.attr("data-options"));
        if (s) {
            if (s.substring(0, 1) != "{") {
                s = "{" + s + "}";
            }
            data_options = (new Function("return " + s))();
            options = $.extend(true, {}, $.fn.supplier.defaults, data_options, options);
        } else {
            options = $.extend(true, {}, $.fn.supplier.defaults, options);
        }
        $(sender).data("options", options);
        //添加隐藏的input存储选择结果
        var name = $(sender).attr("name");
        name = name + "_result";
        var valuer = $('<input type="hidden" name="' + name + '" />');
        sender.before(valuer);
        $(sender).data('supplier_result', valuer);

        //从远程获取数据
        var myloader = function (param, success, error) {
            //禁用验证
            $(sender).combobox("disableValidation");
            var q = param.q || '';
            if (q.length <= 1) { return false }
            $.ajax({
                url: AjaxUrl.getsuppliers + q,
                dataType: 'jsonp',
                success: function (data) {
                    if (data.Code == "200") {
                        supplierListData = data.Data;
                        success(data.Data);
                    } else if (data.Code == "300") {
                        console.log("接口异常")
                    }
                },
                error: function () {
                    error.apply(this, arguments);
                }
            });
        }
        var saveTarget = options.target || null;//存储target
        var saveTargetName;
        var saveTargetDom;//存储targetDom元素
        if (saveTarget) {
            saveTargetDom = $(saveTarget);
            saveTargetName = $(saveTarget).attr("name");
        }
        options.formatter = formatItem;
        options.loader = debounce(myloader, 200);
        options.validType = "OnlySelectDropValue['" + $(sender).attr("id") + "']";
        //input框数值改变
        options.onChange = function (n, o) {
            //判断如果输入框的值属于数据中的某一个则相当于选中下拉列表中的某一个
            saveVal(sender, n)
            if (n == "") {
                if (options.value && options.value != "" && supplierData == null) {
                    getDatas(function () {
                        $(sender).combobox("loadData", supplierData);
                    });
                } else {
                    $(sender).combobox("loadData", supplierData);
                }
                setBorder(sender, "#D3D3D3");
                saveResult(sender, null);
                $($(sender).data('supplier_result')).val(null);
                if (options.target) {
                    $(options.target).combobox('clear');
                    $(options.target).combobox('loadData', []);
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
                            break;
                        } else if (newVal == data[i].Name) {
                            $(sender).combobox("select", data[i][options.valueField]);
                            saveVal(sender, data[i][options.valueField])
                            saveResult(sender, data[i]);
                            $(sender).combobox("hidePanel");
                            break;
                        } else {
                            saveResult(sender, null);
                        }
                    }
                }
            }
        };
        //选择下拉时触发
        options.onSelect = function (record) {
            saveVal(sender, record.Name);
            saveResult(sender, record);
            $($(sender).data('supplier_result')).val(JSON.stringify(record));
            setBorder(sender, "#D3D3D3");
            if (options.onDoSelect) {
                options.onDoSelect(record);
            }
            if (options.target) {
                beneficiaryChange(options, record.ID);
            }
        };

        $(sender).combobox(options);

        //如果有数据就将数据放入下拉列表中
        if (supplierData) {
            $(sender).combobox("loadData", supplierData);
        } else {
            //如果没有则把this放入数组中
            initsItem.push(sender);
        }

        if (options.value && options.value != "") {
            $(sender).supplier('setVal', options.value);
        } else if (!options.value) {
            getDatas(function (supplierData) {
                $(sender).combobox("loadData", supplierData);
            });
        }
    }

    //供应商插件的默认配置
    $.fn.supplier.defaults = $.extend({}, $.fn.combobox.defaults, {
        value: null,
        width: 210,
        valueField: 'ID',
        textField: 'Name',
        prompt: '请输入两位以上搜索供应商',
        formatter: null,
        panelMaxHeight: 300,
        loader: null,
        mode: 'remote',
        required: true,
        missingMessage: '供应商不能为空',
        novalidate: true,
        tipPosition: 'right',
        validType: null,
        onChange: null,
        onLoadSuccess: null,
        onSelect: null,
        onDoSelect: null
    });

    //供应商插件对外的方法
    $.fn.supplier.methods = {
        //获取input框的值
        getVal: function (jq) {
            return $(jq).data('val');
        },
        //设置input框的值
        setVal: function (jq, param) {
            var valueField = $(jq).data('options').valueField;
            getsupplierByName(param, function (data) {
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
        //获取供应商options
        options: function (jq) {
            return $(jq).combobox('options');
        },
        //获取某个供应商的数据
        getResult: function (jq) {
            return $(jq).data('result');
        },
        //获取插件携带的所有值
        getAllData: function (jq) {
            return {
                options: $(jq).combobox('options'),
                val: $(jq).data('val'),
                result: $(jq).data('result')
            };
        },
        //获取供应商受益人数据
        getSupplierBeneficiary: function (jq) {
            var alldata;
            if ($(jq).data('options').target && $(jq).data('result').IsFactory) {
                alldata = $($(jq).data('options').target).supplierBeneficiary("getAllData");
            }
            return alldata;
        }
    };

    //将自定义的插件加入 easyui 的插件组
    $.parser.plugins.push('supplier');
})(jQuery)