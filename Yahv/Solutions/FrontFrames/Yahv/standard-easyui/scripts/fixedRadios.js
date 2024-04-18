/// <reference path="../../jquery-easyui-1.7.6/jquery.easyui.min.js" />
/// <reference path="../../jquery-easyui-1.7.6/jquery.min.js" />



//获取类型

var fixedRadios = {
    RqfBussiness: 'RqfBussiness',
    Range: 'Range',
    RqfInvoice: 'RqfInvoice',
    QuantityRemark: 'QuantityRemark',
    TradeType: 'TradeType',
    AreaType: 'AreaType',
    SealType: 'SealType',
    WareHouses: 'WareHouses',
    PurchaseType: 'PurchaseType',
    PurchaseNature: 'PurchaseNature'
};

(function ($) {

    function getRqfBussinessType(cb) {
        $.ajax({
            async: false,
            type: "get",
            url: '/PvDataApi/Enums/RqfBussinessType',
            dataType: "json",
            success: function (json) {
                if (cb) {
                    cb(json)
                }
            },
            error: function (err) {
                alert('error:' + JSON.stringify(err));
            }
        });
    }

    function getRange(cb) {
        $.ajax({
            async: false,
            type: "get",
            url: '/PvDataApi/Enums/Range',
            dataType: "json",
            success: function (json) {
                if (cb) {
                    cb(json)
                }
            },
            error: function (err) {
                alert('error:' + JSON.stringify(err));
            }
        });
    }

    function getRqfInvoice(cb) {
        $.ajax({
            async: false,
            type: "get",
            url: '/PvDataApi/Enums/RqfInvoiceType',
            dataType: "json",
            success: function (json) {
                if (cb) {
                    cb(json)
                }
            },
            error: function (err) {
                alert('error:' + JSON.stringify(err));
            }
        });
    }

    function getQuantityRemark(cb) {
        $.ajax({
            async: false,
            type: "get",
            url: '/PvDataApi/Enums/QuantityRemark',
            dataType: "json",
            success: function (json) {
                if (cb) {
                    cb(json)
                }
            },
            error: function (err) {
                alert('error:' + JSON.stringify(err));
            }
        });
    }

    function getTradeType(cb) {
        $.ajax({
            async: false,
            type: "get",
            url: '/PvDataApi/Enums/TradeType',
            dataType: "json",
            success: function (json) {
                if (cb) {
                    cb(json)
                }
            },
            error: function (err) {
                alert('error:' + JSON.stringify(err));
            }
        });
    }

    function getAreaType(cb) {
        $.ajax({
            async: false,
            type: "get",
            url: '/PvDataApi/Enums/AreaType',
            dataType: "json",
            success: function (json) {
                if (cb) {
                    cb(json)
                }
            },
            error: function (err) {
                alert('error:' + JSON.stringify(err));
            }
        });
    }

    function getSealType(cb) {
        $.ajax({
            async: false,
            type: "get",
            url: '/PvDataApi/Enums/SealType',
            dataType: "json",
            success: function (json) {
                if (cb) {
                    cb(json)
                }
            },
            error: function (err) {
                alert('error:' + JSON.stringify(err));
            }
        });
    }

    function gets(cb, url) {
        $.ajax({
            async: false,
            type: "get",
            url: url,
            dataType: "json",
            success: function (json) {
                if (cb) {
                    cb(json)
                }
            },
            error: function (err) {
                alert('error:' + JSON.stringify(err));
            }
        });
    }

    if (!top.$.baseData) {
        top.$.baseData = {};
    }

    //业务类型
    if (!top.$.baseData.RqfBussiness) {
        getRqfBussinessType(function (json) {
            top.$.baseData.RqfBussiness = json;
        });
    }
    //所在地区
    if (!top.$.baseData.Range) {
        getRange(function (json) {
            top.$.baseData.Range = json;
        });
    }
    //发票类型
    if (!top.$.baseData.RqfInvoice) {
        getRqfInvoice(function (json) {
            top.$.baseData.RqfInvoice = json;
        });
    }
    //用量说明
    if (!top.$.baseData.QuantityRemark) {
        getQuantityRemark(function (json) {
            top.$.baseData.QuantityRemark = json;
        });
    }
    //货期说明
    if (!top.$.baseData.TradeType) {
        getTradeType(function (json) {
            top.$.baseData.TradeType = json;
        });
    }
    //地区说明
    if (!top.$.baseData.AreaType) {
        getAreaType(function (json) {
            top.$.baseData.AreaType = json;
        });
    }
    //盖章说明
    if (!top.$.baseData.SealType) {
        getSealType(function (json) {
            top.$.baseData.SealType = json;
        });
    }
    //库房
    if (!top.$.baseData.WareHouses) {
        gets(function (json) {
            top.$.baseData.WareHouses = json.Data;
        }, '/csrmapi/WareHouses/');
    }
    //采购单类型
    if (!top.$.baseData.PurchaseType) {
        top.$.baseData.PurchaseType = [{ "ID": 1, Name: "市场采购" }, { "ID": 2, Name: "委托采购" }];
    }

    //货期说明
    if (!top.$.baseData.PurchaseNature) {
        top.$.baseData.PurchaseNature = [{ "ID": 1, Name: "即时采购" }, { "ID": 2, Name: "被动备货" }];
    }



    function initMethord(sender, options, data) {
        sender.hide();
        var formName = sender.prop('name');
        if (!formName) {
            alert('initRqfBussinessType.控件name不能为空！2');
        }
        sender.prop('name', '');
        sender.attr('radiosname', formName);

        for (var index = data.length - 1; index >= 0; index--) {
            var item = data[index];
            var input = $('<input name="' + formName + '" />');
            //var input = $('<input />');
            if (options.split && index != 0) {
                sender.after(input).after(options.split);
            } else {
                sender.after(input);
            }
            input.radiobutton({
                labelWidth: options.labelWidth,
                label: item['Name'],
                labelPosition: 'after',
                value: item['ID'],
                checked: options.value == item['ID'] || parseInt(options.value) == index,
                onChange: options.onChange
            });
        }
    }

    //编写fixedRadios插件
    $.fn.fixedRadios = function (opt, param) {
        //如果options为string，则是方法调用
        if (typeof opt == 'string') {
            var method = $.fn.fixedRadios.methods[opt];
            if (method) {
                return method(this, param);//如果有该方法就调用该方法
            } else {
                alert("该插件没有这个方法：" + opt)
            }
        }

        var options = opt || {};
        options = $.extend(true, {}, $.fn.fixedRadios.defaults, options);

        return this.each(function () {
            var sender = $(this);

            //保存设置类型
            sender.data('options', options);

            var data = top.$.baseData[options.type];
            if (typeof (data) == 'undefined' || !data) {
                alert('无法找到初始化的类型:' + options.type);
                return;
            }
            initMethord(sender, options, data);
            //结束
        });
    }

    //fixedRadios插件的默认配置
    $.fn.fixedRadios.defaults = {
        type: '',
        value: '',  //值
        labelWidth: 80,// 标签宽度
        onChange: function (checked) {

        },
        split: ''
    };


    //fixedRadios插件对外的方法
    $.fn.fixedRadios.methods = {
        //保留
        test: function (jq) {
            var sender = $(jq);
            return sender;
        },
        //设置值
        setValue: function (jq, value) {
            var sender = $(jq);
            var name = sender.attr('radiosname');
            var inputs = sender.parent().find('input[radiobuttonname="' + name + '"]');
            var selected = null;
            inputs.each(function () {
                if ($(this).val() == value) {
                    selected = $(this);
                }

            });
            if (selected) {
                selected.radiobutton('check', 0);
            }
            return sender;
        },
        //获取值
        getValue: function (jq) {
            var sender = $(jq);
            var name = sender.attr('radiosname');
            var inputs = sender.parent().find('input[radiobuttonname="' + name + '"]');
            var selected = null;


            //alert(name);

            inputs.each(function () {



                if ($(this).next().find('.radiobutton-inner:visible').length > 0) {
                    selected = $(this).val();
                }
            });

            return selected;
        },
    };

    //将自定义的插件加入 easyui 的插件组
    $.parser.plugins.push('fixedRadios');
})(jQuery)