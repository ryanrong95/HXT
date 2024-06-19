/// <reference path="http://fix.szhxd.net/frontframe/jquery-easyui-1.7.6/jquery.min.js" />
/// <reference path="http://fix.szhxd.net/frontframe/jquery-easyui-1.7.6/jquery.easyui.min.js" />

function mwForClose() {
    var $ = top.window.$;

    var ewindow = $('.easyui-myWindow').last();
    if (ewindow.length > 0) {
        //ewindow.window('close', true);
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

        var my0ptions = $.extend({}, $.myWindow.defaults, options);
        my0ptions.onClose = function () {
            mwForClose();
        };

        ewindow.window(my0ptions);

        var iframe = ewindow.find('iframe')[0];
        iframe.onload = function () {
        };

        return ewindow;
    };

    ////建议使用统一的oper 模拟  get;set;块的方法
    //$.myWindow.operMyWindowTarget = function (json) {
    //    if (json) {
    //        top.window.myWindowTarget = json;
    //    } else {
    //        return top.window.myWindowTarget;
    //    }
    //};

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
        operaIframe: null//操作iframe
    };

    $.myWindow.close = function () {
        mwForClose();
    };
})(jQuery);

$(function () {
    $('*').keyup(function (e) {
        if (e.keyCode == 27) {
            mwForClose();
        }
        return false;
    });
});