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
            message: '型号不能为空'
        }
    });
    //接口地址
    var AjaxUrl = {
        searhStandardPartnumber: '/PvDataApi/Data/SearchStandardPartnumbers/'
    }

    //设置边框颜色
    function setBorder(sneder, color) {
        sneder.next().css("border-color", color);
    }

    //获取展示icon样式
    function getIcons(selector) {
        var row = selector;
        var initIcons = [];
        var isShow = false;
        if (row.IsCcc || isShow) {
            initIcons.push('icon-CCC');
        }
        if (row.IsEmbargo || isShow) {
            initIcons.push('icon-embargo');
        }
        if (row.CIQprice > 0 || isShow) {
            initIcons.push('icon-CIQ');
        }
        if (row.TariffRate > 0 || isShow) {
            initIcons.push('icon-tariff');
        }
        if (row.AddedTariffRate > 0 || isShow) {
            initIcons.push('icon-AddedTariffs');
        }
        if (row.Eccn != 'EAR99' || isShow) {
            initIcons.push('icon-ECCN');
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

    //编写standardPartNumer插件
    $.fn.standardPartNumer = function (opt, param) {
        //如果options为string，则是方法调用
        if (typeof opt == 'string') {
            var method = $.fn.standardPartNumer.methods[opt];
            if (method) {
                return method(this, param);//如果有该方法就调用该方法
            } else {
                alert("standardPartNumer.该插件没有这个方法")
            }
        }

        return this.each(function () {

            var sender = $(this);

            var options = opt || {};
            var options = $.extend(true, {
            }, $.fn.standardPartNumer.defaults, options);

            var firstQuery = options.value = sender.val();

            //保存设置类型
            sender.data('options', options);

            var sender_id = sender.prop('id');
            if (!sender_id) {
                sender_id = 'standardPartNumer_sender_' + Math.random().toString().substring(2);
                sender.prop('id', sender_id);
            }
            var setTimeoutAjax = 0;

            sender.combobox({
                panelWidth: sender.width() + 6 * 20,
                loader: function (param, success, error) {
                    var q = $.trim(sender.combobox('getText'));
                    q = $.trim(q);
                    if (q.length <= 1) {
                        return false;
                    }
                    clearTimeout(setTimeoutAjax);
                    setTimeoutAjax = setTimeout(function () {
                        $.ajax({
                            url: AjaxUrl.searhStandardPartnumber,
                            dataType: 'json',
                            type: 'GET',
                            data: {
                                name: q
                            },
                            success: function (json) {
                                var items = json.data || [];
                                var selector = null;
                                var items = $.map(json.data, function (item, index) {
                                    if (q == item.Partnumber || q == $.trim(item.Partnumber)) {
                                        selector = item;
                                    }
                                    return item;
                                });
                                if (!selector) {
                                    setBorder(sender, '#FFFF66');

                                    var eccnSender = $(sender.data('options').eccnSelector);
                                    if (eccnSender.length > 0) {
                                        eccnSender.html('');
                                    }
                                    setIcons(sender, null);
                                }
                                sender.data('items', items);
                                success(items);
                            },
                            error: function (err) {
                                alert('standardPartNumer.error:' + JSON.stringify(err));
                            }
                        });
                    }, 100);
                },
                mode: 'remote',
                valueField: 'Partnumber',
                textField: 'Partnumber',
                formatter: function (row) {
                    var arry = [];
                    var isShow = false;

                    if (row.Eccn != 'EAR99' || isShow) {
                        arry.push('<span class="icon-ECCN" title="ECCN"></span>');
                    }

                    if (row.IsCcc || isShow) {
                        arry.push('<span class="icon-CCC" title="CCC"></span>');
                    }
                    if (row.IsEmbargo || isShow) {
                        arry.push('<span class="icon-embargo" title="禁运"></span>');
                    }
                    if (row.CIQprice > 0 || isShow) {
                        arry.push('<span class="icon-CIQ" title="CIQ"></span>');
                    }
                    if (row.TariffRate > 0 || isShow) {
                        arry.push('<span class="icon-tariff" title="关税"></span>');
                    }
                    if (row.AddedTariffRate > 0 || isShow) {
                        arry.push('<span class="icon-AddedTariffs" title="额外关税"></span>');
                    }

                    var html = row.Partnumber + '<span style="float:right">' + arry.join('') + '</span>';

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
                    var eccnSender = $(sender.data('options').eccnSelector);

                    if (selector) {
                        setBorder(sender, '#eee');
                        setIcons(sender, selector);
                        if (eccnSender.length > 0) {
                            eccnSender.html(selector.Eccn);
                        }
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

    $.fn.standardPartNumer.defaults = {
        value: '',
        prompt: '请输入三位以上搜索型号',
        eccnSelector: ''//设置Eccn 展示的区域
    };

    //标准型号名称补齐插件对外的方法
    $.fn.standardPartNumer.methods = {
        //保留测试
        test: function (jq) {
            return $(jq);
        },
        //设置值
        setValue: function (jq, value) {
            var sender = $(jq);
            var options = sender.data('options');
            options.value = value;
            sender.val(value);
            sender.standardPartNumer(options);
            return sender;
        },
        //设置input的值
    };
    //将自定义的插件加入 easyui 的插件组
    $.parser.plugins.push('standardPartNumer');
})(jQuery)