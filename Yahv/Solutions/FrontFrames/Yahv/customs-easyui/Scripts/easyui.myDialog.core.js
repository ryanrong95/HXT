/// <reference path="http://fixed2.b1b.com/Yahv/jquery-easyui-1.7.6/jquery.min.js" />
/// <reference path="http://fixed2.b1b.com/Yahv/jquery-easyui-1.7.6/jquery.easyui.min.js" />
//增加自动带入f_参数的工作

(function ($) {
    function getAbsoluteUrl(url) {
        var a = document.createElement('a');
        a.href = url;
        url = a.href;
        return url;
    }

    //获取当前地址参数数据
    function queryString() {
        if (location.search.length <= 1) {
            return [];
        }
        var arry = location.search.substring(1).split('&');

        var rtns = [];
        for (var index = 0; index < arry.length; index++) {
            var item = arry[index].split('=');
            if (item.length == 2) {
                rtns.push({
                    key: item[0],
                    value: item[1]
                });
            }

        }
        return rtns;
    }

    var _iframe_id = 'fused_';
    //当前设置数据

    $.myDialogCore = function (options) {

        //alert(document.URL);

        var $ = top.window.$;

        options = options || {};

        if (typeof ($.fn.panel.defaults.zIndex) == 'undefined') {
            $.fn.panel.defaults.zIndex = 9999;
        }

        var options = $.extend({}, $.myDialogCore.defaults, options);

        if (!options.width) {
            options.width = $(window).width() * .8;
        }

        if (!options.height) {
            options.height = $(window).height() * .8;
        }
        var iframeUrl = getAbsoluteUrl(options.url);
        if (options.url == null || !options.url.length) {
            iframeUrl = '';
        }

        var id = _iframe_id + $.fn.panel.defaults.zIndex;

        var query;
        if (iframeUrl.indexOf('?') < 0) {
            query = '?'
        } else {
            query = '&'
        }
        query += 'pagemode=dialog';

        if (location.search.length > 0) {
            var uquery = queryString();
            for (var index = 0; index < uquery.length; index++) {
                var item = uquery[index];
                console.log(item);
                if (item.key.indexOf('f_') == 0) {
                    query += '&' + item.key + '=' + item.value;
                }
            }
        }

        var html = ['<iframe id="', id, '" class="easyui-myDialogCore" style="',
            'background-color:transparent;',
            //'border:dotted 1px red;',
            'position:absolute;',
            'left:0;',
            'top:0;', 'width:100%;', 'height:100%;', 'z-index:', ($.fn.panel.defaults.zIndex++), '"',
            'src="', iframeUrl, query, '"'
            , '>'
            , '</iframe>'].join('');

        function masking() { //加载遮罩  
            $("<div class=\"datagrid-mask\"></div>").css({
                'background-color': 'transparent;',
                display: "block",
                width: "100%",
                height: $(window).height()
            }).appendTo("body");
            $("<div class=\"datagrid-mask-msg\"></div>").html("").appendTo("body").css({
                'background-color': 'transparent',
                border: 'none',
                display: "block",
                left: ($(document.body).outerWidth(true) - 190) / 2,
                top: ($(window).height() - 45) / 2
            });
        }
        function unmasking() { //取消遮罩
            $(".datagrid-mask").remove();
            $(".datagrid-mask-msg").remove();
        }




        delete options.url;

        //添加实体
        $(top.window.document.body).append(html);
        $('#' + id).load(function () {
            unmasking();
        });

        masking();

        var writer = setInterval(function () {
            var doc = top.frames[id].contentDocument;
            if (doc.body) {
                var script = doc.createElement('script');
                script.src = 'http://fixed2.b1b.com/Yahv/customs-easyui/controls/topContentDialog.core.js';
                doc.body.appendChild(script);
                clearInterval(writer);
            }
        }, 100);

        if (!options.onClose) {
            options.onClose = function () {
                $('#' + options.iframeId).remove();
                $('#' + options.iframeId + 'Mask').remove();

            };
        } else {
            var m = options.onClose;
            options.onClose = function () {
                m();
                $('#' + options.iframeId).remove();
                $('#' + options.iframeId + 'Mask').remove();
            };
        }

        var buttons = [];
        //设置按钮
        if (options.isHaveOk) {
            buttons.push({
                iconCls: 'icon-yg-confirm',
                text: '提交',
                handler: function () {
                    if (!!options.sumbitDialog) {
                        options.sumbitDialog();
                    }
                }
            });
        }

        if (options.isHaveCancel) {
            buttons.push({
                iconCls: 'icon-yg-cancel',
                text: '关闭',
                handler: function () {
                    options.onClose();
                }
            });
        }
        options.buttons = buttons;

        options.iframeId = id;

        $.myDialogCore.options.push(options);

        return null;
    };

    if (!top.$.myDialogCore.options) {
        top.$.myDialogCore.options = [];
    }

    $.myDialogCore.defaults = {
        isHaveOk: true,
        isHaveCancel: true,
        modal: true,
        collapsible: false,
        minimizable: false,
        maximizable: true,
        closable: true,
        shadow: true,
        resizable: false,
        border: 'thin',
        url: '',
        title: '',
        onClose: function () { }
    };

    $.myDialogCore.close = function () {
        try {
            // todo 未触发close事件
            var $ = top.window.$;
            var edialog = $('.easyui-myDialogCore').last();
            var id = edialog.prop('id');
            $('#' + id + 'Mask').remove();

            if (edialog.length > 0) {
                edialog.remove();
            }

        } catch (e) {
            console.log(e);
        }
    };
})(jQuery);