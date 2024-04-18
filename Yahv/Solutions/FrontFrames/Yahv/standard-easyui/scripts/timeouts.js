/**
* alert提示，超时自动关闭
* 以下为传入的参数：
*  position: "TR",默认右上，
*     其他值：
*          左上："TL";
*          中上："TC";
*          右上："TR";
*          左下："BL"
*          中下："BC"
*          右下："BR",
           居中："CC"
*  msg："提示信息",
*  type: "info",默认为info（信息）类型
*      其他值：
           信息："info",
           错误："error",
           成功："success",
           警告："warning"
*  timeout: 3000,超时关闭，设置为0则不关闭
*  has_close_btn: true,是否有关闭按钮
*  has_icon: true,是否有类型图标
*  fn:null，关闭弹出框后的回调函数
*
*/
(function ($) {
    $.timeouts = {
        alert: function (options) {
            var options = $.extend(true, {}, $.timeouts.defaults, options);
            var $container;
            if (options.id != null) {
                $container = $('<div class="timeouts-alert" id="' + options.id + '"></div>');//alert容器
            } else {
                $container = $('<div class="timeouts-alert"></div>');//alert容器
            }
            var $icon = $('<div class="timeouts-alert_icon iconfont"></i>');//alert不同类型的图标
            var $p = $('<p class="timeouts-alert_content">' + options.msg + '</p>');//alert的文字
            var $closebtn = $('<i class="timeouts-alert_closeBtn iconfont icon-close"></i>');//alert关闭按钮

            $container.addClass(options.position);
            $container.addClass("timeouts-alert--" + options.type);
            $icon.addClass("icon-" + options.type);

            $container.append($icon);
            $container.append($p);
            $container.append($closebtn);

            //如果有关闭按钮
            if (options.has_close_btn) {
                $closebtn.removeClass("noclosebtn");
            } else {
                $closebtn.addClass("noclosebtn");
            }

            //如果有图标
            if (options.has_icon) {
                $closebtn.removeClass("noicon");
            } else {
                $closebtn.addClass("noicon");
            }

            // 存储container的css值
            var css = {};

            //如果页面中有多个弹出框时计算它的位置
            if (options.position.indexOf("TL") !== -1) {
                $('.timeouts-alert.TL').each(function () {
                    css["top"] = parseInt($(this).css("top")) + this.offsetHeight + 20;
                });
            } else if (options.position.indexOf("TR") !== -1) {
                $('.timeouts-alert.TR').each(function () {
                    css["top"] = parseInt($(this).css("top")) + this.offsetHeight + 20;
                });
            } else if (options.position.indexOf("TC") !== -1) {
                $('.timeouts-alert.TC').each(function () {
                    css["top"] = parseInt($(this).css("top")) + this.offsetHeight + 20;
                });
            } else if (options.position.indexOf("BL") !== -1) {
                $('.timeouts-alert.BL').each(function () {
                    css["bottom"] = parseInt($(this).css("bottom")) + this.offsetHeight + 20;
                });
            } else if (options.position.indexOf("BR") !== -1) {
                $('.timeouts-alert.BR').each(function () {
                    css["bottom"] = parseInt($(this).css("bottom")) + this.offsetHeight + 20;
                });
            } else if (options.position.indexOf("BC") !== -1) {
                $('.timeouts-alert.BC').each(function () {
                    css["bottom"] = parseInt($(this).css("bottom")) + this.offsetHeight + 20;
                });
            } else if (options.position.indexOf("CC") !== -1) {
                $('.timeouts-alert.CC').each(function () {
                    css["bottom"] = parseInt($(this).css("bottom")) + this.offsetHeight + 20;
                });
            }

            $container.css(css);

            if ($container.fadeIn) {
                $container.fadeIn();
            } else {
                $container.css({ display: 'block' });
            }

            var top$ = window.top.$ || $;

            top$("body").append($container);

            //关闭alert的方法
            function removeToast() {
                $.timeouts.remove($container);
                if (options.fn) {
                    options.fn();
                }
            }

            //在options.timeout秒之内自动关闭，如果options.timeout为0，则默认不关闭
            if (options.timeout > 0) {
                setTimeout(removeToast, options.timeout);
            }

            // 关闭按钮的点击事件
            $closebtn.click(function () {
                removeToast();
            });
            return $container;
        }
    };
    //默认配置
    $.timeouts.defaults = {
        position: "TR",
        msg: "提示信息",
        type: "info",//
        timeout: 3000,
        has_close_btn: true,
        has_icon: true,
        fn: null,
        id: null
    }
    //将弹出框关闭并且从body中移除
    $.timeouts.remove = function ($containers) {
        if ($containers.fadeOut) {
            $containers.fadeOut(function () {
                return $containers.remove();
            });
        }
        else {
            $containers.remove();
        }
    }
})(jQuery);