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

    //接口地址
    var AjaxUrl = {
        searhStandardPartnumber: '/PvDataApi/Data/SearchStandardPartnumbers/'
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
        //if (row.Eccn != 'EAR99' || isShow) {
        //    initIcons.push('icon-ECCN');
        //}


        if (row.IsUnpopular) {
            initIcons.push('icon-unpopular');
        }

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

    $.fn.standardPartNumerTag = function (opt) {
        return this.each(function () {
            var sender = $(this);

            var options = $.extend(true, {},
                $.fn.standardPartNumerTag.defaults,
                opt || {},
                eval('({' + sender.attr('data-option') + '})'));

            sender.css({ cursor: 'pointer' });
            var rendered = sender.attr('rendered');

            if (rendered == '1') {
                return;
            }
            sender.attr('rendered', '1');

            var partNumber = $.trim(sender.html());

            //如果型号为空就跳出
            if (!partNumber) {
                return;
            }

            $.ajax({
                url: AjaxUrl.searhStandardPartnumber,
                dataType: 'json',
                type: 'GET',
                data: {
                    name: partNumber
                },
                success: function (json) {

                    if (!json.data) {
                        return;
                    }

                    //判断重复加载
                    if (json.data.length == 0) {
                        return;
                    }

                    var data = json.data[0];

                    var icons = getIcons(data);

                    var content = $('<div style="font-size:12px">');

                    var iconsarry = [];
                    for (var index = 0; index < icons.length; index++) {
                        iconsarry.push('<span class="' + icons[index].iconCls + '"></span>');
                    }
                    var eccnSender = $('<span>');
                    var op = setEccn(data.Eccn);
                    if (op) {
                        eccnSender.css('color', op.color);
                        eccnSender.html('[Eccn：' + data.Eccn + ' ' + op.message + ']');
                    } else {
                        eccnSender.css('color', '');
                        eccnSender.html('[Eccn：' + data.Eccn + ']');
                    }

                    //增加原地展示:before
                    if (options.target == 'before') {
                        sender.before(iconsarry.join(' '));
                    }
                    //增加原地展示:after
                    if (options.target == 'after') {
                        sender.after(iconsarry.join(''));
                    }

                    var taxesarry = [];
                    if (data.HSCode) {
                        taxesarry.push('HSCode：' + data.HSCode);
                    }
                    if (data.TaxCode) {
                        taxesarry.push('税务编码：' + data.TaxCode);
                    }
                    if (data.TaxName) {
                        taxesarry.push('税务名称：' + data.TaxName);
                    }



                    var rates = (data.TariffRate ? data.TariffRate : 0) + '%/' +
                        (data.VATRate ? data.VATRate : 0) + '%/' +
                        (data.AddedTariffRate ? data.AddedTariffRate : 0) + '%'

                    if (iconsarry.length > 0) {
                        content.append(iconsarry.join(' ')).append('<br>');
                    }

                    if (data.Eccn) {
                        content.append(eccnSender).append('<br>');
                    }

                    if (taxesarry.length > 0 && false) {//约定不要
                        content.append(taxesarry.join('<br>')).append('<br>');
                    }

                    if (data.TariffRate && false) {//约定不要
                        content.append(rates);
                    }

                    if (content.html().length == 0) {
                        return;
                    }

                    sender.tooltip({
                        position: 'right',
                        content: content,
                        onShow: function () {
                            $(this).tooltip('tip').css({
                            });
                        }
                    });
                },
                error: function (err) {
                    alert('standardPartNumber.tag.error:' + JSON.stringify(err));
                }
            });
        });
    };

    //默认参数
    $.fn.standardPartNumerTag.defaults = {
        target: 'after', // before,after
    };

    $(function () {
        $('partnumber-tip').standardPartNumerTag();
    });

})(jQuery)