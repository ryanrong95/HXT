/// <reference path="../../jquery-easyui-1.7.6/jquery.easyui.min.js" />
/// <reference path="../../jquery-easyui-1.7.6/jquery.min.js" />

//标准型号名称补齐插件（可以输入可以选择）
//name              表示调用控件的dom的name属性
//有2个<input type="hidden">的input，以下它们的name值，这些值携带了响应的数据
//name              表示用户输入的标准型号名称补齐名称
//name_result       表示某个标准型号名称补齐的数据

/*
标准型号查询
http://hv.erp.b1b.com/PvDataApi/Data/ClassifiedInfo/?partnumber=AD620ARZ-REEL
*/

(function ($) {
    $.extend($.fn.combobox.defaults.rules, {
        isNull: {
            validator: function (value) {
                return $.trim(value) != ''
            },
            message: '品牌不能为空'
        }
    });
    //接口地址
    var AjaxUrl = {
        searhStandardBrand: '/rfqapi/Brands/Search/',
    }

    ////设置边框颜色
    //function setBorder(sneder, color) {
    //    sneder.next().css("border-color", color);
    //}

    //获取展示icon样式
    function getIcons(selector) {
        var row = selector;
        var initIcons = [];
        var isShow = false;
        if (row.IsAgent || isShow) {
            initIcons.push('icon-IsAgent');
        }
        return $.map(initIcons, function (item) {
            return { iconCls: item };
        });
    }

    function setIcons(sender, selector) {
        var geter = selector ? getIcons(selector) : [];
        //设置外围宽度

        var width = sender.combobox('options').width;
        var tsender = sender.next().find('.validatebox-text');
        tsender.width(width + geter.length * 20);
        //设置内围input宽度
        var isender = sender.next();

        isender.width(width + geter.length * 20);

        var rsender = sender.next().find('.textbox-addon-right');
        rsender.find('.myAdd').remove();

        for (var index = 0; index < geter.length; index++) {
            var item = geter[index];
            rsender.prepend('<a href="javascript:;" class="textbox-icon ' + item.iconCls + ' myAdd" icon-index="0" tabindex="-1" style="width: 26px; height: 20px;"></a>');
        }
    }

    //编写crmStandardBrand插件
    $.fn.crmStandardBrand = function (opt, param) {
        //如果options为string，则是方法调用
        if (typeof opt == 'string') {
            var method = $.fn.crmStandardBrand.methods[opt];
            if (method) {
                return method(this, param);//如果有该方法就调用该方法
            } else {
                alert("crmStandardBrand.该插件没有这个方法")
            }
        }

        return this.each(function () {

            var sender = $(this);

            var options = opt || {};
            var options = $.extend(true, {
            }, $.fn.crmStandardBrand.defaults, options);
            var value = sender.val();

            if (value) {
                options.value = value;
            }

            //保存设置类型
            sender.data('options', options);

            var sender_id = sender.prop('id');
            if (!sender_id) {
                sender_id = 'brand_sender_' + Math.random().toString().substring(2);
                sender.prop('id', sender_id);
            }
            var setTimeoutAjax = 0;

            sender.combobox({
                panelWidth: sender.width() + 1 * 20,
                loader: function (param, success, error) {
                    var q = $.trim(sender.combobox('getText'));
                    q = $.trim(q);
                    if (q.length <= 1) {
                        return false;
                    }

                    //alert(q);

                    clearTimeout(setTimeoutAjax);
                    setTimeoutAjax = setTimeout(function () {
                        $.ajax({
                            url: AjaxUrl.searhStandardBrand,
                            dataType: 'json',
                            type: 'GET',
                            data: {
                                key: q
                            },
                            success: function (json) {
                                var data = json.data || [];
                                var selector = null;
                                var items = $.map(data, function (item, index) {
                                    if (q == item.Name || $.trim(q) == $.trim(item.Name)) {
                                        selector = item;
                                    }
                                    return item;
                                });

                                if (!selector) {
                                    //setBorder(sender, '#FFFF66');
                                    setIcons(sender, null);
                                }
                                sender.data('items', items);
                                success(items);
                                sender.combobox('isValid');
                            },
                            error: function (err) {
                                alert('crmStandardBrand.error:' + JSON.stringify(err));
                            }
                        });
                    }, 100);
                },
                mode: 'remote',
                valueField: 'ID',
                textField: 'Name',
                limitToList: true,
                formatter: function (row) {
                    var arry = [];
                    var isShow = false;
                    if (row.IsAgent) {
                        arry.push('<span class="icon-IsAgent" title="IsAgent"></span>');
                    }
                    var html = row.Name + '<span style="float:right">' + arry.join('') + '</span>';

                    return html;
                },
                prompt: options.prompt,
                required: true,
                value: options.value,
                onSelect: function (record) {
                    var partnumber = record.Partnumber;
                    var items = sender.data('items');
                    var selector = null;
                    var items = $.map(items, function (item, index) {
                        if (partnumber == item.Partnumber || partnumber == $.trim(item.Partnumber)) {
                            selector = item;
                        }
                        return item;
                    });

                    if (selector) {
                        //setBorder(sender, '#eee');
                        setIcons(sender, selector);
                    }
                },
                onChange: function (newValue) {

                },
                validType: 'isNull',
                data: [],
                icons: []
            });
        });
    };

    $.fn.crmStandardBrand.defaults = {
        value: '',
        prompt: '请输入两位以上搜索品牌',
        eccnSelector: ''//设置Eccn 展示的区域
    };

    //标准型号名称补齐插件对外的方法
    $.fn.crmStandardBrand.methods = {
        //保留测试
        test: function (jq) {
            return $(jq);
        },
        //设置值
        setValue: function (jq, value) {
            var sender = $(jq);
            sender.combobox('setValue', value);
            sender.combobox('reload');
            return sender;
        },
        //设置input的值
    };
    //将自定义的插件加入 easyui 的插件组
    $.parser.plugins.push('crmStandardBrand');
})(jQuery)