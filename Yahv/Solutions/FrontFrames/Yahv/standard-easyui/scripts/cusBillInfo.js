//客户开票信息插件

//name              表示调用控件的dom的name属性
//有2个<input type="hidden">的input，以下它们的name值，这些值携带了响应的数据
//name              表示用户输入的客户开票信息名称
//name_result       表示某个客户开票信息的数据


(function ($) {
    $.extend($.fn.combobox.defaults.rules, {
        cusBillNull: {
            validator: function (value) {
                var flag = false;
                if (value == "客户开票信息为空") {
                    flag = false;
                } else {
                    flag = true;
                }
                return flag;
            },
            message: '客户开票信息不能为空'
        }
    });
    //接口地址
    var AjaxUrl = {
        getcusBillInfoData: '/csrmapi/Clients/Invoices?id='//获取客户开票信息数据
    };
    //addData
    function addData(data) {
        for (var i = 0; i < data.length; i++) {
            data[i].TypeAccountContactName = data[i].TypeDes + " " + " " + data[i].Account + " " + " " + data[i].Name;
        }
        return data;
    }
    function getcusBillInfoData(sender, id, flag, cb) {
        $.ajax({
            url: AjaxUrl.getcusBillInfoData + id,
            dataType: 'jsonp',
            success: function (data) {
                if (data.Code == "100") {
                    $(sender).combobox("setValue", "客户不存在");
                    setBorder(sender, "#fb4d4d");
                    setColor(sender);
                } else if (data.Code == "200") {
                    var data1 = addData(data.Data);
                    setBorder(sender, "#D3D3D3");
                    removeColor(sender);
                    if (data.Data.length == 0) {
                        //$(sender).combobox("setValue", "客户开票信息为空");
                        //setBorder(sender, "#fb4d4d");
                        //setColor(sender);
                    } else {
                        $(sender).combobox("setValue", null);
                        setBorder(sender, "#D3D3D3");
                        if (flag) {
                            removeColor(sender);
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
    //保存某个客户开票信息的数据
    function saveResult(sender, result) {
        $(sender).data("result", result);
        $(sender).data("selectResult").val(JSON.stringify(result));
    }
    //编写cusBillInfo插件
    $.fn.cusBillInfo = function (opt, param) {
        //如果options为string，则是方法调用
        if (typeof opt == 'string') {
            var method = $.fn.cusBillInfo.methods[opt];
            if (method) {
                return method(this, param);//如果有该方法就调用该方法
            } else {
                alert("该插件没有这个方法")
            }
        }

        var options = opt || {};
        var data_options = {};

        var sender = this;

        var s = $.trim(sender.attr("data-options"));
        if (s) {
            if (s.substring(0, 1) != "{") {
                s = "{" + s + "}";
            }
            data_options = (new Function("return " + s))();
            options = $.extend(true, {}, $.fn.cusBillInfo.defaults, data_options, options);
        } else {
            options = $.extend(true, {}, $.fn.cusBillInfo.defaults, options);
        }

        //添加隐藏的input来存储值
        var name = $(sender).attr("name");
        var valuer = $('<input type="hidden" name="' + name + '_result"/>');
        sender.before(valuer);
        $(sender).data("selectResult", valuer);

        //获取客户开票信息数据
        options.loader = function (param, success, error) {
            if (options.CustomerCompanyID != '') {
                getcusBillInfoData(sender, options.CustomerCompanyID, false, function (data) {
                    clientCompanyListData = data;
                    success(data);
                })
            }
        }
        //格式化下拉数据
        options.formatter = function (row) {
            var s = '<div class="formatList cus-billinfo">' +
                    //'<span><em>客户名称:</em>' + row.Enterprise + '</span>' +
                    '<span><em>发票类型:</em>' + row.TypeDes + '</span>' +
                    '<span><em>开户行:</em>' + row.Bank + '</span>' +
                    '<span><em>开户行地址:</em>' + row.BankAddress + '</span>' +
                    '<span><em>纳税人识别号:</em>' + row.TaxperNumber + '</span>' +
                    '<span><em>公司:</em>' + row.Enterprise.Name + '</span>' +
                    '<span><em>联系人姓名:</em>' + row.Name + '</span>' +
                    '<span><em>手机号:</em>' + row.Mobile + '</span>' +
                    '<span><em>电话:</em>' + row.Tel + '</span>' +
                    '<span><em>银行账号:</em>' + row.Account + '</span>' +
                    '<span><em>到货地址:</em>' + row.Address + '</span>' +
                    '<span><em>邮编:</em>' + row.Postzip + '</span>' +
                    '<span><em>到货地区:</em>' + row.DistrictDes + '</span></div> ';
            return s;
        }
        //选择下拉列表
        options.onSelect = function (record) {
            saveVal(sender, record.ID);
            saveResult(sender, record);
        }
        $(sender).combobox(options);
    }

    //标准客户开票信息名称补齐插件的默认配置
    $.fn.cusBillInfo.defaults = $.extend({}, $.fn.combobox.defaults, {
        width: 240,
        CustomerCompanyID: '',//配置客户公司ID
        valueField: 'ID',
        textField: 'TypeAccountContactName',
        prompt: '请选择客户开票信息',
        panelMinWidth: 420,//最小宽度
        panelMaxHeight: 320,
        mode: 'remote',
        loader: null,
        formatter: null,
        editable: false,
        required: false,
        //missingMessage: '客户开票信息不能为空',
        //validType: 'cusBillNull',
        novalidate: true,
        tipPosition: 'right',
        value: null,
        onSelect: null
    });
    //标准客户开票信息名称补齐插件对外的方法
    $.fn.cusBillInfo.methods = {
        //获取input框的值
        getVal: function (jq) {
            return $(jq).data('val')
        },
        //获取客户开票信息options
        options: function (jq) {
            return $(jq).combobox('options')
        },
        //获取某个客户开票信息的数据
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
            $(jq).data('result', null);
            var name = $(jq).next().find("input.validatebox-text").next().attr("name") + "_result";
            $("input[name='" + name + "']").val(null);
            getcusBillInfoData(jq, param.id, true, function (data) {
                $(jq).combobox("loadData", data);
                if (param.cb) {
                    param.cb(data);
                }
            })
        }
    };
    $.parser.plugins.push('cusBillInfo'); //将自定义的插件加入 easyui 的插件组    
})(jQuery)