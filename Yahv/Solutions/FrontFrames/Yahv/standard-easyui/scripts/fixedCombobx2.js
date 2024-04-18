/// <reference path="../../jquery-easyui-1.7.6/jquery.easyui.min.js" />
/// <reference path="../../jquery-easyui-1.7.6/jquery.min.js" />

//获取类型



(function ($) {

    $.extend($.fn.validatebox.defaults.rules, {
        fixedCombobx: {
            validator: function (value, param) {
                var sender = $('#' + param[0]);
                var data = sender.combobox('getData');
                if (data == null) {
                    return false;
                }
                var val = $.trim(sender.combobox('getValue'));
                var options = sender.data("options");

                var og = $.grep(data, function (item, index) {
                    return val == $.trim(item[options.valueField]);
                });

                return og.length > 0;
            },
            message: '必须选择已有项!'
        }
    });

    function ajaxData(cb, type) {
        $.ajax({
            async: false,
            type: "get",
            url: fixedCombobxUrls[type],
            dataType: "json",
            success: function (json) {
                if (cb) {
                    cb(json, type)
                }
            },
            error: function (err) {
                alert('error:' + JSON.stringify(err));
            }
        });
    }

    var fixedCombobxUrls = {
        ClientType: '/crmplus2api/Fixed/Enums?name=ClientType',
        VIPLevel: '/crmplus2api/Fixed/Enums?name=VIPLevel',
        ClientGrade: '/crmplus2api/Fixed/Enums?name=ClientGrade',
        AuditStatus: '/crmplus2api/Fixed/Enums?name=AuditStatus',
        Currency: '/crmplus2api/Fixed/Enums?name=Currency',
        OrderType: '/crmplus2api/Fixed/Enums?name=OrderType',
        Origin: "/crmplus2api/Fixed/Enums?name=Origin",
        SupplierType: "/crmplus2api/Fixed/Enums?name=SupplierType",
        EnterpriseNature: "/crmplus2api/Fixed/Enums?name=EnterpriseNature",
        BusinessRelationType: "/crmplus2api/Fixed/Enums?name=BusinessRelationType",
        RfqInvoice: "/PvDataApi/Enums/RqfInvoiceType",
        InvoiceType: '/crmplus2api/Fixed/Enums?name=InvoiceType',
        SupplierGrade: '/crmplus2api/Fixed/Enums?name=SupplierGrade',
        SettlementType: '/crmplus2api/Fixed/Enums?name=SettlementType',
        ClearType: '/crmplus2api/Fixed/Enums?name=ClearType',
        nBrandType: '/crmplus2api/Fixed/Enums?name=nBrandType',
        CommissionMethod: '/crmplus2api/Fixed/Enums?name=CommissionMethod',
        CommissionType: '/crmplus2api/Fixed/Enums?name=CommissionType',
        BookAccountMethord: '/crmplus2api/Fixed/Enums?name=BookAccountMethord',
        SampleType: '/crmplus2api/Fixed/Enums?name=SampleType',
        ReportStatus: '/crmplus2api/Fixed/Enums?name=ReportStatus',
        ProductStatus: '/crmplus2api/Fixed/Enums?name=ProductStatus',
        FollowWay: '/crmplus2api/Fixed/Enums?name=FollowWay',
        DataStatus: '/crmplus2api/Fixed/Enums?name=DataStatus',
        AddressType: '/crmplus2api/Fixed/Enums?name=AddressType',
        QuoteMethod: '/crmplus2api/Fixed/Enums?name=QuoteMethod',
        FreightPayer: '/crmplus2api/Fixed/Enums?name=FreightPayer',
        BookAccountType: '/crmplus2api/Fixed/Enums?name=BookAccountType',
        FixedArea: '/crmplus2api/Fixed/Dic?name=FixedArea',
        FixedSource: '/crmplus2api/Fixed/Dic?name=FixedSource',
        WareHouses: '/crmplus2api/WareHouses/All',
        WarehouseGrade: '/crmplus2api/Fixed/Enums?name=WarehouseGrade',
        SiteGrade: '/crmplus2api/Fixed/Enums?name=SiteGrade',
    };

    if (!top.$.baseData) {
        top.$.baseData = {};
    }
    if (!top.$.baseData.fixedCombobx) {
        top.$.baseData.fixedCombobx = {};
        for (var key in fixedCombobxUrls) {
            ajaxData(function (json, type) {
                var data = $.map(json, function (item, index) {
                    //if (item.ID == -32768) {
                    //    item.ID = 'a';
                    //}
                    return item;
                });
                data.unshift({ ID: 'a', Name: '全部', Code: 'a' })
                top.$.baseData.fixedCombobx[type] = data;
            }, key);
        }
    }


    //编写fixedCombobx插件
    $.fn.fixedCombobx = function (opt, param) {
        //如果options为string，则是方法调用
        if (typeof opt == 'string') {
            var method = $.fn.fixedCombobx.methods[opt];
            if (method) {
                return method(this, param);//如果有该方法就调用该方法
            } else {
                alert("fixedCombobx.该插件没有这个方法：" + opt)
            }
        }

        var options = opt || {};
        options = $.extend(true, {}, $.fn.fixedCombobx.defaults, options);

        return this.each(function () {
            var sender = $(this);

            var sender_id = sender.prop('id');
            if (!sender_id) {
                sender_id = 'client_fixedCombobx_' + Math.random().toString().substring(2);
                sender.prop('id', sender_id);
            }

            //保存设置类型
            sender.data('options', options);

            var data = top.$.baseData.fixedCombobx[options.type];
            if (typeof (data) == 'undefined' || !data) {
                alert('fixedCombobx.无法找到初始化的类型:' + options.type);
                return;
            }

            if (!options.isAll) {
                data = $.grep(data, function (item, index) {
                    if (item.ID == -32768 || item.ID == 'a') {
                        return false;
                    }
                    return true;
                });
            }

            sender.combobox({
                valueField: options.valueField,
                textField: options.textField,
                data: data,
                value: options.value != null && options.value !== '' ? options.value : data[0][options.valueField],
                validType: 'fixedCombobx["' + sender_id + '"]',
                required: options.required,
                editable: options.editable,
                validateOnCreate: true,
                onChange: options.onChange,
            });

        });
    }

    //fixedCombobx插件的默认配置
    $.fn.fixedCombobx.defaults = {
        valueField: 'ID',
        textField: 'Name',
        type: '',
        value: '',  //值
        required: true,
        editable: true,
        isAll: false,//是否包涵All选项,
        //onChange: null,
        // onChange: function (newValue, oldValue) {
        // } 
    };


    //fixedCombobx插件对外的方法myOnChange
    $.fn.fixedCombobx.methods = {
        //保留
        test: function (jq) {
            var sender = $(jq);
            return sender;
        },
        //设置单个值
        setValue: function (jq, value) {
            var sender = $(jq);
            sender.combobox('setValue', value);
            sender.combobox('isValid')

            return sender;
        },

        getValue: function (jq, value) {
            var sender = $(jq);
            return sender.combobox('getValue');
        },
        ////设置多个值
        //setValues: function (jq, arry) {
        //    var sender = $(jq);
        //    sender.combobox('setValues', arry);
        //    return sender;
        //},
    };

    //将自定义的插件加入 easyui 的插件组
    $.parser.plugins.push('fixedCombobx');
})(jQuery)