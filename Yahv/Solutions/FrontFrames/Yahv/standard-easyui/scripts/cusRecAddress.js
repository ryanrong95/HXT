//客户收货地址插件

//name              表示调用控件的dom的name属性
//有2个<input type="hidden">的input，以下它们的name值，这些值携带了响应的数据
//name              表示用户输入的客户收货地址名称
//name_result       表示某个客户收货地址的数据


(function ($) {
    $.extend($.fn.combobox.defaults.rules, {
        cusAddressNull: {
            validator: function (value) {
                if (value == "客户收货地址数据为空") {
                    flag = false;
                } else {
                    flag = true;
                }
                return flag;
            },
            message: '客户收货地址不能为空'
        }
    });
    //接口地址
    var AjaxUrl = {
        getcusRecAddressData: '/csrmapi/Clients/Consignees?id='//获取客户收货地址数据
    };
    //格式化下拉列表
    function formatItem(row) {
        var s = '<div class="formatList cus-address">' +
                //'<span><em>客户名称:</em>' + row.Enterprise + '</span>' +
                '<span><em>收货地址:</em>' + row.Address + '</span>' +
                //'<span><em>收货地址ID:</em>' + row.ID + '</span>' +
                '<span><em>收货所在地:</em>' + row.DistrictDes + '</span>' +
                '<span><em>联系人姓名:</em>' + row.Name + '</span>' +
                '<span><em>手机号:</em>' + row.Mobile + '</span>' +
                '<span><em>电话:</em>' + row.Tel + '</span></div> ';
        return s;
    }
    function getcusRecAddressData(sender, id, flag, cb) {
        $.ajax({
            url: AjaxUrl.getcusRecAddressData + id,
            dataType: 'jsonp',
            success: function (data) {
                if (data.Code == "100") {
                    $(sender).combobox("setValue", "客户不存在");
                    setBorder(sender, "#fb4d4d");
                    setColor(sender);
                } else if (data.Code == "200") {
                    var data1 = data.Data;
                    if (data.Data.length == 0) {
                        $(sender).combobox("setValue", "客户收货地址数据为空");
                        setBorder(sender, "#fb4d4d");
                        setColor(sender);
                    } else {
                        $(sender).combobox("setValue", null);
                        setBorder(sender, "#D3D3D3");
                        if (flag) {
                            removeColor(sender, "#000000");
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
    //保存某个客户收货地址的数据
    function saveResult(sender, result) {
        $(sender).data("result", result)
    }
    //编写cusRecAddress插件
    $.fn.cusRecAddress = function (opt, param) {
        //如果options为string，则是方法调用
        if (typeof opt == 'string') {
            var method = $.fn.cusRecAddress.methods[opt];
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
                options = $.extend(true, {}, $.fn.cusRecAddress.defaults, data_options, options);
            } else {
                options = $.extend(true, {}, $.fn.cusRecAddress.defaults, options);
            }
        })();

        //添加隐藏的input来存储值
        var name = $(sender).attr("name");
        var valuer = $('<input type="hidden" name="' + name + '_result"/>');
        sender.before(valuer);

        //获取客户收货地址数据
        var myloader = function (param, success, error) {
            if (options.CustomerCompanyID != '') {
                getcusRecAddressData(sender, options.CustomerCompanyID, false, function (data) {
                    clientCompanyListData = data;
                    success(clientCompanyListData);
                })
            }
        }
        options.loader = myloader;
        options.formatter = formatItem;
        options.validType = 'cusAddressNull';
        options.onSelect = function (record) {
            saveVal(sender, record.ID);
            saveResult(sender, record);
            var name = $(sender).next().find("input.validatebox-text").next().attr("name") + "_result";
            $("input[name='" + name + "']").val(JSON.stringify(record));
        };

        $(sender).combobox(options);
    }

    //标准客户收货地址名称补齐插件的默认配置
    $.fn.cusRecAddress.defaults = $.extend({}, $.fn.combobox.defaults, {
        value: null,
        width: 200,
        CustomerCompanyID: '',//配置客户公司ID
        valueField: 'ID',
        textField: 'Address',
        prompt: '请选择客户收货地址',
        panelMinWidth: 400,//最小宽度
        panelMaxHeight: 300,
        mode: 'remote',
        loader: null,
        formatter: null,
        editable: false,
        required: true,
        missingMessage: '客户收货地址不能为空',
        validType: null,
        novalidate: true,
        tipPosition: 'right',
        onSelect: null//选择下拉时
    });
    //标准客户收货地址名称补齐插件对外的方法
    $.fn.cusRecAddress.methods = {
        //获取input框的值
        getVal: function (jq) {
            return $(jq).data('val')
        },
        //获取客户收货地址options
        options: function (jq) {
            return $(jq).combobox('options')
        },
        //获取某个客户收货地址的数据
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
            getcusRecAddressData(jq, param.id, true, function (data) {
                $(jq).combobox("loadData", data);
                if (param.cb) {
                    param.cb(data);
                }
            })
        }
    };
    $.parser.plugins.push('cusRecAddress'); //将自定义的插件加入 easyui 的插件组    
})(jQuery)