/// <reference path="D:\Projects_vs2015\NewSolution\zWebAppTester\Scripts/jquery-1.11.3.min.js" />
/// <reference path="D:\Projects_vs2015\NewSolution\zWebAppTester\Scripts/jquery.easyui-1.4.5.min.js" />
(function ($) {

    $.myWindow = function (options, param) {
        if (typeof options == 'string') {

            var sender = $('options');
            if (sender.length > 0) {
                return sender;
            }

            var method = $.myWindow.methods[options];
            if (method) {
                return method(this, param);
            }
        }

        options = options || {};

        var height = Math.max($(window).height() * .8);
        var width = Math.max($(window).width() * .8);

        var soptions = $.extend({}, $.myWindow.defaults, options);

        if (soptions.width) {
            width = soptions.width;
        }

        if (soptions.width) {
            height = soptions.height;
        }

        //id = "', id, '"
        var ewindow = $(['<div class="easyui-myWindow" style="width: ', width, 'px; height: ', height, 'px">',
            ['<iframe width="100%" height="', 100, '%" frameborder="0"></iframe>'].join(''),
            '</div>'].join(''));
        $(document.body).append(ewindow);

        if (soptions.onClose) {
            var temp_onClose = soptions.onClose;

            soptions.onClose = function () {

                temp_onClose();
                ewindow.window('destroy');
                ewindow.remove();
            };
        }

        //为归类插件添加的事件
        if (soptions.operaIframe) {
            soptions.closed = false;
            soptions.noheader = false;
            soptions.onOpen = function () {
                $(this).css({ 'overflow': 'hidden' });
                soptions.operaIframe(ewindow.find('iframe'));
            }
        }

        ewindow.window(soptions);


        var iframe = ewindow.find('iframe');
        iframe.prop('src', soptions.url);

        ewindow.open = function (url) {
            iframe.prop('src', url);
            ewindow.window('open');
        };

        ewindow.close = function () {
            ewindow.window('close');
            ewindow.window('destroy');
            ewindow.remove();
        };
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

    $.myWindow.methods = {
        options: function (jq) {
            return $.data(jq[0], 'myWindow').options;
        }
    };

    $.myWindow.defaults = {
        onBeforeOpen: function () { },
        onOpen: function () {
            $(this).css({ 'overflow': 'hidden' });
        },
        onClose: function () {

        },
        iconCls: 'icon-save',
        modal: true,
        collapsible: false,
        minimizable: false,
        maximizable: false,
        closable: true,
        closed: true,
        shadow: true,
        modal: true,
        noheader: true,
        resizable: false,
        border: 'thin',
        url: '',
        IsEsc: true,
    };

    $.myWindow.close = function () {
        if (!window.top || !window.top.$) {
            return null;
        }
        var $ = window.top.$;
        var rzlts = $.grep($('iframe'), function (item, index) {
            return $(item).prop('contentWindow') == window;
        });

        $(rzlts).each(function () {
            var ewindow = $(this).parent();
            try {
                ewindow.window('close');
                ewindow.window('destroy');

                ewindow.remove();
            } catch (e) {

            }
        });
    };
    //关闭页面，销毁页面
    $.myWindow.close2 = function () {
        var $ = top.window.$;
        var ewindow = $('.easyui-myWindow').last();
        if (ewindow.length > 0) {
            ewindow.window('destroy');
            ewindow.remove();
        }
    };

})(jQuery);


$(function () {
    $('*').keyup(function (e) {
        if (e.keyCode == 27) {
            var $ = window.top.$;
            var ewindow = $('.easyui-myWindow').last();


            //var dis = ewindow.parent().css('display');
            //alert(dis == 'none');
            //debugger;
            if (ewindow.length > 0) {
                //根据属性判断按Esc键是否触发关闭
                //modify by 沈忱
                var options = ewindow.window("options")
                if (options.IsEsc) {
                    //try {
                    ewindow.window('close', true);
                    //} catch (e) {

                    //}
                    ewindow.window('destroy');
                    ewindow.remove();
                }
            }
        }
        return false;
    });
});