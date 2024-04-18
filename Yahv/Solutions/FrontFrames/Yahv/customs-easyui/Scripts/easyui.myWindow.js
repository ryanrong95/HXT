/// <reference path="http://fixed2.b1b.com/Yahv/jquery-easyui-1.7.6/jquery.min.js" />
/// <reference path="http://fixed2.b1b.com/Yahv/jquery-easyui-1.7.6/jquery.easyui.min.js" />
function mwForClose() {
    var $ = top.window.$;

    var ewindow = $('.easyui-myWindow').last();
    if (ewindow.length > 0) {
        //ewindow.window('close', true);
        ewindow.window('destroy');
        ewindow.remove();
    }
}
function mwForClose2() {
    var $ = top.window.$;

    var ewindow = $('.easyui-myWindow').last();
    if (ewindow.length > 0) {
        ewindow.window('close', true);
        ewindow.window('destroy');
        ewindow.remove();
    }
}
(function ($) {
    function getAbsoluteUrl(url) {
        var a = document.createElement('a');
        a.href = url;
        url = a.href;
        return url;
    }


    $.myWindow = function (options) {

        var $ = top.window.$;

        options = options || {};

        var options = $.extend({}, $.myWindow.defaults, options);

        if (!options.width) {
            options.width = $(window).width() * .8;
        }

        if (!options.height) {
            options.height = $(window).height() * .8;
        }

        if (!options.closable) {
            options.closable = true;
        }

        var iframeUrl = getAbsoluteUrl(options.url);
        if (options.url == null || !options.url.length) {
            iframeUrl = '';
        }

        //alert([document.URL, options.url]);

        var ewindow = $(['<div class="easyui-myWindow" style="overflow:hidden;width: ', options.width, 'px; height: ', options.height, 'px">',
            ['<iframe width="100%" src="', iframeUrl, '" height="', 100, '%" frameborder="0"></iframe>'].join(''),
            '</div>'].join(''));

        delete options.url;

        $(document.body).append(ewindow);

        //为归类插件添加的事件
        if (options.operaIframe) {
            options.onOpen = function () {
                options.operaIframe(ewindow.find('iframe'));
            }
        }

        if (!options.onClose) {
            options.onClose = function () {
                mwForClose();
            };
        } else {
            var m = options.onClose;
            options.onClose = function () {
                m();
                mwForClose();
            };
        }

        ewindow.window(options);

        return ewindow;
    };

    $.myWindow.setMyWindow = function (name, json) {
        top.window[name] = json;
    };

    $.myWindow.getMyWindow = function (name) {
        return top.window[name];
    };

    $.myWindow.removeMyWindow = function (name) {
        delete top.window[name];
    };


    $.myWindow.defaults = {
        onBeforeOpen: function () { },
        onOpen: function () { },
        onClose: function () { },
        iconCls: '',
        modal: true,
        collapsible: false,
        minimizable: false,
        maximizable: true,
        closable: true,
        //closed: true,
        shadow: true,
        //noheader: true,
        resizable: false,
        border: 'thin',
        url: '',
        title: '',
        onClose: function () {

        },
        operaIframe:null//操作iframe
    };

    $.myWindow.close = function () {
        mwForClose2();
    };
    //关闭页面，销毁页面
    $.myWindow.close2 = function () {
        mwForClose();
    };
})(jQuery);

$(function () {
    $('*').keyup(function (e) {
        if (e.keyCode == 27) {
            try {
                mwForClose2();
            } catch (e) {

            }
        }
        return false;
    });
});