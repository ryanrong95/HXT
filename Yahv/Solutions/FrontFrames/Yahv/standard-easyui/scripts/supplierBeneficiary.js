//供应商受益人插件

//name              表示调用控件的dom的name属性
//有2个<input type="hidden">的input，以下它们的name值，这些值携带了响应的数据
//name              表示用户输入的供应商受益人名称
//name_result       表示某个供应商受益人的数据


(function ($) {
    $.extend($.fn.combobox.defaults.rules, {
        supplierBeneficiaryNull: {
            validator: function (value) {
                if (value == "供货商受益人数据为空" || value == "供货商不存在") {
                    flag = false;
                } else {
                    flag = true;
                }
                return flag;
            },
            message: '供货商受益人数据不能为空'
        }
    });
    //接口地址
    var AjaxUrl = {
        getsupplierBeneficiaryData: '/csrmapi/Suppliers/Benneficiaries?id='//获取供应商受益人数据
    };
    //addData
    function addData(data) {
        for (var i = 0; i < data.length; i++) {
            data[i].BankAccount = data[i].Bank + " " + " " + data[i].Account;
        }
        return data;
    }

    function getsupplierBeneficiaryData(sender, id, flag, cb) {
        $.ajax({
            url: AjaxUrl.getsupplierBeneficiaryData + id,
            dataType: 'jsonp',
            success: function (data) {
                if (data.Code == "100") {
                    $(sender).combobox("setValue", "供货商不存在");
                    setBorder(sender, "#fb4d4d");
                    setColor(sender);
                } else if (data.Code == "200") {
                    var data1 = addData(data.Data);
                    if (data.Data.length == 0) {
                        //$(sender).combobox("setValue", "供货商受益人数据为空");
                        //setBorder(sender, "#fb4d4d");
                        //setColor(sender);
                    } else {
                        $(sender).combobox("setValue", null);
                        setBorder(sender, "#D3D3D3");
                        if (flag) {
                            removeColor(sender, "#aaaaaa");
                        }
                        if (data.Data.length == 1) {
                            $(sender).combobox("select", data.Data[0].ID);
                        } else if (data.Data.length > 1) {
                            $(sender).combobox("showPanel");
                        }

                    }
                    cb(data1);
                } else if (data.Code == "300") {
                    console.log("接口异常");
                }
            },
            error: function (data) {
                error.apply(this, arguments);
            }
        });
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
    //保存某个供应商受益人的数据
    function saveResult(sender, result) {
        $(sender).data("result", result)
    }
    //编写supplierBeneficiary插件
    $.fn.supplierBeneficiary = function (opt, param) {
        //如果options为string，则是方法调用
        if (typeof opt == 'string') {
            var method = $.fn.supplierBeneficiary.methods[opt];
            if (method) {
                return method(this, param);//如果有该方法就调用该方法
            } else {
                alert("该插件没有这个方法")
            }
        }

        var options = opt || {};
        var sender = this;
        var name = $(sender).attr("name") + "_result";
        var valuer = $('<input type="hidden" name="' + name + '"/>');
        sender.before(valuer);
        $(sender).data('supplierBeneficiary_result', valuer);
        //data-options,html配置
        var data_options = {};
        var s = $.trim(sender.attr("data-options"));
        if (s) {
            if (s.substring(0, 1) != "{") {
                s = "{" + s + "}";
            }
            data_options = (new Function("return " + s))();
            options = $.extend(true, {}, $.fn.supplierBeneficiary.defaults, data_options, options);
        } else {
            options = $.extend(true, {}, $.fn.supplierBeneficiary.defaults, options);
        }
        $(sender).data("options", options);
        var supplierBeneficiaryData = [];

        //获取供应商受益人数据
        options.loader = function (param, success, error) {
            if (options.supplierID && options.supplierID != "") {
                getsupplierBeneficiaryData(sender, options.supplierID, false, function (data) {
                    success(data)
                })
            }
        };
        //格式化下拉列表数据
        options.formatter = function (row) {
            var s = '<div class="formatList supplier-beneficiary">' +
                    '<span><em>实际名称:</em>' + row.RealName + '</span>' +
                    '<span><em>开户行:</em>' + row.Bank + '</span>' +
                    '<span><em>开户行地址:</em>' + row.BankAddress + '</span>' +
                    '<span><em>银行账号:</em>' + row.Account + '</span>' +
                    '<span><em>SwiftCode:</em>' + row.SwiftCode + '</span>' +
                    '<span><em>支付方式:</em>' + row.MethordDes + '</span>' +
                    '<span><em>币种:</em>' + row.CurrencyDes + '</span>' +
                    '<span><em>所在地:</em>' + row.DistrictDes + '</span>' +
                    '<span><em>联系人姓名:</em>' + row.Name + '</span>' +
                    '<span><em>联系人电话:</em>' + row.Tel + '</span>' +
                    '<span><em>联系人手机号:</em>' + row.Mobile + '</span>' +
                    '<span><em>联系人邮箱:</em>' + row.Email + '</span>' +
                    '</div> ';
            return s;
        };
        //下拉选择
        options.onSelect = function (record) {
            saveVal(sender, record.ID);
            saveResult(sender, record);
            $($(sender).data('supplierBeneficiary_result')).val(JSON.stringify(record));
        };
        $(sender).data("options", options);
        $(sender).combobox(options);
    }

    //标准供应商受益人名称补齐插件的默认配置
    $.fn.supplierBeneficiary.defaults = $.extend({}, $.fn.combobox.defaults, {
        width: 200,
        supplierID: '',//配置供应商ID
        valueField: 'ID',
        textField: 'BankAccount',
        //prompt: '请选择供应商受益人',
        panelMinWidth: 400,//最小宽度
        panelMaxHeight: 300,
        mode: 'remote',
        loader: null,
        formatter: null,
        editable: false,
        required: false,
        //missingMessage: '供应商受益人不能为空',
        //validType: "supplierBeneficiaryNull",
        novalidate: true,
        tipPosition: 'right',
        value: null
    });

    //标准供应商受益人名称补齐插件对外的方法
    $.fn.supplierBeneficiary.methods = {
        //获取input框的值
        destroy: function (jq) {
            return $(jq).combobox('destroy');
        },
        clear: function (jq) {
            return $(jq).combobox('clear');
        },
        //获取input框的值
        getVal: function (jq) {
            return $(jq).data('val')
        },
        //setVal: function (jq, param) {
        //    var valueField = $(jq).data('options').valueField;
        //    if ($(jq).data("options").supplierID && $(jq).data("options").supplierID!="") {
        //        $(jq).loadDataByID({
        //            id: $(jq).data("options").supplierID, cb: function () {
        //                var data = $(jq).combobox("getData");
        //                for (var i = 0; i < data.length; i++) {
        //                    if (data[i].ID == param) {
        //                        $(jq).combobox("loadData", data);
        //                        $(jq).combobox("select", data[i][valueField]);
        //                        $(jq).data('result', data[i]);
        //                        var name = $(jq).next().find("input.validatebox-text").next().attr("name") + "_result";
        //                        $("input[name='" + name + "']").val(data[i]);
        //                    }
        //                }
        //            }
        //        })
        //    }
        //},
        //获取供应商受益人options
        options: function (jq) {
            return $(jq).data('options');
        },
        //获取某个供应商受益人的数据
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
        },
        loadDataByID: function (jq, param) {
            $(jq).combobox("setValue", null);
            $(jq).data('result', null);
            var name = $(jq).next().find("input.validatebox-text").next().attr("name") + "_result";
            $("input[name='" + name + "']").val(null);
            getsupplierBeneficiaryData(jq, param.id, true, function (data) {
                $(jq).combobox("loadData", data);
                if (param.cb) {
                    param.cb();
                }
            })
        }
    };
    $.parser.plugins.push('supplierBeneficiary'); //将自定义的插件加入 easyui 的插件组    
})(jQuery)