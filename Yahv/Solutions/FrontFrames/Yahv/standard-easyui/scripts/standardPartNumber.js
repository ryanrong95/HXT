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

    //获取当前
    var url = document.scripts[document.scripts.length - 1].src;
    //加载：jqueryform.js
    if (typeof (top.$.baseData.Eccn) == 'undefined' || !$.fn.Eccn) {
        var lower = url.toLowerCase();
        var prexUrl = url.substring(0, lower.indexOf('/yahv/') + '/yahv/'.length);
        var script = '<script src="' + prexUrl + 'standard-easyui/scripts/standardPartNumber.eccn.js"></script' + '>';
        document.write(script);
    }

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
            initIcons.push('icon-ccc1');
        }
        if (row.IsEmbargo || isShow) {
            initIcons.push('icon-embargo1');
        }
        if (row.CIQprice > 0 || isShow) {
            initIcons.push('icon-ciq1');
        }
        if (row.TariffRate > 0 || isShow) {
            initIcons.push('icon-tariff1');
        }
        if (row.AddedTariffRate > 0 || isShow) {
            initIcons.push('icon-addedTariffs1');
        }

        if (row.IsUnpopular > 0 || isShow) {
            //alert(111);
            initIcons.push('icon-unpopular');
        }

        //if (row.Eccn != 'EAR99' || isShow) {
        //    initIcons.push('icon-ECCN');
        //}
        if (row.Eccn || isShow) {
            initIcons.push('icon-eccn1');
        }
        return $.map(initIcons, function (item) {
            return { iconCls: item };
        });
    }

    function setEccn(eccn) {

        if (!eccn) {
            return null;
        }

        eccn = eccn.toLowerCase().replace('.', '');
        var data = top.$.baseData.Eccn;
        var selector = null;
        for (var key in data) {
            var arry = data[key].data;
            for (var index = 0; index < arry.length; index++) {
                if (arry[index] == eccn) {
                    selector = data[key];
                    break;
                }
            }
            if (selector) {
                break;
            }
        }
        return selector;
    }



    function setIcons(sender, selector) {
        var geter = selector ? getIcons(selector) : [];
        //设置外围宽度

        var width = sender.combobox('options').width;
        var tsender = sender.next().find('.validatebox-text');
        tsender.width(width + geter.length * 25);
        //设置内围input宽度
        var isender = sender.next();

        isender.width(width + geter.length * 25);

        var rsender = sender.next().find('.textbox-addon-right');
        rsender.find('.myAdd').remove();

        for (var index = 0; index < geter.length; index++) {
            var item = geter[index];
            rsender.prepend('<a href="javascript:;" class="textbox-icon ' + item.iconCls + ' myAdd" icon-index="0" tabindex="-1" style="width: 28px; height: 20px;"></a>');
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

            var value = sender.val();
            if (value) {
                options.value = value;
            }

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
                                    if (q == item.Partnumber || $.trim(q) == $.trim(item.Partnumber)) {
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
                                sender.combobox('isValid');
                            },
                            error: function (err) {
                                //alert('standardPartNumer.error:' + JSON.stringify(err));
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

                    //if (row.Eccn != 'EAR99' || isShow) {
                    //    arry.push('<span class="icon-ECCN" title="ECCN"></span>');
                    //}
                    if (row.Eccn || isShow) {
                        arry.push('<span class="icon-eccn1" title="ECCN"></span>');
                    }

                    if (row.IsCcc || isShow) {
                        arry.push('<span class="icon-ccc1" title="CCC"></span>');
                    }
                    if (row.IsEmbargo || isShow) {
                        arry.push('<span class="icon-embargo1" title="禁运"></span>');
                    }
                    if (row.CIQprice > 0 || isShow) {
                        arry.push('<span class="icon-ciq1" title="CIQ"></span>');
                    }
                    if (row.TariffRate > 0 || isShow) {
                        arry.push('<span class="icon-tariff1" title="关税"></span>');
                    }
                    if (row.AddedTariffRate > 0 || isShow) {
                        arry.push('<span class="icon-addedTariffs1" title="额外关税"></span>');
                    }

                    if (row.IsUnpopular > 0 || isShow) {
                        arry.push('<span class="icon-unpopular" title="冷偏型号"></span>');
                        //initIcons.push('icon-unpopular');
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
                            var op = setEccn(selector.Eccn);
                            if (op) {
                                eccnSender.css('color', op.color);
                                eccnSender.html(selector.Eccn + '[' + op.message + ']');
                            } else {
                                eccnSender.css('color', '');
                                eccnSender.html(selector.Eccn);
                            }
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
            sender.combobox('setValue', value);
            sender.combobox('reload');
            return sender;
        },
        //设置input的值
    };
    //将自定义的插件加入 easyui 的插件组
    $.parser.plugins.push('standardPartNumer');
})(jQuery)