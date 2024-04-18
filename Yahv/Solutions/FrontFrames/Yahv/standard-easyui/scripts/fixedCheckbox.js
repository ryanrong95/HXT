/// <reference path="../../jquery-easyui-1.7.6/jquery.easyui.min.js" />
/// <reference path="../../jquery-easyui-1.7.6/jquery.min.js" />

//获取类型

var fixedCheckbox = {
    SupplierType: 'SupplierType'
};

(function ($) {
    function getSupplierType(cb) {
        $.ajax({
            async: false,
            type: "get",
            url: '/PvDataApi/Enums/SupplierType',
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
    if (!top.$.baseData.SupplierType) {
        getSupplierType(function (json) {
            top.$.baseData.SupplierType = json;
        });
    }

    //初始化
    function initMethord(sender, options, data) {
        sender.hide();
        var formName = sender.prop('name');
        if (!formName) {
            alert('initSupplierType.控件name不能为空！2');
        }
        sender.prop('name', '');
        sender.attr('checkname', formName);
        var inputs = [];
        for (var index = data.length - 1; index >= 0; index--) {
            var item = data[index];
            var input = $('<input name="' + formName + '" />');
            if (options.split && index != 0) {
                sender.after(input).after(options.split);
            } else {
                sender.after(input);
            }

            input.val(item['ID']);

            inputs.push(input);

            input.checkbox({
                labelWidth: options.labelWidth,
                label: item['Name'],
                labelPosition: 'after',
                value: item['ID'],
                checked: options.value == item['ID'] || parseInt(options.value) == index,
                onChange: function (checked) {
                    if (checked) {
                        //互斥分组
                        var mutex = options.mutex;
                        if ($(this).val() == mutex) {
                            $.map(inputs, function (item) {
                                if (item.val() != mutex) { //未来可以修改为包涵
                                    item.checkbox('uncheck', 0);
                                }
                                return item;
                            });
                        } else {
                            $.map(inputs, function (item) {
                                if (item.val() == mutex) { //未来可以修改为包涵
                                    item.checkbox('uncheck', 0);
                                }
                                return item;
                            });
                        }
                    }
                }
            });

        }
    }


    //function initSupplierType(sender, options) {

    //    sender.hide();
    //    var formName = sender.prop('name');
    //    if (!formName) {
    //        alert('initSupplierType.控件name不能为空！2');
    //    }

    //    var data = top.$.baseData.SupplierType;
    //    initMethord();
    //}

    //初始化节点
    //var inits = {
    //    SupplierType: initSupplierType
    //};

    //编写fixedCheckbox插件
    $.fn.fixedCheckbox = function (opt, param) {
        //如果options为string，则是方法调用
        if (typeof opt == 'string') {
            var method = $.fn.fixedCheckbox.methods[opt];
            if (method) {
                return method(this, param);//如果有该方法就调用该方法
            } else {
                alert("fixedCheckbox.该插件没有这个方法：" + opt)
            }
        }

        var options = opt || {};
        options = $.extend(true, {}, $.fn.fixedCheckbox.defaults, options);

        return this.each(function () {
            var sender = $(this);

            //保存设置类型
            sender.data('options', options);

            var data = top.$.baseData[options.type];
            if (typeof (data) == 'undefined' || !data) {
                alert('fixedCheckbox.无法找到初始化的类型:' + options.type);
                return;
            }
            initMethord(sender, options, data);
            //结束
        });
    }

    //fixedCheckbox插件的默认配置
    $.fn.fixedCheckbox.defaults = {
        type: '',
        value: '',  //值
        labelWidth: 80, // 标签宽度
        split: '',
        mutex: 0, //互斥值
    };


    //fixedCheckbox插件对外的方法
    $.fn.fixedCheckbox.methods = {
        //保留
        test: function (jq) {
            var sender = $(jq);
            return sender;
        },
        //设置单个值
        setValue: function (jq, value) {
            var sender = $(jq);
            var name = sender.attr('checkname');
            var inputs = sender.parent().find('input[checkboxname="' + name + '"]');
            var selected = null;
            inputs.each(function () {
                if ($(this).val() == value) {
                    selected = $(this);
                }
            });

            if (selected) {
                selected.checkbox('check', 0);
            }
            return sender;
        },
        //设置多个值
        setValues: function (jq, values) {
            var sender = $(jq);
            var name = sender.attr('checkname');
            var inputs = sender.parent().find('input[checkboxname="' + name + '"]');
            inputs.each(function () {
                $(this).checkbox('uncheck', 0);
            });

            for (var index = 0; index < values.length; index++) {
                var selected = null;
                var value = values[index];

                inputs.each(function () {
                    if ($(this).val() == value) {
                        selected = $(this);
                    }
                });

                if (selected) {
                    selected.checkbox('check', 0);
                }
            }
            return sender;
        },
    };

    //将自定义的插件加入 easyui 的插件组
    $.parser.plugins.push('fixedCheckbox');
})(jQuery)