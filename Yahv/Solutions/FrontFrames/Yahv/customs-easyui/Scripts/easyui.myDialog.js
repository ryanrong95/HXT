/// <reference path="http://fixed2.szhxt.net/Yahv/jquery-easyui-1.7.6/jquery.min.js" />
/// <reference path="http://fixed2.szhxt.net/Yahv/jquery-easyui-1.7.6/jquery.easyui.min.js" />


function mdForClose() {
    var $ = top.window.$;
    var edialog = $('.easyui-myDialog').last();
    if (edialog.length > 0) {
        //edialog.dialog('close', true);
        edialog.dialog('destroy');
        edialog.remove();
    }
}
function mdForClose2() {
    var $ = top.window.$;
    var edialog = $('.easyui-myDialog').last();
    if (edialog.length > 0) {
        edialog.dialog('close', true);
        edialog.dialog('destroy');
        edialog.remove();
    }
}
(function ($) {
    function getAbsoluteUrl(url) {
        var a = document.createElement('a');
        a.href = url;
        url = a.href;
        return url;
    }


    $.myDialog = function (options) {

        //alert(document.referrer);

        //window
        var $ = top.window.$;

        options = options || {};

        var options = $.extend({}, $.myDialog.defaults, options);

        if (!options.width) {
            options.width = $(window).width() * .9;
        }

        if (!options.height) {
            options.height = $(window).height() * .85;
        }

        var iframeUrl = getAbsoluteUrl(options.url);

        var edialog = $(['<div class="easyui-myDialog" style="overflow:hidden;width: ', options.width, 'px; height: ', options.height, 'px">',
            ['<iframe width="100%" src="', iframeUrl, '" height="', 100, '%" frameborder="0"></iframe>'].join(''),
            '</div>'].join(''));

        delete options.url;

        var id = 'edialog_' + Math.random().toString().substring(2);
        edialog.prop('id', id);

        $(document.body).append(edialog);

        if (options.isHaveOk && options.isHaveCancel) {
            options.buttons = [{
                iconCls: 'icon-yg-confirm',
                text: options.OkText,
                handler: function () {
                    var edialog = $('#' + id);
                    var contents = edialog.find('iframe').contents();
                    contents.find('[type="submit"]').click();
                }
            }, {
                iconCls: 'icon-yg-cancel',
                text: '关闭',
                handler: function () {
                    var edialog = $('#' + id);
                    edialog.dialog('close', true);
                    edialog.dialog('destroy');
                    edialog.remove();
                }
            }];
        } else if (!options.isHaveOk && options.isHaveCancel) {
            options.buttons = [{
                iconCls: 'icon-yg-cancel',
                text: '关闭',
                handler: function () {
                    var edialog = $('#' + id);
                    edialog.dialog('close', true);
                    edialog.dialog('destroy');
                    edialog.remove();
                }
            }];
        } else if (options.isHaveOk && !options.isHaveCancel) {
            options.buttons = [{
                iconCls: 'icon-yg-confirm',
                text: options.OkText,
                handler: function () {
                    var edialog = $('#' + id);
                    var contents = edialog.find('iframe').contents();
                    contents.find('[type="submit"]').click();
                }
            }];
        } else if (!options.isHaveOk && !options.isHaveCancel) {
            options.buttons = null;
        }
        if (!options.onClose) {
            options.onClose = function () {
                mdForClose();
            };
        } else {
            var m = options.onClose;
            options.onClose = function () {
                m();
                mdForClose();
            };
        }
        edialog.dialog(options);
        return edialog;
    };


    $.myDialog.setMyDialog = function (name, json) {
        top.window[name] = json;
    };

    $.myDialog.getMyDialog = function (name) {
        return top.window[name];
    };

    $.myDialog.removeMyDialog = function (name) {
        delete top.window[name];
    };


    $.myDialog.defaults = {
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
        OkText: '提交',
        onClose: function () {

        }
    };

    $.myDialog.close = function () {
        mdForClose2();
    };

})(jQuery);

$(function () {
    $('*').keyup(function (e) {

        if (e.keyCode == 27) {
            mdForClose2();
        }
        return false;
    });
});